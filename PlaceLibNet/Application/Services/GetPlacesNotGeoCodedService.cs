using LoggingLib;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class GetPlacesNotGeoCodedService
    {
        private readonly Ilog _outputHandler;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public GetPlacesNotGeoCodedService(PlaceRepository placeRepository, Ilog outputHandlerp, IMapper iMapper)
        {
            _outputHandler = outputHandlerp;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            return _iMapper.Map<IEnumerable<PlaceModel>>(_placeRepository.GetUnknownPlacesIgnoreSearchedAlready().Take(amount));
            
        }
    }
}
