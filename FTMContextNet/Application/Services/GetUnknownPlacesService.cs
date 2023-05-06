using LoggingLib;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using PlaceLibNet;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class GetUnknownPlacesService
    {
        private readonly Ilog _iLog;
        private readonly FtmPlaceCacheRepository _persistedCacheCacheRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesService(FtmPlaceCacheRepository persistedCacheCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheCacheRepository = persistedCacheCacheRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesService");

            return _iMapper.Map<IEnumerable<PlaceModel>>(_persistedCacheCacheRepository.GetUnknownPlaces().Take(amount));
        }
    }
}
