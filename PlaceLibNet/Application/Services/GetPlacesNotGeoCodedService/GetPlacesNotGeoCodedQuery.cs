using System.Collections.Generic;
using MediatR;
using PlaceLibNet.Application.Models.Read;

namespace PlaceLibNet.Application.Services.GetPlacesNotGeoCodedService;

public class GetPlacesNotGeoCodedQuery : IRequest<IEnumerable<PlaceModel>>
{
    public GetPlacesNotGeoCodedQuery(int amount)
    {
        Amount = amount;
    }

    public int Amount { get; private set; }
}