using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreateTreeGroups
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly Ilog _ilog;

        public CreateTreeGroups(PersistedCacheRepository persistedCacheRepository, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ilog = outputHandler;
        }

        public void Execute()
        {

            _ilog.WriteLine("Executing Creating Tree Groups");

            _persistedCacheRepository.DeleteTreeGroups();

            foreach (var group in _persistedCacheRepository.GetGroupPerson())
            {
                _persistedCacheRepository.SaveTreeGroups(group.Key, group.Value);
            }

            _ilog.WriteLine("Finished Creating Tree Groups");

        }
    }
}
