using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services;

public class CreatePersonOrigin
{
    private readonly PersistedCacheRepository _persistedCacheRepository;
    private readonly Ilog _ilog;

    public CreatePersonOrigin(PersistedCacheRepository persistedCacheRepository, Ilog outputHandler)
    {
        _persistedCacheRepository = persistedCacheRepository;
        _ilog = outputHandler;
    }

    public void Execute()
    {

        _ilog.WriteLine("Executing Create Person Origin");

        _persistedCacheRepository.CreatePersonOriginEntries(1);
    }

}