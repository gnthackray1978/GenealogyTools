using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;

namespace FTMContextNet.Application.Services
{
    internal class UpdatePlaceGeoCodeData
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceGeoCodeData(PersistedCacheRepository persistedCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public InfoModel Execute()
        {
            _iLog.WriteLine("Executing UpdatePlaceCache");

            return _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo());
        }
    }
}
