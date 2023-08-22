using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MSG.CommonTypes;
using MediatR;

namespace FTMContextNet.Application.UserServices.CreatePersonOriginsList;

public class CreatePersonOrigin : IRequestHandler<CreatePersonOriginListCommand, CommandResult>
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
     
    public async Task<CommandResult> Handle(CreatePersonOriginListCommand request, 
        CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }

        _ilog.WriteLine("Executing Create Person Origin");

        await Task.Run(()=>_persistedCacheRepository.CreatePersonOriginEntries(_importCacheRepository.GetCurrentImportId(), _auth.GetUser()), cancellationToken);

        return CommandResult.Success();
    }
}