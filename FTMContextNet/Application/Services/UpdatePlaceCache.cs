using LoggingLib;
using AutoMapper;
using System.Collections.Generic;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Model;

namespace FTMContextNet.Application.Services
{

    public class UpdatePlaceCache
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public UpdatePlaceCache(PlaceRepository placeRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        public void Execute(List<Place> sourcePlaces)
        {
            _iLog.WriteLine("Executing UpdatePlaceCache");

            _iLog.WriteLine("Adding missing places to persisted cache");

            _placeRepository.AddMissingPlaces(sourcePlaces);

            _iLog.WriteLine("Reset persisted cache entries where place has changed");

            _placeRepository.ResetUpdatedPlaces(sourcePlaces);

            _iLog.WriteLine("Finished Adding and Updating Places");
        }
    }
}
