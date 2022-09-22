using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;

namespace FTMContextNet.Application.Services
{
    public class GetUnknownPlacesService
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesService(PersistedCacheRepository persistedCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesService");

            return _iMapper.Map<IEnumerable<PlaceModel>>(_persistedCacheRepository.GetUnknownPlaces().Take(amount));
        }
    }
}
