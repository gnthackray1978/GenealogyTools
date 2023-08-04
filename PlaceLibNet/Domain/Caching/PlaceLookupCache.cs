using System.Collections.Generic;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching;

public class PlaceLookupCache : IPlaceLookupCache
{
    private readonly IPlaceNameFormatter _placeNameFormatter;
    private readonly IPlaceRepository _placeRepository;

    public List<PlaceLookup> PlaceLookups { get; set; }

    private bool isConstructed = false;

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
        isConstructed = true;
    }
    public PlaceLookupCache(IPlaceRepository placeRepository, IPlaceNameFormatter placeNameFormatter)
    {
        _placeRepository = placeRepository;
        _placeNameFormatter = placeNameFormatter;
    }

    public void Load()
    {
        if (!isConstructed)
        {
            PlaceLookups = _placeRepository.GetCachedPlaces();  
            PlaceLookups.Sort((s, y) => s.PlaceFormatted.CompareTo(y.PlaceFormatted));
            isConstructed = true;
        }
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