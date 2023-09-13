//using System.Threading;
//using System.Threading.Tasks;
//using FTMContextNet.Application.Services;
//using FTMContextNet.Application.UserServices.CreatePersonLocationsInCache;
//using FTMContextNet.Data.Repositories;
//using FTMContextNet.Data.Repositories.GedImports;
//using MSGIdent;
//using FTMContextNet.Domain.Commands;
//using LoggingLib;
//using MediatR;

//namespace FTMContextNet.Application.UserServices.CreateTreeRecord
//{
//    public class CreateTreeRecord : IRequestHandler<CreateTreeRecordCommand, CommandResult>
//    {
//        private static readonly SemaphoreSlim RateLimit = new SemaphoreSlim(1, 1);
//        private readonly IPersistedCacheRepository _persistedCacheRepository;
//        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
//        private readonly Ilog _ilog;
//        private readonly IAuth _auth;

//        public CreateTreeRecord(IPersistedCacheRepository persistedCacheRepository,
//            IPersistedImportCacheRepository persistedImportCacheRepository, IAuth auth, Ilog outputHandler)
//        {
//            _persistedCacheRepository = persistedCacheRepository;
//            _persistedImportCacheRepository = persistedImportCacheRepository;
//            _ilog = outputHandler;
//            _auth = auth;
//        }

//        private void Execute()
//        { 
//            _ilog.WriteLine("Creating Tree Records");

//            _persistedCacheRepository.PopulateTreeRecordFromCache(_persistedImportCacheRepository.GetCurrentImportId());

//            _ilog.WriteLine("Creating Tree Groups");

//            foreach (var group in _persistedCacheRepository.GetGroupPerson(_persistedImportCacheRepository.GetCurrentImportId()))
//            {
//                _persistedCacheRepository.InsertTreeGroups(group.Key, group.Value, _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());
//            }


//            try
//            {
//                _ilog.WriteLine("Creating Tree Group Mappings");

//                var groups = _persistedCacheRepository.GetGroups(_persistedImportCacheRepository.GetCurrentImportId());

//                var idx = 0;

//                foreach (var grp in groups)
//                {
//                    foreach (var mapping in grp.Value)
//                    {
//                        _persistedCacheRepository.InsertTreeRecordMapGroup(idx, mapping, grp.Key, _persistedImportCacheRepository.GetCurrentImportId(), _auth.GetUser());

//                        idx++;
//                    }
//                }

//                _ilog.WriteLine("Finished Create Tree Group Mappings");
//            }
//            finally
//            {
//                RateLimit.Release();
//            }
//        }

//        public async Task<CommandResult> Handle(CreateTreeRecordCommand request, CancellationToken cancellationToken)
//        {
//            if (_auth.GetUser() == -1)
//            {
//                return CommandResult.Fail(CommandResultType.Unauthorized);
//            }
 
//            await Task.Run(Execute, cancellationToken);

//            return CommandResult.Success();
//        }
//    }
//}
