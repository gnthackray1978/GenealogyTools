using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreatePersonsAndMarriages
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly InMemoryCacheRepository _inMemoryCacheRepository;
        private readonly Ilog _ilog;

        public CreatePersonsAndMarriages(PersistedCacheRepository persistedCacheRepository,InMemoryCacheRepository inMemoryCacheRepository, Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository;
            _inMemoryCacheRepository = inMemoryCacheRepository;
            _ilog = outputHandler;
        }

        public void Execute() {

            _ilog.WriteLine("Executing CreatePersonsAndMarriages");

            _persistedCacheRepository.DeletePersons();
            
            _persistedCacheRepository.BeginSavePersons(_inMemoryCacheRepository.GetPersonCacheSize());

            foreach (var personSubset in _inMemoryCacheRepository.GetPersons())
            {
                var birthDateRange = _inMemoryCacheRepository.GetPersonBirthDateRange(personSubset.Id);

                var associatedLocationData = _inMemoryCacheRepository.GetAllLocationsForPerson(personSubset.Id);

                var parents = _inMemoryCacheRepository.GetParentIds(personSubset.Id);

                var origin = _inMemoryCacheRepository.GetOrigin(personSubset.Id);
                
                _persistedCacheRepository.SavePersons(personSubset, birthDateRange,associatedLocationData, parents, origin);
            }

            _persistedCacheRepository.SaveAll();

            _persistedCacheRepository.BeginSaveMarriages(_inMemoryCacheRepository.GetMarriageCacheSize());
            
            foreach (var marriageSubset in _inMemoryCacheRepository.GetMarriages())
            {
                _persistedCacheRepository.SaveMarriages(marriageSubset);
            }

            _persistedCacheRepository.SaveAll();
        }

    }
}
