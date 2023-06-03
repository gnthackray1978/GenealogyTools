using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoggingLib;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;

namespace PlaceLibNet.Application.Services
{
    public class GetUnknownPlacesSearchedAlreadyServices
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesSearchedAlreadyServices(PlaceRepository placeRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesSearchedAlreadyServices");


            IEnumerable<PlaceLookup> tp = _placeRepository.GetUnknownPlacesIgnoreSearchedAlready().Take(amount);


            return _iMapper.Map<IEnumerable<PlaceModel>>(tp);
        }
    }
}
