using FTMContextNet.Data.Repositories;
using LoggingLib;
using System.Collections.Generic;
using System.Linq;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Collections.Grouping;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.ExtensionMethods;

namespace FTMContextNet.Application.Services
{
    public class CreateDupeEntrys
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

        public APIResult Execute()
        {
            //  _persistedCacheRepository.DeleteDupes();
            if (_auth.GetUser() == -1)
            {
                return new APIResult
                {
                    ApiResultType = APIResultType.Unauthorized
                };
            }

            _ilog.WriteLine("Executing Create Dupe Entries");

            var groupCollection = new GroupCollection();

            var comparisonPersons = _persistedCacheRepository.GetComparisonPersons(_persistedImportCacheRepository.GetCurrentImportId());

            var ignoreList = _persistedCacheRepository.GetIgnoreList();

            

            int idx = 0;
            int comparisonTotal = comparisonPersons.Count();

            _ilog.WriteLine(comparisonTotal + " records");

            foreach (var cp in comparisonPersons)
            {
                //12707
                //13124

                //if (cp.Fact.Surname== "Bates" || cp.Fact.Surname == "Briggs")
                //{
                //    Debug.WriteLine("stop");
                //}

                if (idx % 1000 == 0)
                    _ilog.ProgressUpdate(idx, comparisonTotal," dupes");

                // if this person is in a existing group
                // get that group



                var group = groupCollection.FindById(cp.Id) ?? groupCollection.CreateGroup(cp.ToItem());
                
                group.AddRange(comparisonPersons
                    .Where(w => w.Equals(cp)
                                && !ignoreList.ContainsPair(w.Surname, cp.Surname)
                                && !group.Contains(w.Id)).Select(s =>s.ToItem()));

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



            _persistedCacheRepository.AddDupeEntrys(tp,_auth.GetUser());


            return new APIResult
            {
                ApiResultType = APIResultType.Success
            };
        }
        
    }
}
