using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroupMappings
    {
        private static readonly SemaphoreSlim RateLimit = new SemaphoreSlim(1,1);
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly Ilog _ilog;

        public CreateTreeGroupMappings(IPersistedCacheRepository persistedCacheRepository, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ilog = outputHandler;
        }



        public async void Execute()
        {
            await RateLimit.WaitAsync();


            try
            {
                _ilog.WriteLine("Executing Create Tree Group Mappings");

                _persistedCacheRepository.DeleteRecordMapGroups();

                var groups = _persistedCacheRepository.GetGroups();
                
                var idx = 0;

                foreach (var grp in groups)
                {
                    foreach (var mapping in grp.Value)
                    {
                        _persistedCacheRepository.SaveTreeRecordMapGroup(idx, mapping, grp.Key);

                        idx++;
                    }
                }

                _ilog.WriteLine("Finished Create Tree Group Mappings");
            }
            finally
            {
                RateLimit.Release();
            }
            
        }

        private Dictionary<string, List<string>> GetGroups()
        {
            var results = new Dictionary<string, List<string>>();

            var treeIds = _persistedCacheRepository.GetTreeIds();

            var relationships = _persistedCacheRepository.GetRelationships();

            var treeNameDictionary = _persistedCacheRepository.GetRootNameDictionary();

            var groupNames = _persistedCacheRepository.GetGroupNamesDictionary();

            foreach (var treeId in treeIds)
            {
                var groupMembers = relationships.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();

                results.Add(treeNameDictionary[treeId], (from gm in groupMembers where groupNames.ContainsKey(gm) select groupNames[gm]).ToList());
            }

            return results;
        }
    }
}
