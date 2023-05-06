using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using PlaceLibNet;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class GetInfoService
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly FtmPlaceCacheRepository _ftmMakerRepository;
        private readonly IMapper _iMapper;

        public GetInfoService(PersistedCacheRepository persistedCacheRepository, FtmPlaceCacheRepository ftmMakerRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _iLog = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository;
            _ftmMakerRepository = ftmMakerRepository;
            _iMapper = iMapper;
        }

        public InfoModel Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var infoModal = _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo());

            infoModal.Unsearched = _ftmMakerRepository.GetUnsearchedCount();
            infoModal.PlacesCount = _ftmMakerRepository.GetGeoCodeCacheSize();

            return infoModal;
        }
    }
}
