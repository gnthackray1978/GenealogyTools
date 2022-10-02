using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using FTMContextNet.Domain.Entities.NonPersistent.Locations;

namespace FTMContextNet.Application.Services
{
    public class GetUnknownPlacesSearchedAlreadyServices
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesSearchedAlreadyServices(PersistedCacheRepository persistedCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesSearchedAlreadyServices");


            IEnumerable<PlaceLookup> tp = _persistedCacheRepository.GetUnknownPlacesIgnoreSearchedAlready().Take(amount);


            return _iMapper.Map<IEnumerable<PlaceModel>>(tp);
        }
    }
}
