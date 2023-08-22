using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using LoggingLib;
using MediatR;

namespace FTMContextNet.Application.UserServices.GetTreeImportStatus
{
    public class GetTreeImportStatus : IRequestHandler<GetTreeImportStatusQuery, bool>
    {
        private readonly Ilog _iLog;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly IPersistedCacheRepository _persistedCacheRepository; 

        public GetTreeImportStatus(IPersistedCacheRepository persistedCacheRepository, IPersistedImportCacheRepository persistedImportCacheRepository, Ilog outputHandlerp)
        {
            _iLog = outputHandlerp;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _persistedCacheRepository = persistedCacheRepository;
        }

        public async Task<bool> Handle(GetTreeImportStatusQuery request,
            CancellationToken cancellationToken)
        {
            _iLog.WriteLine("Executing GetInfoService");

            bool isFound = false;

            var currentImportId = _persistedImportCacheRepository.GetCurrentImportId();

            //todo do this in a better way
            await Task.Run(() =>
                isFound = _persistedCacheRepository.ImportPresent(currentImportId));


            return isFound;
        }
    }
}
