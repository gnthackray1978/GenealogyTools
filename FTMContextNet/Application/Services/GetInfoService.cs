using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;

namespace FTMContextNet.Application.Services
{
    public class GetInfoService
    {
        private readonly Ilog _iLog;
        private readonly IPersistedCacheRepository _persistedCacheRepository; 
        private readonly IMapper _iMapper;

        public GetInfoService(IPersistedCacheRepository persistedCacheRepository,  Ilog outputHandlerp, IMapper iMapper)
        {
            _iLog = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository; 
            _iMapper = iMapper;
        }

        public InfoModel Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var infoModal = _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo());
             

            return infoModal;
        }
    }
}
