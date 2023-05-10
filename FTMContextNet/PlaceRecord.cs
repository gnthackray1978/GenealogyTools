using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FTMContextNet.Data.Repositories;
using LoggingLib;
using PlaceLib;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;

namespace FTMContextNet;

public class PlaceRecord : IEnumerable<PlaceRecordItem>
{
    private List<CountyDto> _counties;
    public List<PlaceRecordItem> Places { get; set; } 

    private PlaceRepository _placeRepository;

    private PersistedCacheRepository _persistedCacheRepository;

    private int _invalidPlaces = 0;

    private int _duplicateCount = 0;

    private Ilog _logger;

    public PlaceRecord(PlaceRepository placeRepository, PersistedCacheRepository persistedCacheRepository, Ilog logger)
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

    public void Process()
    {
        var timer = new Stopwatch();
        timer.Start();

        var placeId = _placeRepository.GetNewFtmPlaceId();
        var id = _placeRepository.GetNewId();

        PreparePlaceData(_persistedCacheRepository.GetPlaces().ToArray());


        var placeCache = _placeRepository.GetCachedPlaces();

        // at this point
        // the places list should be full of valid places
        // i.e. no empties and have 3 component locations

        _logger.WriteLine("geocoded place cache: " + placeCache.Count);
        _logger.WriteLine("FTMPerson table places: " + Places.Count);

        var placeLibCounter = 0;

        foreach (var place in Places)
        {
            //is our place in the geocode cache?
            var match = placeCache.FirstOrDefault(p => p.place.Contains(place.Place));

            if (match != null)
            {
                place.GoogleCacheId = match.placeid;
            }
            else
            {
                // we need to look in our other database
                // but first we need to set a county

                placeId++;
                id ++;

                var placeLibEntryFound = false;

                var county = "";

                foreach (var countyDto in _counties.Where(countyDto => place.Place.Contains(countyDto.County)))
                {
                    county = countyDto.County;   
                    break;
                }

                if (!string.IsNullOrEmpty(county))
                {
                    var placeParts = place.Place.Split("/").SkipLast(2);
                  
                    place.County = county;

                    foreach (var placeName in placeParts)
                    {
                        var plibplace = _placeRepository.SearchPlaces(placeName, Capitalize(county));

                        if (plibplace!= null && plibplace.Lat != "" && plibplace.Long != "")
                        {
                            place.PlaceLibId = plibplace.Id;
                            place.Lat = plibplace.Lat;
                            place.Lon = plibplace.Long;
                            placeLibCounter++;
                            placeLibEntryFound = true;
                            break;
                        }

                    }
                }

           
                _placeRepository.InsertFtmPlaceCache(id,
                    placeId, place.PlaceRaw, place.Place, "[]",
                    "", place.County, placeLibEntryFound, false, 
                    place.Lat, place.Lon, placeLibEntryFound ? "placelib":"");
                
            }


        }
        
        timer.Stop();
         
        
        _logger.WriteLine("places: " + Places.Count);
        _logger.WriteLine("glocated: " + Places.Count(w => w.GoogleCacheId != 0));
        _logger.WriteLine("plocated: " + placeLibCounter);
        _logger.WriteLine("invalid: " + _invalidPlaces);
        _logger.WriteLine("dupes: " + _duplicateCount);

        _logger.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));

    }
    
    public void PreparePlaceData(string[] places)
    {
        foreach (var place in places)
        {
            string formattedPlace = FormatPlace(place);

            if (isValid(formattedPlace))
            {
                AddPlace(formattedPlace, place);
            }
        }
    }

    private string FormatPlace(string place)
    {
        return place.ToLower().Replace(" ", "").Replace(",", "/").Replace("//", "/");
    }

    /// <summary>
    /// Valid when has 3 components AND
    /// is in England or Wales
    /// </summary>
    /// <param name="place"></param>
    /// <returns></returns>
    private bool isValid(string place)
    {
        var count = place.Count(c => c == '/');
       
        if (place.Contains("england") || place.Contains("wales"))
        {
            if (count >1)
            {
                return true;
            }
        }

        _invalidPlaces++;


        return false;
    }

    private void AddPlace(string place, string rawPlace)
    {
        Places ??= new List<PlaceRecordItem>();

        var placeLH = new PlaceRecordItem()
        {
            PlaceRaw = rawPlace,
            Place = place
        };

        if (Places.All(a => a.Place != placeLH.Place))
            Places.Add(placeLH);
        else
            _duplicateCount++;
    }

    public IEnumerator<PlaceRecordItem> GetEnumerator()
    {
        return this.Places.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}