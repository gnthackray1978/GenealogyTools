using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;

namespace FTMContextNet.Application.Services.GedImport;

public class DeleteImport
{
    private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;

    private readonly Ilog _logger;

    private readonly IAuth _auth;

    public DeleteImport(IPersistedImportCacheRepository persistedImportCacheRepository, Ilog logger, IAuth auth)
    {
        _persistedImportCacheRepository = persistedImportCacheRepository;

        _auth = auth;

        _logger = logger;
    }

    public APIResult Execute(int importId)
    {
        if (_auth.GetUser() == -1)
        {
            return new APIResult
            {
                ApiResultType = APIResultType.Unauthorized
            };
        }

        var exists = _persistedImportCacheRepository.ImportExists(importId);

        if (!exists) return new APIResult { ApiResultType = APIResultType.RecordExists, Message = importId + ": Record doesnt exist" };

        _persistedImportCacheRepository.DeleteImport(importId);

        return new APIResult
        {
            ApiResultType = APIResultType.Success
        };
    }

}