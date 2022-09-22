using LoggingLib;
using FTMContextNet.Data.Repositories;
using AutoMapper;
using System.Collections.Generic;
using FTMContextNet.Domain.Entities.Source;

namespace FTMContextNet.Application.Services
{

    public class UpdatePlaceCache
    {
        private readonly Ilog _iLog;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceCache(PersistedCacheRepository persistedCacheRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _persistedCacheRepository = persistedCacheRepository;
            _iMapper = iMapper;
        }

        public void Execute(List<Place> sourcePlaces)
        {
            _iLog.WriteLine("Executing UpdatePlaceCache");

            _iLog.WriteLine("Adding missing places to persisted cache");

            _persistedCacheRepository.AddMissingPlaces(sourcePlaces);

            _iLog.WriteLine("Reset persisted cache entries where place has changed");

            _persistedCacheRepository.ResetUpdatedPlaces(sourcePlaces);

            _iLog.WriteLine("Finished Adding and Updating Places");
        }
    }
}
