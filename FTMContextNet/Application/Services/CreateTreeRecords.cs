using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeRecords
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly Ilog _ilog;

        public CreateTreeRecords(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository,  Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _ilog = outputHandler;
        }

        public void Execute() {

            _ilog.WriteLine("Executing Creating Tree Records");

            _persistedCacheRepository.PopulateTreeRecordsFromCache(_persistedImportCacheRepository.GetCurrentImportId());
        }

    }
}
