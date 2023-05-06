using LoggingLib;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Collections.Generic;
using FTMContextNet.Domain.Entities.Source;
using PlaceLibNet;
using PlaceLibNet.Model;

namespace FTMContextNet.Application.Services
{

    public class UpdatePlaceCache
    {
        private readonly Ilog _iLog;
        private readonly FtmPlaceCacheRepository _persistedCacheCacheRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceCache(FtmPlaceCacheRepository persistedCacheCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheCacheRepository = persistedCacheCacheRepository;
            _iMapper = iMapper;
        }

        public void Execute(List<Place> sourcePlaces)
        {
            _iLog.WriteLine("Executing UpdatePlaceCache");

            _iLog.WriteLine("Adding missing places to persisted cache");

            _persistedCacheCacheRepository.AddMissingPlaces(sourcePlaces);

            _iLog.WriteLine("Reset persisted cache entries where place has changed");

            _persistedCacheCacheRepository.ResetUpdatedPlaces(sourcePlaces);

            _iLog.WriteLine("Finished Adding and Updating Places");
        }
    }
}
