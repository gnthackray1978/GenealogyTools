using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Collections.Grouping;
using FTMContextNet.Domain.Commands;
using FTMContextNet.Domain.ExtensionMethods;
using LoggingLib;
using MediatR;
using MSGIdent;
using MSG.CommonTypes;

namespace FTMContextNet.Application.UserServices.CreateDuplicateList
{
    public class CreateDupeEntrys : IRequestHandler<CreateDuplicateListCommand, CommandResult>
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public CreateDupeEntrys(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository importCacheRepository,
            IAuth auth,
            Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = importCacheRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public async Task<CommandResult> Handle(CreateDuplicateListCommand request,
            CancellationToken cancellationToken)
        {
            if (_auth.GetUser() == -1)
            {
                return CommandResult.Fail(CommandResultType.Unauthorized);
            }

            _ilog.WriteLine("Executing Create Dupe Entries");

            await Task.Run(Execute,cancellationToken);

            return CommandResult.Success();
        }

        private void Execute()
        {
            var groupCollection = new GroupCollection();

            var comparisonPersons =
                _persistedCacheRepository.GetComparisonPersons(_persistedImportCacheRepository.GetCurrentImportId());

            var ignoreList = _persistedCacheRepository.GetIgnoreList();

            int idx = 0;
            int comparisonTotal = comparisonPersons.Count();

            _ilog.WriteLine(comparisonTotal + " records");

            foreach (var cp in comparisonPersons)
            {
                if (idx % 1000 == 0)
                    _ilog.ProgressUpdate(idx, comparisonTotal, " dupes");

                // if this person is in a existing group
                // get that group


                var group = groupCollection.FindById(cp.Id) ?? groupCollection.CreateGroup(cp.ToItem());

                group.AddRange(comparisonPersons
                    .Where(w => w.Equals(cp)
                                && !ignoreList.ContainsPair(w.Surname, cp.Surname)
                                && !group.Contains(w.Id)).Select(s => s.ToItem()));

                groupCollection.WriteGroup(group);


                idx++;
            }

            _ilog.WriteLine("Found: " + groupCollection.Groups.Count());

            groupCollection.SetAggregates();

            var tp = new List<KeyValuePair<int, string>>();

            foreach (var group in groupCollection.Groups.GroupBy(g => g.IncludedTrees))
            {
                _ilog.WriteCounter(group.Key);

                var p = group.OrderByDescending(o => o.LatestTree).First();

                foreach (var person in p.Items)
                {
                    tp.Add(new KeyValuePair<int, string>(person.PersonId, group.Key));
                }
            }

            _persistedCacheRepository.AddDupeEntrys(tp, _auth.GetUser());
        }
    }
}
