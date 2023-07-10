using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using LoggingLib;

namespace FTMContextNet.Application.Services;

public class CreatePersonOrigin
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

    public void Execute()
    {

        _ilog.WriteLine("Executing Create Person Origin");

        _persistedCacheRepository.CreatePersonOriginEntries(_importCacheRepository.GetCurrentImportId(), _auth.GetUser());


    }

}