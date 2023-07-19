using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;

namespace FTMContextNet.Application.Services;

public class CreatePersonOrigin : IServiceRunner
{
    private readonly IPersistedCacheRepository _persistedCacheRepository;
    private readonly IPersistedImportCacheRepository _importCacheRepository;
    private readonly IAuth _auth;
    private readonly Ilog _ilog;

    public CreatePersonOrigin(IPersistedCacheRepository persistedCacheRepository, IPersistedImportCacheRepository importCacheRepository, IAuth auth, Ilog outputHandler)
    {
        _persistedCacheRepository = persistedCacheRepository;
        _importCacheRepository = importCacheRepository;
        _ilog = outputHandler;
        _auth = auth;
    }

    public APIResult Execute()
    {
        if (_auth.GetUser() == -1)
        {
            return new APIResult
            {
                ApiResultType = APIResultType.Unauthorized
            };
        }

        _ilog.WriteLine("Executing Create Person Origin");

        _persistedCacheRepository.CreatePersonOriginEntries(_importCacheRepository.GetCurrentImportId(), _auth.GetUser());


        return new APIResult
        {
            ApiResultType = APIResultType.Success
        };
    }

}