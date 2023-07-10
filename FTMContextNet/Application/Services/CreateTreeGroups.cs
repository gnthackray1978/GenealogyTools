using FTMContextNet.Data.Repositories;
using FTMContextNet.Domain.Auth;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroups
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public CreateTreeGroups(PersistedCacheRepository persistedCacheRepository,IAuth auth, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public void Execute()
        {

            _ilog.WriteLine("Executing Creating Tree Groups");

            _persistedCacheRepository.DeleteTreeGroups();

            foreach (var group in _persistedCacheRepository.GetGroupPerson())
            {
                _persistedCacheRepository.InsertTreeGroups(group.Key, group.Value, _auth.GetUser());
            }

            _ilog.WriteLine("Finished Creating Tree Groups");

        }
    }
}
