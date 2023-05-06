using LoggingLib;
using FTMContextNet.Application.Models.Read;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using PlaceLibNet;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;
using PlaceLibNet.Model;

namespace FTMContextNet.Application.Services
{
    public class GetUnknownPlacesSearchedAlreadyServices
    {
        private readonly Ilog _iLog;
        private readonly FtmPlaceCacheRepository _persistedCacheCacheRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesSearchedAlreadyServices(FtmPlaceCacheRepository persistedCacheCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheCacheRepository = persistedCacheCacheRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesSearchedAlreadyServices");


            IEnumerable<PlaceLookup> tp = _persistedCacheCacheRepository.GetUnknownPlacesIgnoreSearchedAlready().Take(amount);


            return _iMapper.Map<IEnumerable<PlaceModel>>(tp);
        }
    }
}
