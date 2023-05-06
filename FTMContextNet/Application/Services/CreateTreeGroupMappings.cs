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
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly Ilog _ilog;

        public CreateTreeGroupMappings(PersistedCacheRepository persistedCacheRepository, Ilog outputHandler)
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

                var tp = _persistedCacheRepository.GetGroups();

                //
                var idx = 0;

                foreach (var grp in tp)
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
    }
}
