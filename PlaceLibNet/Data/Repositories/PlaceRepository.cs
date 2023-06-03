using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ConfigHelper;
using GoogleMapsHelpers;
using LoggingLib;
using PlaceLib;
using PlaceLib.Model;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Domain;
using PlaceLibNet.Model;

namespace PlaceLibNet.Data.Repositories
{
    public class PlaceRepository
    {
        private readonly PlacesContext _placesContext;
        private readonly Ilog _iLog;
        
        public PlaceRepository(PlacesContext placesContext, Ilog iLog)
        {
            _placesContext = placesContext;
            _iLog = iLog;
        }


        public List<PlaceDto> GetPlaces()
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            var places = placecontext.Places.Where(w => w.Ctyhistnm != "").Select(s => new PlaceDto()
            {
                County = s.Ctyhistnm.ToLower(),
                Country = s.Ctry15nm,
                Place = s.Place15nm.ToLower()
            }).Distinct().ToList();

            return places;
        }

        public List<CountyDto> GetCounties(bool originalCase = false)
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            var counties = placecontext.Places.Select(s => new CountyDto()
            {
                County = originalCase ? s.Ctyhistnm.ToLower() : s.Ctyhistnm,
                Country = s.Ctry15nm

            }).Distinct().Where(w => w.County.Trim() != "").ToList();

            return counties;
        }

        public string SearchPlacesDBForCounty(string searchString)
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            string county = "";

            var placedbResult = placecontext.Places.FirstOrDefault(w => w.Place15nm == searchString);

            if (placedbResult != null)
            {
                county = placedbResult.Ctyhistnm;
            }
            else
            {
                var stripped = searchString.ToLower();
                stripped = Regex.Replace(stripped, " ", "");

                placedbResult = placecontext.Places.FirstOrDefault(w => w.Placesort == stripped);

                if (placedbResult != null)
                {
                    county = placedbResult.Ctyhistnm;
                }
            }

            return county;
        }

        public Places SearchPlaces(string searchString, string county, bool requiredValidLatLong = false)
        {
        //    using var placecontext = new PlacesContext(new MSGConfigHelper());



           // Debug.WriteLine(searchString + "/" + county);
            Places placedbResult = _placesContext
                .Places
                .Where(w => w.Placesort == searchString && w.Ctyhistnm == county)
                .OrderBy(o => o.Place15cd)
                .FirstOrDefault();
            
            if(!requiredValidLatLong)
                return placedbResult;

            if (placedbResult != null && placedbResult.Lat != "" && placedbResult.Long != "")
            {
                return placedbResult;
            }

            return null;
        }

        public int GetGeoCodeCacheSize()
        { 
            return _placesContext.PlaceCache.Count(); 
        }

        public int GetUnsearchedCount()
        {
            return _placesContext.PlaceCache.Count(w => !w.Searched);
        }

        public int GetNewId()
        {
            // var placeId = FTMPlaceCache.Max(m => m.FTMPlaceId);
             var id = _placesContext.PlaceCache.Max(m => m.Id);
             return id+1;

        }

        public int GetNewFtmPlaceId()
        { 
            var placeId = _placesContext.PlaceCache.Max(m => m.AltId);
           
            return placeId + 1;

        }

        public void InsertIntoCache(int id, int placeId,
            string name,
            string nameFormatted,
            string jsonResult,
            string country,
            string county,
            bool searched,
            bool badData,
            string lat,
            string lon,
            string src)
        {
            _placesContext.InsertPlaceCache(id,placeId,
                name,nameFormatted,jsonResult,country,county,searched,badData,lat,lon, src);
        }

        public List<PlaceCache> GetUnsetCountiesAndCountrys()
        {

            var unsetCountiesCount = _placesContext.PlaceCache.Where(w => (w.County == "" || w.Country == "") && w.JSONResult != null);


            return unsetCountiesCount.ToList();
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

        public void UpdateLatLons()
        {
            foreach (var fc in _placesContext.PlaceCache.ToList())
            {
                var locationLat = Location.GetLocation(fc.JSONResult).lat;
                var locationLong = Location.GetLocation(fc.JSONResult).lng;

                _placesContext.UpdatePlaceCacheLatLong(fc.Id,locationLat.ToString(),locationLong.ToString());
            }
        }


        public void FormatNames()
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            foreach (var fc in _placesContext.PlaceCache.ToList())
            {
                
                string name = DeleteNonAlphaNumericExceptSlash(fc.NameFormatted);

               // name = name.Replace(',', '/');
              //  name = name.Replace('|', ' ').Trim();

                name = ReplaceSlashesWithSingleSlash(name);
                 

                if(fc.NameFormatted!=name)
                    placecontext.UpdateFormattedName(fc.Id, name);
            }
        }

   
        public static string ReplaceSlashesWithSingleSlash(string input)
        {
            // Create a regular expression that matches a single slash or any number of whitespace characters.
            var regex = new Regex(@"(\s+/\s+|\s+/|/\s+)");

            // Replace all matches of the regular expression with a single slash.
            return regex.Replace(input, "/");
        }
        public static string DeleteNonAlphaNumericExceptSlash(string input)
        {
            // Create a regular expression that matches a single slash or any number of whitespace characters.
            var regex = new Regex(@"[^a-zA-Z\d\s/]");

            // Replace all matches of the regular expression with a single slash.
            return regex.Replace(input, "");
        }

        public void SaveChanges()
        {
            _placesContext.SaveChanges();
        }


        public int GetUnsetUkCountiesCount()
        {
            var places = _placesContext.PlaceCache.Where(w => w.County == "" || w.Country == "").ToList();

            return places.Count;
        }
        public int GetUnsetJsonResultCount()
        {
            var places = _placesContext.PlaceCache.Where(w => w.JSONResult == null).ToList();

            return places.Count;
        }

        /// <summary>
        /// sets county and country values in ftmcache. 
        /// </summary>
        public void SetGeolocatedResult()
        {
            var places = _placesContext
                .PlaceCache
                .Where(w => w.Src != "placelib").ToList();

            
            foreach (var place in places)
            {
                var locationInfo = Location.GetLocationInfo(place.JSONResult);

                if (!locationInfo.IsValid)
                {
                    place.BadData = true;
                    continue;
                }

                place.Country = locationInfo.Country;

                // dont overwrite this , if it has a value
                if (locationInfo.IsValidCounty && place.County == "")
                    place.County = locationInfo.County;

                place.Lat = Location.GetLocation(place.JSONResult).lat.ToString();
                place.Long = Location.GetLocation(place.JSONResult).lng.ToString();

                if (place.Lat == "0" && place.Long == "0")
                    place.BadData = true;

                place.Src = "google";
            }


            _placesContext.SaveChanges();
        }

        public void SetCounties()
        {
           
            var places = _placesContext
                .PlaceCache
                .Where(w => w.Src != "placelib" && w.County == "").ToList();

            foreach (var place in places)
            {
                var locationInfo = Location.GetLocationInfo(place.JSONResult);
                    
                if(locationInfo.PostalTown != "")
                {
                    place.County = SearchPlacesDBForCounty(locationInfo.PostalTown);
                }

                if (place.County == "" && locationInfo.Political != "")
                {
                    if (EnglishHistoricCounties.Get.Contains(locationInfo.Political))
                    {
                        place.County = locationInfo.Political;
                    }

                    if (place.County == "")
                        place.County = SearchPlacesDBForCounty(locationInfo.Political);
                }
            }
            
            SaveChanges();
        }


        private static (List<Place> missingPlaces, List<Place> updatedPlaces)
    CheckForUpdates(PlacesContext context, List<Place> sourcePlaces, Ilog iLog, bool showInfo = false)
        {
            iLog.WriteLine("Finding missing and updated places");

            var cacheDictionary = new Dictionary<int, string>();

            // for performance reasons create a place cache of the existing records
            foreach (var p in context.PlaceCache)
            {
                if (!cacheDictionary.ContainsKey(p.AltId))
                    cacheDictionary.Add(p.AltId, p.Name);
            }

            List<Place> missingPlaces = new List<Place>();
            List<Place> updatedPlaces = new List<Place>();

            //loops through new records and check if they are in the cache
            //if not then its a missing place so add them to missing place list
            foreach (var p in sourcePlaces)
            {
                if (!cacheDictionary.ContainsKey(p.Id))
                {
                    missingPlaces.Add(p);
                }
                else
                {
                    //ok so the id is in the cache 
                    //is the name the same?
                    if (cacheDictionary[p.Id] != p.Name)
                    {
                        //if not
                        updatedPlaces.Add(p);
                    }
                }
            }

            if (showInfo)
            {
                iLog.WriteLine(updatedPlaces.Count + " updated places ");

                foreach (var m in updatedPlaces)
                {
                    iLog.WriteLine("Missing Place : " + m.Name);
                }

                iLog.WriteLine(missingPlaces.Count + " missing places ");

                foreach (var m in missingPlaces)
                {
                    iLog.WriteLine("Missing Place : " + m.Name);
                }
            }
            return (missingPlaces, updatedPlaces);
        }


        /// <summary>
        /// Looks through the cacheData.ftmplacecache table and if location is not in it
        /// then add it.
        /// Also looks to see if place has changed.
        /// The ids in the ftmplacecache.FTMPlaceId and dna_match_file place table should be aligned. If
        /// the place changes then it needs updating.
        /// </summary>
        /// <param name="sourcePlaces">list of all places from the dna_match_file DB</param>
        public void AddMissingPlaces(List<Place> sourcePlaces)
        {
            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(this._placesContext, sourcePlaces, this._iLog);

            _iLog.WriteLine("Adding " + data.missingPlaces.Count + " missing places");

            if (data.missingPlaces.Count > 0)
            {
                int newId = this._placesContext.PlaceCache.Count() + 1;

                foreach (var p in data.missingPlaces)
                {
                    this._placesContext.PlaceCache.Add(new PlaceCache()
                    {
                        Id = newId,
                        Name = p.Name,
                        JSONResult = null,
                        NameFormatted = Location.FormatPlace(p.Name),
                        AltId = p.Id,
                        Searched = false,
                        BadData = false
                    });

                    newId++;
                    this._placesContext.SaveChanges();
                }
            }

        }

        /// <summary>
        /// finds place ids in the ftmcache and sets them to null when
        /// the place ids have been changed.
        /// </summary>
        public void ResetUpdatedPlaces(List<Place> sourcePlaces)
        {

            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(this._placesContext, sourcePlaces, this._iLog);

            _iLog.WriteLine(data.updatedPlaces.Count + " updated places ");


            if (data.updatedPlaces.Count > 0)
            {
                foreach (var p in data.updatedPlaces)
                {
                    var cachedValue = _placesContext.PlaceCache.FirstOrDefault(f => f.AltId == p.Id);

                    if (cachedValue != null)
                    {
                        cachedValue.Name = p.Name;
                        cachedValue.JSONResult = null;
                        cachedValue.Country = "";
                        cachedValue.County = "";
                        cachedValue.NameFormatted = Location.FormatPlace(p.Name);
                        cachedValue.Searched = false;
                    }

                    _placesContext.SaveChanges();
                }
            }

        }



        /// <summary>
        /// return list of all  places in place cache
        /// </summary>
        /// <returns></returns>
        public List<PlaceLookup> GetCachedPlaces()
        {
            var places = _placesContext.PlaceCache
                .Where(w => w.BadData == false)
                .Select(s => new PlaceLookup() { PlaceId = s.AltId, PlaceFormatted = s.NameFormatted, Place = s.Name })
                .ToList();

            foreach (var f in places)
            {
                f.Place = f.Place.ToLower().Replace(" ", "").Replace(",","/").Replace("//", "/");
            }

            return places;
        }

        /// <summary>
        /// return list of entries in decrypt place cache that don't haven't been geolocated
        /// </summary>
        /// <returns></returns>
        public List<PlaceLookup> GetUnknownPlaces()
        {
            var places = _placesContext.PlaceCache
                .Where(w => (string.IsNullOrEmpty(w.Lat)) 
                            && w.BadData == false)
                .Select(s => new PlaceLookup() { PlaceId = s.Id, PlaceFormatted = s.NameFormatted })
                .ToList();

            //foreach (var f in places)
            //{
            //    f.PlaceFormatted = f.PlaceFormatted.Replace("//", "").Replace("|", "");
            //}

            return places;
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
