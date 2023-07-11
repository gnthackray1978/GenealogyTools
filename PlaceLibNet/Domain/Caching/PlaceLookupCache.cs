using System.Collections.Generic;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching;

public class PlaceLookupCache : IPlaceLookupCache
{
    private readonly IPlaceNameFormatter _placeNameFormatter;
    public List<PlaceLookup> PlaceLookups { get; set; }

    /// <summary>
    /// PlaceCache table caching object.
    /// </summary>
    /// <param name="placeLookups">PLACEFORMATTED should be as the name suggests formatted!</param>
    public PlaceLookupCache(List<PlaceLookup> placeLookups, IPlaceNameFormatter placeNameFormatter)
    {
        //place lookup comes from the place cache table
        PlaceLookups = placeLookups;
        _placeNameFormatter = placeNameFormatter;
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