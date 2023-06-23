using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using LoggingLib;

namespace FTMContextNet.Application.Services
{
    public class GetGedFiles
    {
        private readonly Ilog _iLog;
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public GetGedFiles(IPersistedCacheRepository persistedCacheRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _iLog = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public List<GedFileModel> Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var gedFileModel = _iMapper.Map<List<GedFileModel>>(_persistedCacheRepository.GetImportData());


            return gedFileModel;
        }
    }
}
