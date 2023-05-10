using LoggingLib;
using FTMContextNet.Application.Models.Read;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class GetUnknownPlacesService
    {
        private readonly Ilog _iLog;
        private readonly PlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesService(PlaceRepository placeRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }

        public IEnumerable<PlaceModel> Execute(int amount)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesService");

            return _iMapper.Map<IEnumerable<PlaceModel>>(_placeRepository.GetUnknownPlaces().Take(amount));
        }
    }
}
