using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using FTMContextNet.Domain.Auth;
using Newtonsoft.Json.Serialization;

namespace FTMContextNet.Application.Services
{
    public class GetInfoService
    {
        private readonly Ilog _iLog;
        private readonly IPersistedCacheRepository _persistedCacheRepository; 
        private readonly IMapper _iMapper;
        private readonly IAuth _auth;

        public GetInfoService(IPersistedCacheRepository persistedCacheRepository,
            Ilog outputHandlerp, IMapper iMapper, IAuth auth)
        {
            _iLog = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository; 
            _iMapper = iMapper;
            _auth = auth;
        }

        public InfoModel Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var infoModal = _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo(_auth.GetUser()));
             

            return infoModal;
        }
    }
}
