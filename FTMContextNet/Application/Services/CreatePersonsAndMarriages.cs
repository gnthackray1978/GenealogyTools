using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class CreatePersonsAndMarriages
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly GedRepository _gedRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public CreatePersonsAndMarriages(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository,
            GedRepository gedRepository,
            IAuth auth,
            Ilog outputHandler) {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _gedRepository = gedRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public APIResult Execute() {

            if (_auth.GetUser() == -1)
            {
                return new APIResult
                {
                    ApiResultType = APIResultType.Unauthorized
                };
            }

            _ilog.WriteLine("Executing CreatePersonsAndMarriages");
            
            var gedDb =_gedRepository.ParseLabelledTree();
 
            _persistedCacheRepository.InsertPersons(_persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser(), gedDb.Persons);
            
            _persistedCacheRepository.InsertMarriages(_persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser(), gedDb.Relationships);


            return new APIResult
            {
                ApiResultType = APIResultType.Success
            };
        }

    }
}
