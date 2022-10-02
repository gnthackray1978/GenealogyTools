using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using FTMContextNet.Application.Models.Write;
using FTMContextNet.Domain.Entities.NonPersistent.Locations;

namespace FTMContextNet.Application.Services
{
    public class UpdatePlaceGeoData
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceGeoData(PersistedCacheRepository persistedCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public ServiceResult Execute(GeoCodeResultModel data)
        {
            _iLog.WriteLine("Executing UpdatePlaceGeoData");

            _persistedCacheRepository.SetPlaceGeoData(data.placeid,data.results);

            return ServiceResult.Success;
        }
    }
}
