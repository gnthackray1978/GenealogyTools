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
            
            var gedDb =_gedRepository.ParseLabelledTree();

            var importData = _persistedCacheRepository.AddImportRecord(gedDb.FileName, gedDb.FileSize);
            
            foreach (var id in importData.CurrentId)
            {
                _persistedCacheRepository.DeletePersons(id);

                _persistedCacheRepository.DeleteMarriages(id);

                _persistedCacheRepository.DeleteImport(id);
            }
             
            _persistedCacheRepository.SavePersons(importData.NextId, gedDb.Persons);
            
            _persistedCacheRepository.SaveMarriages(importData.NextId, gedDb.Relationships);
             
        }

    }
}
