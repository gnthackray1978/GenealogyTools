using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MediatR;

//DeleteImportCommand
namespace FTMContextNet.Application.UserServices.DeleteImport;

public class DeleteImport : IRequestHandler<DeleteImportCommand, CommandResult>
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
     
    public async Task<CommandResult> Handle(DeleteImportCommand request, CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }

        var exists = _persistedImportCacheRepository.ImportExists(request.ImportId);

        //todo magic strings      
        if(!exists) return CommandResult.Fail(CommandResultType.RecordExists, request.ImportId + ": Record doesnt exist");
         
        await Task.Run(() =>
        {
           _persistedImportCacheRepository.DeleteImport(request.ImportId);
        }, cancellationToken);


        return CommandResult.Success();
    }
}