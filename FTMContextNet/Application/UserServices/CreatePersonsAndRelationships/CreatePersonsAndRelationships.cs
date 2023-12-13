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
using System.Collections.Generic;
using System.Linq;

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
        private int _currentImportId;
        private int _currentUserId;

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

            _ilog.WriteLine("Executing CreatePersonsAndRelationships Handler");

            _currentImportId = _persistedImportCacheRepository.GetCurrentImportId();

            _currentUserId = _auth.GetUser();


            await AddTreeRecord(cancellationToken);

            AddTreeMetaData();

            await AddGroups(cancellationToken);

            return CommandResult.Success();
        }

        private bool IsTreeImported()
        {
            return _persistedCacheRepository.ImportPresent(_currentImportId);
        }

        private async Task AddTreeRecord(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                string path = Path.Combine(_iMSGConfigHelper.GedPath, _persistedImportCacheRepository.GedFileName());

                _ilog.WriteLine("Parsing Tree");

                var gedDb = _gedRepository.ParseLabelledTree(path);
                
                _ilog.WriteLine("Adding Person Details");

                _persistedCacheRepository.InsertPersons(_currentImportId, _currentUserId, gedDb.Persons);

                _ilog.WriteLine("Adding Relationships");

                _persistedCacheRepository.InsertRelationships(_currentImportId, _currentUserId, gedDb.Relationships);

                _ilog.WriteLine("Adding Person Tree Origins");

                _persistedCacheRepository.CreatePersonOriginEntries(_currentImportId, _currentUserId);

            }, cancellationToken);
        }

        private async Task AddGroups(CancellationToken cancellationToken)
        {
            var groupPersons = _persistedCacheRepository.GetGroupPerson(_currentImportId);

            var groups = _persistedCacheRepository.GetGroups(_currentImportId);

            await Task.Run(() =>
            {
                _ilog.WriteLine("Adding Tree Groups");

                _persistedCacheRepository.InsertTreeGroups(groupPersons, _currentImportId, _currentUserId);

                _ilog.WriteLine("Adding Tree Group Mappings");

                _persistedCacheRepository.InsertTreeRecordMapGroup(groups, _currentImportId, _currentUserId);

                _ilog.WriteLine("Finished Create Tree Group Mappings");

            }, cancellationToken);
        }

        private void AddTreeMetaData()
        {
            _ilog.WriteLine("Creating Tree Records");

            _persistedCacheRepository.PopulateTreeRecordFromCache(_currentUserId, _currentImportId);
        }
    }
}
