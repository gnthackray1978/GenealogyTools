using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConfigHelper;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MSG.CommonTypes;
using MediatR;
using FTMContextNet.Data.Repositories.GedProcessing;
using FTMContextNet.Data.Repositories.TreeAnalysis;

namespace FTMContextNet.Application.UserServices.CreatePersonsAndRelationships
{
    public class CreatePersonsAndRelationships : IRequestHandler<CreatePersonAndRelationshipsCommand, CommandResult>
    {
       //private static readonly SemaphoreSlim RateLimit = new SemaphoreSlim(1, 1);
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly IGedRepository _gedRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;
        private readonly IMSGConfigHelper _iMSGConfigHelper;

        public CreatePersonsAndRelationships(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository,
            IGedRepository gedRepository,
            IAuth auth,
            Ilog outputHandler,
            IMSGConfigHelper iMSGConfigHelper)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _gedRepository = gedRepository;
            _ilog = outputHandler;
            _auth = auth;
            _iMSGConfigHelper = iMSGConfigHelper;
        }
          
        public async Task<CommandResult> Handle(CreatePersonAndRelationshipsCommand request, CancellationToken cancellationToken)
        {
            if (_auth.GetUser() == -1)
            {
                return CommandResult.Fail(CommandResultType.Unauthorized);
            }

            if (IsTreeImported())
            {
                return CommandResult.Fail(CommandResultType.RecordExists);
            }

            //check if tree has been imported already if so then abort.


            await AddTreeRecord(cancellationToken);

            AddTreeMetaData();

            AddGroups();

            return CommandResult.Success();
        }

        private bool IsTreeImported()
        {
            var importId = _persistedImportCacheRepository.GetCurrentImportId();

            return _persistedCacheRepository.ImportPresent(importId);
        }

        private async Task AddTreeRecord(CancellationToken cancellationToken)
        {
            _ilog.WriteLine("Executing CreatePersonsAndRelationships");

             
            await Task.Run(() =>
            {
                string path = Path.Combine(_iMSGConfigHelper.GedPath, _persistedImportCacheRepository.GedFileName());

                var gedDb = _gedRepository.ParseLabelledTree(path);

                _persistedCacheRepository.InsertPersons(_persistedImportCacheRepository.GetCurrentImportId(),
                    _auth.GetUser(), gedDb.Persons);

                _persistedCacheRepository.InsertMarriages(_persistedImportCacheRepository.GetCurrentImportId(),
                    _auth.GetUser(), gedDb.Relationships);

                _persistedCacheRepository.CreatePersonOriginEntries(_persistedImportCacheRepository.GetCurrentImportId(),_auth.GetUser());

            }, cancellationToken);
        }

        private void AddGroups()
        { 
            _ilog.WriteLine("Creating Tree Groups");

            foreach (var group in _persistedCacheRepository.GetGroupPerson(_persistedImportCacheRepository.GetCurrentImportId()))
            {
                _persistedCacheRepository.InsertTreeGroups(group.Key, group.Value,
                    _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());
            }
            
            _ilog.WriteLine("Creating Tree Group Mappings");

            var groups = _persistedCacheRepository.GetGroups(_persistedImportCacheRepository.GetCurrentImportId());
        
            foreach (var grp in groups)
            {
                foreach (var mapping in grp.Value)
                {
                    _persistedCacheRepository.InsertTreeRecordMapGroup( mapping, grp.Key, _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());
                
                }
            }
            
            _ilog.WriteLine("Finished Create Tree Group Mappings");
        }

        private void AddTreeMetaData()
        {
            _ilog.WriteLine("Creating Tree Records");

            _persistedCacheRepository.PopulateTreeRecordFromCache(_auth.GetUser(), _persistedImportCacheRepository.GetCurrentImportId());
        }
    }
}
