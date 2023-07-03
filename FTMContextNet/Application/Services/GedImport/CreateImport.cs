using System.Linq;
using FTMContextNet.Application.Models.Create;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;

namespace FTMContextNet.Application.Services.GedImport;

public class CreateImport
{
    private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;

    private readonly Ilog _logger;

    private readonly IAuth _auth;

    public CreateImport(IPersistedImportCacheRepository persistedImportCacheRepository, Ilog logger, IAuth auth)
    {
        _persistedImportCacheRepository = persistedImportCacheRepository;

        _auth = auth;

        _logger = logger;
    }

    public APIResult Execute(CreateImportModel createImportModel)
    {
        if (_auth.GetUser() == -1)
        {
            return new APIResult
            {
                ApiResultType = APIResultType.Unauthorized
            };
        }

        var exists = _persistedImportCacheRepository.ImportExists(createImportModel.FileName, createImportModel.FileSize, _auth.GetUser());

        if (exists) return new APIResult { ApiResultType = APIResultType.RecordExists, Message = "Record exists" };

        var id = _persistedImportCacheRepository.AddImportRecord(createImportModel.FileName, createImportModel.FileSize,
            false, _auth.GetUser());

        return new APIResult
        {
            ApiResultType = APIResultType.Success,
            Id = id.NextId
        };

    }

}
