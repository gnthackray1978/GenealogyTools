using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;
using MediatR;

namespace FTMContextNet.Application.UserServices.CreateGedImport;

public class CreateImport : IRequestHandler<CreateImportCommand, CommandResult>
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
    
    public async Task<CommandResult> Handle(CreateImportCommand request, CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }

        var exists = _persistedImportCacheRepository.ImportExists(request.FileName, request.FileSize, _auth.GetUser());
         
        if (!exists) return CommandResult.Fail(CommandResultType.RecordExists, "Record exists");
        
        ImportData id  = new ImportData();

        await Task.Run(() =>
        {
            id = _persistedImportCacheRepository.AddImportRecord(request.FileName, request.FileSize,    false, _auth.GetUser());
        }, cancellationToken);
        
        return CommandResult.SuccessWithId(id.NextId.ToString());
    }
}
