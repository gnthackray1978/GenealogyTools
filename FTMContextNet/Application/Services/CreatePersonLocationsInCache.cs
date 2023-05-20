using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FTMContextNet.Data.Repositories;
using LoggingLib;
using PlaceLib;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet.Application.Services;

public class CreatePersonLocationsInCache
{
    private readonly List<CountyDto> _counties;

    private readonly PlaceRepository _placeRepository;

    private readonly PersistedCacheRepository _persistedCacheRepository;


    private Ilog _logger;

    public CreatePersonLocationsInCache(PlaceRepository placeRepository, PersistedCacheRepository persistedCacheRepository, Ilog logger)
    {

        _persistedCacheRepository = persistedCacheRepository;

        _counties = placeRepository.GetCounties(true);

        _placeRepository = placeRepository;

        _logger = logger;
    }

    private string Capitalize(string str)
    {

        if (str.Length == 0) return "";


        if (str.Length == 1)
            return str[0].ToString().ToUpper();


        return char.ToUpper(str[0]).ToString() + str.Substring(1);
    }

    public void Execute()
    {
        var timer = new Stopwatch();
        timer.Start();

        var placeId = _placeRepository.GetNewFtmPlaceId();
        var id = _placeRepository.GetNewId();
        var counter = 1;

        var locations = _persistedCacheRepository.GetPersonLocations();

        var placeCache = _placeRepository.GetCachedPlaces();

        // at this point
        // the places list should be full of valid places
        // i.e. no empties and have 3 component locations

        _logger.WriteLine("Geocoded place cache: " + placeCache.Count);
        _logger.WriteLine("Person table places: " + locations.Count);

        var tp = locations.Where(w => w.PlaceFormatted.Contains("wooburn")).ToList();

        var tp1 = placeCache.Where(w => w.PlaceFormatted.Contains("wooburn")).ToList();

        //wooburn/buckingham/england
        //wooburn/buckingham/england

        var placeLibCounter = 0;
        var total = locations.Count;

        var tp22 = placeCache.FirstOrDefault(p => p.PlaceFormatted.Contains("wooburn/buckingham/england"));

        foreach (var location in locations)
        {

            if (location.PlaceFormatted.Contains("wooburn/buckingham/england"))
            {
                Debug.WriteLine("");
            }

            //is our place in the geocode cache?
            var match = placeCache.FirstOrDefault(p => p.PlaceFormatted.Contains(location.PlaceFormatted));

            if (match != null)
            {
                location.GoogleCacheId = match.PlaceId;
            }
            else
            {
                // we need to look in our other database
                // but first we need to set a county

                placeId++;
                id++;

                var placeLibEntryFound = false;

                var county = "";

                foreach (var countyDto in _counties.Where(countyDto => location.PlaceFormatted.Contains(countyDto.County)))
                {
                    county = countyDto.County;
                    break;
                }

                if (!string.IsNullOrEmpty(county))
                {
                    var placeParts = location.PlaceFormatted.Split("/").SkipLast(2);

                    location.County = county;

                    foreach (var placeName in placeParts)
                    {
                        var plibplace = _placeRepository.SearchPlaces(placeName, Capitalize(county), true);

                        if (plibplace == null) continue;

                        location.PlaceLibId = plibplace.Id;
                        location.Lat = plibplace.Lat;
                        location.Lon = plibplace.Long;
                        placeLibCounter++;
                        placeLibEntryFound = true;
                        break;

                    }
                }


                _placeRepository.InsertIntoCache(id,
                    placeId, location.Place, location.PlaceFormatted, "[]",
                    "", location.County, placeLibEntryFound, false,
                    location.Lat, location.Lon, placeLibEntryFound ? "placelib" : "");

            }


            _logger.ProgressUpdate(counter, total, "");
            counter++;

        }

        timer.Stop();


        _logger.WriteLine("places: " + locations.Count);
        _logger.WriteLine("glocated: " + locations.Count(w => w.GoogleCacheId != 0));
        _logger.WriteLine("plocated: " + placeLibCounter);
        _logger.WriteLine("invalid: " + locations.InvalidLocationsCount);
        _logger.WriteLine("dupes: " + locations.DuplicateLocationsCount);

        _logger.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));

    }

}