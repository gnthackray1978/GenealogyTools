using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MediatR;
using MSGIdent;
using MSG.CommonTypes;

namespace FTMContextNet.Application.UserServices.UpdateImportStatus;

public class UpdateImportStatus : IRequestHandler<UpdateImportStatusCommand, CommandResult>
{
    private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;

    private readonly Ilog _logger;

    private readonly IAuth _auth;

    public UpdateImportStatus(IPersistedImportCacheRepository persistedImportCacheRepository, Ilog logger, IAuth auth)
    {
        _persistedImportCacheRepository = persistedImportCacheRepository;

        _auth = auth;

        _logger = logger;
    }

    public async Task<CommandResult> Handle(UpdateImportStatusCommand request, CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }

        var exists = _persistedImportCacheRepository.ImportExists(request.ImportId);

        //todo magic strings....
        if (!exists) return CommandResult.Fail(CommandResultType.RecordExists, request.ImportId + ": Record doesnt exist");


        var id = "";

        await Task.Run(() =>
        {
            id = _persistedImportCacheRepository.SelectImport(request.ImportId, _auth.GetUser());
        }, cancellationToken);


        return CommandResult.SuccessWithId(id);
    }
}