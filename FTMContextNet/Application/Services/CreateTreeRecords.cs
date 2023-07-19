using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeRecords : IServiceRunner
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public CreateTreeRecords(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository, IAuth auth, Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public APIResult Execute() {

            if (_auth.GetUser() == -1)
            {
                return new APIResult
                {
                    ApiResultType = APIResultType.Unauthorized
                };
            }

            _ilog.WriteLine("Executing Creating Tree Records");

            _persistedCacheRepository.PopulateTreeRecordsFromCache(_persistedImportCacheRepository.GetCurrentImportId());

            return new APIResult
            {
                ApiResultType = APIResultType.Success
            };
        }

    }
}
