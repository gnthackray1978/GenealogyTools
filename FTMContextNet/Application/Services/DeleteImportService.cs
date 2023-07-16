using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class DeleteImportService
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly GedRepository _gedRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public DeleteImportService(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository,
            GedRepository gedRepository,
            IAuth auth,
            Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _gedRepository = gedRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public APIResult Execute()
        {
            if (_auth.GetUser() == -1)
            {
                return new APIResult
                {
                    ApiResultType = APIResultType.Unauthorized
                };
            }

            var importId = _persistedImportCacheRepository.GetCurrentImportId();
        
            //personview
            _persistedCacheRepository.DeletePersons(importId);
            _ilog.WriteLine("Deleting persons for import id: " + importId);
            //marriages
            _persistedCacheRepository.DeleteMarriages(importId);
            _ilog.WriteLine("Deleting marriages for import id: " + importId);
            //dupeentries
            _persistedCacheRepository.DeleteDupes(importId);
            _ilog.WriteLine("Deleting dupes for import id: " + importId);
            //personorigins
            _persistedCacheRepository.DeleteOrigins(importId);
            _ilog.WriteLine("Deleting origins for import id: " + importId);
            //treerecordmapgroups
            _persistedCacheRepository.DeleteRecordMapGroups(importId);
            _ilog.WriteLine("Deleting record map groups for import id: " + importId);
            //treegroups
            _persistedCacheRepository.DeleteTreeGroups(importId);
            _ilog.WriteLine("Deleting tree groups for import id: " + importId);
            //treerecords
            _persistedCacheRepository.DeleteTreeRecords(importId);
            _ilog.WriteLine("Deleting tree records for import id: " + importId);
            //import
            _persistedImportCacheRepository.DeleteImport(importId);

            return new APIResult
            {
                ApiResultType = APIResultType.Success
            };
        }
    }
}

