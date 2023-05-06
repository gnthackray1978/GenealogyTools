using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreatePersonsAndMarriages
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly GedRepository _gedRepository;
        private readonly Ilog _ilog;

        public CreatePersonsAndMarriages(PersistedCacheRepository persistedCacheRepository,
            GedRepository gedRepository, Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository;
            _gedRepository = gedRepository;
            _ilog = outputHandler;
        }

        public void Execute() {

            _ilog.WriteLine("Executing CreatePersonsAndMarriages");
            


            _gedRepository.ParseLabelledTree();

            var importData = _persistedCacheRepository.AddImportRecord(_gedRepository._GedDb.FileName, _gedRepository._GedDb.FileSize);

            
            _persistedCacheRepository.BeginSavePersons(importData.NextId,_gedRepository._GedDb.Persons.Count);

           
            foreach (var id in importData.CurrentId)
            {
                _persistedCacheRepository.DeletePersons(id);

                _persistedCacheRepository.DeleteMarriages(id);

                _persistedCacheRepository.DeleteImport(id);
            }
             

            foreach (var personSubset in _gedRepository._GedDb.Persons)
            {
                _persistedCacheRepository.SavePersons(personSubset);
            }

            _persistedCacheRepository.SaveAll();

            _persistedCacheRepository.BeginSaveMarriages(importData.NextId,_gedRepository._GedDb.Relationships.Count);

            foreach (var marriageSubset in _gedRepository._GedDb.Relationships)
            {
                _persistedCacheRepository.SaveMarriages(marriageSubset);
            }
            
            _persistedCacheRepository.SaveAll();
        }

    }
}
