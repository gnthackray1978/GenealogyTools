using System.Collections.Generic;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching;

public class PlaceLookupCache : IPlaceLookupCache
{
    private readonly IPlaceNameFormatter _placeNameFormatter;
    private readonly IPlaceRepository _placeRepository;

    public List<PlaceLookup> PlaceLookups { get; set; }
 
    public PlaceLookupCache(IPlaceRepository placeRepository, IPlaceNameFormatter placeNameFormatter)
    {
        _placeRepository = placeRepository;
        _placeNameFormatter = placeNameFormatter;
    }

    public void Load()
    {
        PlaceLookups = _placeRepository.GetCachedPlaces();  
        PlaceLookups.Sort((s, y) => s.PlaceFormatted.CompareTo(y.PlaceFormatted));
    }

    public bool Exists(string place)
    {
        place = _placeNameFormatter.Format(place);

        var loc = PlaceLookups.BinarySearch(new PlaceLookup { PlaceFormatted = place }
            , Comparer<PlaceLookup>
                .Create((s, y) => s.PlaceFormatted.CompareTo(y.PlaceFormatted)));

        return loc >= 0;
    }

    public PlaceLookup Search(string place)
    {
        place = _placeNameFormatter.Format(place);

        var loc = PlaceLookups.BinarySearch(new PlaceLookup { PlaceFormatted = place }
            , Comparer<PlaceLookup>
                .Create((s, y) => s.PlaceFormatted.CompareTo(y.PlaceFormatted)));

        return loc < 0 ? null : PlaceLookups[loc];
    }
}