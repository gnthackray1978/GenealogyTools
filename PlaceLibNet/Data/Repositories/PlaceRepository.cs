using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConfigHelper;
using GoogleMapsHelpers;
using LoggingLib;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;
using PlaceLibNet.Domain.Entities.Persistent;

namespace PlaceLibNet.Data.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly IPlacesContext _placesContext;
        private readonly Ilog _iLog;
        
        public PlaceRepository(IPlacesContext placesContext, Ilog iLog)
        {
            _placesContext = placesContext;
            _iLog = iLog;
        }
        
        public List<CountyDto> GetCounties(bool toLower = false)
        {
           // using var placecontext = new PlacesContext(new MSGConfigHelper());

            var counties = _placesContext.Places.Select(s => new CountyDto()
            {
                County = toLower ? s.Ctyhistnm.ToLower() : s.Ctyhistnm,
                Country = s.Ctry15nm

            }).Distinct().Where(w => w.County.Trim() != "").ToList();

            return counties;
        }
        
        public List<PlaceSearchCoordSubset> GetPlaceLibCoords()
        {
            var result = _placesContext
                .Places
                .Where(w => w.Ctry15nm == "England")
                .OrderBy(o => o.Place15cd)
                .Select(s => new PlaceSearchCoordSubset()
                {
                    Ctyhistnm =  s.Ctyhistnm, 
                    Id = s.Id,
                    Lat = s.Lat, 
                    Long = s.Long,
                    Placesort = s.Placesort
                }).ToList();
            
            return result;
        }

        public int GetGeoCodeCacheSize()
        { 
            return _placesContext.PlaceCache.Count(); 
        }

        public int GetUnsearchedCount()
        {
            return _placesContext.PlaceCache.Count(w => !w.Searched);
        }
        
        public void InsertIntoCache(IEnumerable<PlaceCache> placeCaches)
        {
            var id= _placesContext.PlaceCache.Max(m=>m.Id) + 1;

            foreach (PlaceCache pc in placeCaches)
            {
                _placesContext.InsertPlaceCache(id,
                    pc.Name, pc.NameFormatted, pc.JSONResult, 
                    pc.Country, pc.County, 
                    pc.Searched, pc.BadData, 
                    pc.Lat, pc.Long, pc.Src);

                id++;
            }
            
        }

        public void UpdateCacheEntry(int id, string name)
        {
            _placesContext.UpdateFormattedName(id,name);
        }

        public void SetPlaceGeoData(int id, string results)
        {
            try
            {
                _placesContext.UpdateJSONCacheResult(id, results);

                Debug.WriteLine("ID : " + id);
            }
            catch (Exception e)
            {
                Debug.WriteLine("failed: " + e.Message);
            }

        }
        
        /// <summary>
        /// Deserialise JSON result of google geolocate and use it to 
        /// fill out place cache fields
        /// </summary>
        public void SetGeolocatedResult()
        {
            
            var placeList = _placesContext
                .PlaceCache.Select(s=>new 
                {
                    s.Id, 
                    parsedLocation = Location.GetLocationInfo(s.JSONResult),
                    s.County,
                    s.Src,
                    s.Lat
                })
                .Where(w => w.Src != "placelib" && w.Lat=="").ToList();

            _iLog.WriteLine(placeList.Count + " places with unset lat and longs");

            foreach (var place in placeList
                         .Where(w=>w.parsedLocation.IsValid))
            {
                var updatedPlace = new PlaceCache
                {
                    Id = place.Id,
                    Country = place.parsedLocation.State,
                    County = (!string.IsNullOrEmpty(place.parsedLocation.County) && place.County == "") ? place.parsedLocation.County : place.County,
                    Lat = place.parsedLocation.lat.ToString(),
                    Long = place.parsedLocation.lng.ToString(),
                    Src = "google"
                };
                 
                _placesContext.UpdatePlaceCacheLatLong(updatedPlace);
            }

            foreach (var place in placeList
                         .Where(w => !w.parsedLocation.IsValid))
            {
                _placesContext.UpdateBadData(place.Id,true);
            }
        }
        
        public void SetCounties()
        {
           
            var places = _placesContext
                .PlaceCache
                .Where(w => w.Src != "placelib" 
                            && w.County == ""
                            && w.Country == "England"
                            && !w.BadData).ToList();

            var idx = 0;
            var total = places.Count;

            // create a cache object here and then search it.
            
            var cs = new CountySearch(_placesContext.Places.Where(w => w.Ctry15nm == "England").ToList(), new PlaceNameFormatter() );
            
            foreach (var place in places)
            {
                var locationInfo = Location.GetLocationInfo(place.JSONResult);

                string county = EnglishHistoricCounties
                    .Match(new List<string>{locationInfo.PostalTown, locationInfo.Political});
                
                if (string.IsNullOrEmpty(county))
                    county = cs.Search(locationInfo.PostalTown);

                if (string.IsNullOrEmpty(county))
                    county = cs.Search(locationInfo.Political);

                if (county != "")
                {
                    _placesContext.UpdateCounty(place.Id,county);
                    _iLog.ProgressUpdate(idx,total,"county updated");
                }

                idx++;
            } 
        }

        public List<PlaceLookup> GetCachedPlaces()
        {
            var places = _placesContext.PlaceCache
                .Where(w => w.BadData == false)
                .Select(s => new PlaceLookup()
                {
                    PlaceId = s.Id, 
                    PlaceFormatted = s.NameFormatted, 
                    Place = s.Name, 
                    Lat = s.Lat,
                    Lng = s.Long
                })
                .ToList();
            
            return places;
        }

        /// <summary>
        /// count of un geolocated cached places
        /// </summary>
        /// <returns></returns>
        public int GetUnknownPlacesCount()
        {
            return _placesContext.PlaceCache
                .Count(w => (string.IsNullOrEmpty(w.Lat)) && w.BadData == false);
        }

        public List<PlaceLookup> GetUnknownPlacesIgnoreSearchedAlready()
        {
            var places = _placesContext.PlaceCache.Where(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]")
                                                                     && !w.Searched && !w.BadData)
                .Select(s => new PlaceLookup() { PlaceId = s.Id, PlaceFormatted = s.NameFormatted })
                .ToList();

            foreach (var f in places)
            {
                f.PlaceFormatted = f.PlaceFormatted.Replace("//", "").Replace("|", "");
            }

            return places;
        }
    }
}
