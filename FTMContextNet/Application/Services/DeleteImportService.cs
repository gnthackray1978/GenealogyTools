using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Auth;
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

        public void Execute()
        {
            var importId = _persistedImportCacheRepository.GetCurrentImportId();
        
            //personview
            _persistedCacheRepository.DeletePersons(importId);
            //marriages
            _persistedCacheRepository.DeleteMarriages(importId);
           
           
            //dupeentries
            _persistedCacheRepository.DeleteDupes(importId);
            //personorigins
            _persistedCacheRepository.DeleteOrigins(_auth.GetUser());
            //treerecordmapgroups
            _persistedCacheRepository.DeleteRecordMapGroups(importId);
            //treegroups
            _persistedCacheRepository.DeleteTreeGroups(importId);
            //treerecords
            _persistedCacheRepository.DeleteTreeRecords(importId);

            //import
            _persistedImportCacheRepository.DeleteImport(importId);
        }
    }
}

