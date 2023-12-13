using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MediatR;
using MSG.CommonTypes;
using FTMContextNet.Data.Repositories.GedProcessing;
using FTMContextNet.Data.Repositories.TreeAnalysis;

namespace FTMContextNet.Application.UserServices.DeleteImport
{
    public class DeleteImportService : IRequestHandler<DeleteTreeCommand, CommandResult>
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly IGedRepository _gedRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;

        public DeleteImportService(IPersistedCacheRepository persistedCacheRepository,
            IPersistedImportCacheRepository persistedImportCacheRepository,
            IGedRepository gedRepository,
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
            
            _ilog.WriteLine("Deleting persons for import id: " + importId);
            _persistedCacheRepository.DeletePersons(importId);

            _ilog.WriteLine("Deleting relationships for import id: " + importId);
            _persistedCacheRepository.DeleteRelationships(importId);

            _ilog.WriteLine("Deleting dupes for import id: " + importId);
            _persistedCacheRepository.DeleteDupes(importId);

            _ilog.WriteLine("Deleting origins for import id: " + importId);
            _persistedCacheRepository.DeleteOrigins(importId);

            _ilog.WriteLine("Deleting record map groups for import id: " + importId);
            _persistedCacheRepository.DeleteRecordMapGroups(importId);

            _ilog.WriteLine("Deleting tree groups for import id: " + importId);
            _persistedCacheRepository.DeleteTreeGroups(importId);

            _ilog.WriteLine("Deleting tree records for import id: " + importId);
            _persistedCacheRepository.DeleteTreeRecord(importId);

            
            //import
            _persistedImportCacheRepository.DeleteImport(importId);
 
        }

        public async Task<CommandResult> Handle(DeleteTreeCommand request, CancellationToken cancellationToken)
        {
            if (_auth.GetUser() == -1)
            {
                return CommandResult.Fail(CommandResultType.Unauthorized);
            }
             
            await Task.Run(Execute, cancellationToken);

            return CommandResult.Success();
        }
    }
}

