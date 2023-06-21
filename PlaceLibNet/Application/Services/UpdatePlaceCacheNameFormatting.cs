using LoggingLib;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;

namespace PlaceLibNet.Application.Services;

public class UpdatePlaceCacheNameFormatting
{
    private readonly Ilog _iLog;
    private readonly PlaceRepository _placeRepository;

    public UpdatePlaceCacheNameFormatting(PlaceRepository placeRepository, Ilog iLog)
    {
        _iLog = iLog;
        _placeRepository = placeRepository;
    }

    public void Execute()
    {
        _iLog.WriteLine("Executing UpdatePlaceCacheNameFormatting");

        var placeNameFormatter = new PlaceNameFormatter();
            
        foreach (var place in _placeRepository.GetCachedPlaces())
        {
            var newFormatting = placeNameFormatter.Format(place.Place);

            if (newFormatting != place.PlaceFormatted)
            {
                _placeRepository.UpdateCacheEntry(place.PlaceId,newFormatting);
            }
        }

        _iLog.WriteLine("UpdatePlaceCacheNameFormatting ended");
    }
}