using MediatR;
using MSG.CommonTypes;
using PlaceLibNet.Domain.Caching;

namespace PlaceLibNet.Domain.Commands;

public class UpdatePlaceGeoDataCommand : IRequest<CommandResult>
{
    public UpdatePlaceGeoDataCommand(int placeId, string results)
    {
        PlaceId = placeId;
        Results = results;
    }

    public int PlaceId { get; private set; }

    public string Results { get; private set; }
}