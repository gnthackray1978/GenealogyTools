using LoggingLib;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Data.Repositories;

namespace PlaceLibNet.Application.Services.GetPlacesNotGeoCodedService
{
    public class GetPlacesNotGeoCodedService : IRequestHandler<GetPlacesNotGeoCodedQuery, IEnumerable<PlaceModel>>
    {
        private readonly Ilog _outputHandler;
        private readonly IPlaceRepository _placeRepository;
        private readonly IMapper _iMapper;

        public GetPlacesNotGeoCodedService(IPlaceRepository placeRepository, Ilog outputHandler, IMapper iMapper)
        {
            _outputHandler = outputHandler;
            _placeRepository = placeRepository;
            _iMapper = iMapper;
        }
        
        public async Task<IEnumerable<PlaceModel>> Handle(GetPlacesNotGeoCodedQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<PlaceModel> tp = new List<PlaceModel>();

            await Task.Run(() =>
            {
                tp = _iMapper.Map<IEnumerable<PlaceModel>>(_placeRepository
                    .GetUnknownPlacesIgnoreSearchedAlready().Take(request.Amount));
            }, cancellationToken);

            return tp;
        }
    }
}
