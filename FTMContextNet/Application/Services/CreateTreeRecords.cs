using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeRecords
    {
        private readonly PersistedCacheRepository _persistedCacheRepository; 
        private readonly Ilog _ilog;

        public CreateTreeRecords(PersistedCacheRepository persistedCacheRepository,  Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository; 
            _ilog = outputHandler;
        }

        public void Execute() {

            _ilog.WriteLine("Executing Creating Tree Records");

            _persistedCacheRepository.DeleteTreeRecords();

            _persistedCacheRepository.PopulateTreeRecordsFromCache();
        }

    }
}
