using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using PlaceLibNet;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class GetPlacesNotGeoCodedService
    {
        private readonly Ilog _outputHandler;
        private readonly FtmPlaceCacheRepository _persistedCacheCacheRepository;
        private readonly IMapper _iMapper;

        public GetPlacesNotGeoCodedService(FtmPlaceCacheRepository persistedCacheCacheRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _outputHandler = outputHandlerp;
            _persistedCacheCacheRepository = persistedCacheCacheRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            return _iMapper.Map<IEnumerable<PlaceModel>>(_persistedCacheCacheRepository.GetUnknownPlacesIgnoreSearchedAlready().Take(amount));
            
        }
    }
}
