using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FTMContextNet.Data.Repositories;
using LoggingLib; 
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;

namespace FTMContextNet.Application.Services;

public class UpdatePersonLocations
{
    private readonly PlaceRepository _placeRepository;

    private readonly PersistedCacheRepository _persistedCacheRepository;


    private Ilog _logger;

    public UpdatePersonLocations(PlaceRepository placeRepository, PersistedCacheRepository persistedCacheRepository, Ilog logger)
    {
        _persistedCacheRepository = persistedCacheRepository;

        _placeRepository = placeRepository;

        _logger = logger;
    }
    /// <summary>
    /// Update lat and long fields in the persons table.
    /// </summary>
    public void Execute()
    {
        _logger.WriteLine("UpdatePersonLocations started");

        var timer = new Stopwatch();
        timer.Start();
         
        var locations = _persistedCacheRepository.GetPersonMapLocations();
       

        var placeCache = new PlaceLookupCache(_placeRepository.GetCachedPlaces(),new PlaceNameFormatter());

        _logger.WriteLine(locations.Count + " locations");

        int count = 0;

        foreach (var location in locations)
        {
            var match = placeCache.Search(location.Location);

            if (match != null)
            {
                location.Lat = match.Lat;
                location.Lng = match.Lng;
            }

            match = placeCache.Search(location.AltLocation);

            if (match != null)
            {
                location.AltLat = match.Lat;
                location.AltLng = match.Lng;
            }

            if (location.Lat != "" || location.AltLat != "")
            {
                count++;
                _persistedCacheRepository.UpdatePersons(location.Id, location.Lat, location.Lng, location.AltLat,
                    location.AltLng);
            }
        }

        _logger.WriteLine("found " + count  + "locations");

        timer.Stop(); 
        _logger.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));

    }

}