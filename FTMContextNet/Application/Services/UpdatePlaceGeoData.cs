using LoggingLib;
using AutoMapper;
using FTMContextNet.Application.Models.Write;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class UpdatePlaceGeoData
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceGeoData(PlaceRepository placeRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        /// <summary>
        /// Updates place entry in cacheData.FTMPlaceCache with result we got back from google geocode.
        /// </summary>
        /// <returns></returns>
        public ServiceResult Execute(GeoCodeResultModel data)
        {
            _iLog.WriteLine("Updating cacheData.FTMPlaceCache with geocode result");

            _placeRepository.SetPlaceGeoData(data.placeid,data.results);

            return ServiceResult.Success;
        }
    }
}
