using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace FTMContextNet.Application.Services
{
    public class GetPlacesNotGeoCodedService
    {
        private readonly Ilog _outputHandler;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public GetPlacesNotGeoCodedService(PersistedCacheRepository persistedCacheRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _outputHandler = outputHandlerp;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            return _iMapper.Map<IEnumerable<PlaceModel>>(_persistedCacheRepository.GetUnknownPlacesIgnoreSearchedAlready().Take(amount));
            //return _iMapper.Map<InfoModel>(_persistedCacheRepository.GetInfo());
        }
    }
}
