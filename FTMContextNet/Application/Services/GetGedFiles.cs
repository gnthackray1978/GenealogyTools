using System.Collections.Generic;
using AutoMapper;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories.GedImports;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class GetGedFiles
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

        public List<ImportModel> Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var gedFileModel = _iMapper.Map<List<ImportModel>>(_persistedImportCacheRepository.GetImportData());


            return gedFileModel;
        }
    }
}
