using System.Collections.Generic;
using MediatR;
using PlaceLibNet.Application.Models.Read;

namespace PlaceLibNet.Application.Services;

public class GetUnknownPlacesSearchedAlreadyQuery : IRequest<IEnumerable<PlaceModel>>
{
    public GetUnknownPlacesSearchedAlreadyQuery(int amount)
    {
        Amount = amount;
    }

    public int Amount { get; private set; }
}