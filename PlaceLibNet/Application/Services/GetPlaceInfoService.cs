using AutoMapper;
using LoggingLib;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Data.Repositories;

namespace PlaceLibNet.Application.Services
{
    public class GetPlaceInfoService
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;

        public GetPlaceInfoService(PlaceRepository placeRepository, Ilog outputHandlerp)
        {
            _iLog = outputHandlerp;
            _placeRepository = placeRepository;
        }

        public PlaceInfoModel Execute()
        {
            _iLog.WriteLine("Executing GetInfoService");

            var infoModal = new PlaceInfoModel
            {
                Unsearched = _placeRepository.GetUnsearchedCount(),
                PlacesCount = _placeRepository.GetGeoCodeCacheSize()
            };

            return infoModal;
        }
    }
}
