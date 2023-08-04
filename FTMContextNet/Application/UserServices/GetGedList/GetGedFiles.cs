using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories.GedImports;
using LoggingLib;
using MediatR;

namespace FTMContextNet.Application.UserServices.GetGedList
{
    public class GetGedFiles : IRequestHandler<GetGedFilesQuery, List<ImportModel>>
    {
        private readonly Ilog _iLog;
        private readonly IPersistedImportCacheRepository _persistedImportCacheRepository;
        private readonly IMapper _iMapper;

        public GetGedFiles(IPersistedImportCacheRepository persistedImportCacheRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _iLog = outputHandlerp;
            _persistedImportCacheRepository = persistedImportCacheRepository;
            _iMapper = iMapper;
        }
  
        public async Task<List<ImportModel>> Handle(GetGedFilesQuery request, 
                        CancellationToken cancellationToken)
        {
            _iLog.WriteLine("Executing GetInfoService");

            var gedFileModel = new List<ImportModel>();
            
            //todo do this in a better way
            await Task.Run(()=> 
                gedFileModel = _iMapper
                    .Map<List<ImportModel>>(_persistedImportCacheRepository.GetImportData()));


            return gedFileModel;
        }
    }
}
