﻿using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MediatR;

namespace FTMContextNet.Application.UserServices.CreatePersonsAndRelationships
{
    public class CreatePersonsAndMarriages : IRequestHandler<CreatePersonAndRelationshipsCommand, CommandResult>
    {
        private static readonly SemaphoreSlim RateLimit = new SemaphoreSlim(1, 1);
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly GedRepository _gedRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public CreatePersonsAndMarriages(IPersistedCacheRepository persistedCacheRepository,
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
          
        public async Task<CommandResult> Handle(CreatePersonAndRelationshipsCommand request, CancellationToken cancellationToken)
        {
            if (_auth.GetUser() == -1)
            {
                return CommandResult.Fail(CommandResultType.Unauthorized);
            }
            
            await AddTreeRecords(cancellationToken);

            AddTreeMetaData();

            AddGroups();

            return CommandResult.Success();
        }

        private async Task AddTreeRecords(CancellationToken cancellationToken)
        {
            _ilog.WriteLine("Executing CreatePersonsAndMarriages");


            await Task.Run(() =>
            {
                var gedDb = _gedRepository.ParseLabelledTree();

                _persistedCacheRepository.InsertPersons(_persistedImportCacheRepository.GetCurrentImportId(),
                    _auth.GetUser(), gedDb.Persons);

                _persistedCacheRepository.InsertMarriages(_persistedImportCacheRepository.GetCurrentImportId(),
                    _auth.GetUser(), gedDb.Relationships);
            }, cancellationToken);
        }

        private void AddGroups()
        { 
            _ilog.WriteLine("Creating Tree Groups");

            foreach (var group in _persistedCacheRepository.GetGroupPerson(_persistedImportCacheRepository.GetCurrentImportId()))
            {
                _persistedCacheRepository.InsertTreeGroups(group.Key, group.Value, _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());
            }


            try
            {
                _ilog.WriteLine("Creating Tree Group Mappings");

                var groups = _persistedCacheRepository.GetGroups(_persistedImportCacheRepository.GetCurrentImportId());

                var idx = 0;

                foreach (var grp in groups)
                {
                    foreach (var mapping in grp.Value)
                    {
                        _persistedCacheRepository.InsertTreeRecordMapGroup(idx, mapping, grp.Key, _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());

                        idx++;
                    }
                }

                _ilog.WriteLine("Finished Create Tree Group Mappings");
            }
            finally
            {
                RateLimit.Release();
            }

        }

        private void AddTreeMetaData()
        {
            _ilog.WriteLine("Creating Tree Records");

            _persistedCacheRepository.PopulateTreeRecordsFromCache(_persistedImportCacheRepository.GetCurrentImportId());
        }
    }
}