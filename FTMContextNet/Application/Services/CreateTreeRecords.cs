using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeRecords
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly FTMMakerRepository _ftmMakerRepository;
        private readonly Ilog _ilog;

        public CreateTreeRecords(PersistedCacheRepository persistedCacheRepository, FTMMakerRepository ftmMakerRepository, Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository;
            _ftmMakerRepository = ftmMakerRepository;
            _ilog = outputHandler;
        }

        public void Execute() {

            _ilog.WriteLine("Executing Creating Tree Records");

            _persistedCacheRepository.DeleteTreeRecords();

            _ftmMakerRepository.GetGroups();

            _persistedCacheRepository.PopulateTreeRecordsFromCache();
        }

    }
}
