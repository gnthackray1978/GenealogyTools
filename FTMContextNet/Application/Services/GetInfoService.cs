using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class GetInfoService
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public GetInfoService(PersistedCacheRepository persistedCacheRepository, PlaceRepository placeRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _iLog = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        public InfoModel Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var infoModal = _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo());

            infoModal.Unsearched = _placeRepository.GetUnsearchedCount();
            infoModal.PlacesCount = _placeRepository.GetGeoCodeCacheSize();

            return infoModal;
        }
    }
}
