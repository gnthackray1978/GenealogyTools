using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroups
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;

        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public CreateTreeGroups(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository, IAuth auth, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public void Execute()
        {

            _ilog.WriteLine("Executing Creating Tree Groups");

            //_persistedCacheRepository.DeleteTreeGroups();

            foreach (var group in _persistedCacheRepository.GetGroupPerson(_persistedImportCacheRepository.GetCurrentImportId()))
            {
                _persistedCacheRepository.InsertTreeGroups(group.Key, group.Value,_persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());
            }

            _ilog.WriteLine("Finished Creating Tree Groups");

        }
    }
}
