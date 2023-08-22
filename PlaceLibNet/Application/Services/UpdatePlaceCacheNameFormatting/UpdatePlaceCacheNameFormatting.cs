using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LoggingLib;
using MediatR;
using MSG.CommonTypes;
using MSGIdent;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Commands;

namespace PlaceLibNet.Application.Services.UpdatePlaceCacheNameFormatting;

public class UpdatePlaceCacheNameFormatting : IRequestHandler<UpdatePlaceCacheCommand, CommandResult>
{
    private readonly Ilog _iLog;
    private readonly IPlaceRepository _placeRepository;
    private readonly IPlaceNameFormatter _placeNameFormatter;
    private readonly IAuth _auth;

    public UpdatePlaceCacheNameFormatting(IPlaceRepository placeRepository,
        IPlaceNameFormatter placeNameFormatter, Ilog iLog, IAuth auth)
    {
        _iLog = iLog;
        _placeRepository = placeRepository;
        _placeNameFormatter = placeNameFormatter;
        _auth = auth;
    }
     

    public async Task<CommandResult> Handle(UpdatePlaceCacheCommand request, CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }

        _iLog.WriteLine("Executing UpdatePlaceCacheNameFormatting");
 
        await Task.Run(() =>
        {
          foreach (var place in _placeRepository.GetCachedPlaces())
          {
              var newFormatting = _placeNameFormatter.Format(place.Place);

              if (newFormatting != place.PlaceFormatted)
              {
                  _placeRepository.UpdateCacheEntry(place.PlaceId, newFormatting);
              }
          }
        }, cancellationToken);

        _iLog.WriteLine("UpdatePlaceCacheNameFormatting ended");

        return CommandResult.Success();
    }
}