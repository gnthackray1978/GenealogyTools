using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LoggingLib;
using MediatR;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Application.Services.GetUnknownPlacesSearchedAlreadyService
{
    public class GetUnknownPlacesSearchedAlreadyServices :
        IRequestHandler<GetUnknownPlacesSearchedAlreadyQuery, IEnumerable<PlaceModel>>
    {
        private readonly Ilog _iLog;
        private readonly IPlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public GetUnknownPlacesSearchedAlreadyServices(IPlaceRepository placeRepository, Ilog iLog, IMapper iMapper)
        {
            _iLog = iLog;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }
         
        public async Task<IEnumerable<PlaceModel>> Handle(GetUnknownPlacesSearchedAlreadyQuery request, 
            CancellationToken cancellationToken)
        {
            _iLog.WriteCounter("Executing GetUnknownPlacesSearchedAlreadyServices");


            IEnumerable<PlaceLookup> tp = new List<PlaceLookup>();

            await Task.Run(() =>
            {
                tp = _placeRepository.GetUnknownPlacesIgnoreSearchedAlready()
                    .Take(request.Amount);
            }, cancellationToken);


            return _iMapper.Map<IEnumerable<PlaceModel>>(tp);
        }
    }
}
