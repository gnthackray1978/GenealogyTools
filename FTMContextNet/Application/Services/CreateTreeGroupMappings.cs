using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroupMappings
    {
        private static readonly SemaphoreSlim RateLimit = new SemaphoreSlim(1,1);
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly IAuth _auth;
        private readonly Ilog _ilog;

        public CreateTreeGroupMappings(IPersistedCacheRepository persistedCacheRepository, 
            IPersistedImportCacheRepository persistedImportCacheRepository, IAuth auth, Ilog outputHandler)
        {
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _persistedCacheRepository = persistedCacheRepository;
            _ilog = outputHandler;
            _auth = auth;
        }
        
        public async void Execute()
        {
            await RateLimit.WaitAsync();


            try 
            {
                _ilog.WriteLine("Executing Create Tree Group Mappings");

                var groups = _persistedCacheRepository.GetGroups(_persistedImportCacheRepository.GetCurrentImportId());
                
                var idx = 0;

                foreach (var grp in groups)
                {
                    foreach (var mapping in grp.Value)
                    {
                        _persistedCacheRepository.InsertTreeRecordMapGroup(idx, mapping, grp.Key, _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());

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
