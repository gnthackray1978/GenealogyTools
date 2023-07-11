using System.Diagnostics;
using System.Linq;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Domain.Caching;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using LoggingLib;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;

namespace FTMContextNet.Application.Services;

public class CreatePersonLocationsInCache
{ 
    private readonly IPlaceRepository _placeRepository;

    private readonly IPersonPlaceCache _personPlaceCache;
    
    private readonly IPlaceLookupCache _placeLookupCache;

    private readonly IPlaceLibCoordCache _placeLibCoordCache;

    private readonly Ilog _logger;

    public CreatePersonLocationsInCache(IPlaceRepository placeRepository,
        IPlaceLibCoordCache placeLibCoordCache,
        IPersonPlaceCache personPlaceCache,
        IPlaceLookupCache placeLookupCache,
        Ilog logger)
    {
        _placeLibCoordCache = placeLibCoordCache;

        _personPlaceCache = personPlaceCache;

        _placeRepository = placeRepository;

        _placeLookupCache = placeLookupCache;

        _logger = logger;
    }

    public static PlaceCache Convert(PersonPlace pri, PlaceSearchCoordSubset pscs)
    {
        var placeCache = new PlaceCache()
        {
            Name = pri.Place,
            NameFormatted = pri.PlaceFormatted,
            JSONResult = "[]",
            County = pri.County,
        };

        if (pscs == null) return placeCache;

        placeCache.Lat = pscs.Lat;
        placeCache.Long = pscs.Long;
        placeCache.Src = "placelib";
        placeCache.BadData = true;


        return placeCache;
    }

    public void Execute()
    {
        var timer = new Stopwatch();
        timer.Start();
         
        var unencodedPlacesCount = _placeRepository.GetUnknownPlacesCount();
    
        _logger.WriteLine("Unencoded places in cache: " + unencodedPlacesCount);
        _logger.WriteLine("Person table locations count: " + _personPlaceCache.Count);
      
        //search the persons table place entries to see if they already exist in the place cache
        //if they don't exist then add a new placecache entry. 
        //look in the placelib to see if the new cache entry exists in there. if it does
        //use it to populate the lat and long of the cache entry.
        
        var newCacheEntries = _personPlaceCache
            .Where(w => !_placeLookupCache.Exists(w.PlaceFormatted))
            .Select(personPlace => 
                Convert(personPlace, _placeLibCoordCache.Search(personPlace.GetComponents(), personPlace.County))).ToList();

        _placeRepository.InsertIntoCache(newCacheEntries);
         
        timer.Stop();

          
        _logger.WriteLine("new records added: " + newCacheEntries.Count());

        _logger.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));

    }
    
}