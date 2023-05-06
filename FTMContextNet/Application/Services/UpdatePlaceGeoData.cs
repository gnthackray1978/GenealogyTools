using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using FTMContextNet.Application.Models.Write;
using PlaceLibNet;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class UpdatePlaceGeoData
    {
        private readonly Ilog _iLog;
        private readonly FtmPlaceCacheRepository _persistedCacheCacheRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceGeoData(FtmPlaceCacheRepository persistedCacheCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheCacheRepository = persistedCacheCacheRepository;
            _iMapper = iMapper;
        }

        /// <summary>
        /// Updates place entry in cacheData.FTMPlaceCache with result we got back from google geocode.
        /// </summary>
        /// <returns></returns>
        public ServiceResult Execute(GeoCodeResultModel data)
        {
            _iLog.WriteLine("Updating cacheData.FTMPlaceCache with geocode result");

            _persistedCacheCacheRepository.SetPlaceGeoData(data.placeid,data.results);

            return ServiceResult.Success;
        }
    }
}
