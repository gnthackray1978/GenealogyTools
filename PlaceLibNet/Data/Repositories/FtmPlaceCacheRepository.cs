using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GoogleMapsHelpers;
using LoggingLib;
using PlaceLib.Model;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Domain;
using PlaceLibNet.Model;

namespace PlaceLibNet.Data.Repositories
{
    public class FtmPlaceCacheRepository
    {
        private readonly PlacesContext _persistedCacheContext;
        private readonly Ilog _iLog;
        
        public FtmPlaceCacheRepository(PlacesContext persistedCacheContext, Ilog iLog)
        {
            _persistedCacheContext = persistedCacheContext;
            _iLog = iLog;
        }

        public int GetGeoCodeCacheSize()
        { 
            return _persistedCacheContext.FTMPlaceCache.Count(); 
        }

        public int GetUnsearchedCount()
        {
            return _persistedCacheContext.FTMPlaceCache.Count(w => !w.Searched);
        }

        public int GetNewId()
        {
            // var placeId = FTMPlaceCache.Max(m => m.FTMPlaceId);
             var id = _persistedCacheContext.FTMPlaceCache.Max(m => m.Id);
             return id+1;

        }

        public int GetNewFtmPlaceId()
        { 
            var placeId = _persistedCacheContext.FTMPlaceCache.Max(m => m.FTMPlaceId);
           
            return placeId + 1;

        }

        public void InsertFtmPlaceCache(int id, int placeId,
            string ftmOrginalName,
            string ftmOrginalNameFormatted,
            string jsonResult,
            string country,
            string county,
            bool searched,
            bool badData,
            string lat,
            string lon,
            string src)
        {
            _persistedCacheContext.InsertFTMPlaceCache(id,placeId,
                ftmOrginalName,ftmOrginalNameFormatted,jsonResult,country,county,searched,badData,lat,lon, src);
        }

        public List<FtmPlaceCache> GetUnsetCountiesAndCountrys()
        {

            var unsetCountiesCount = _persistedCacheContext.FTMPlaceCache.Where(w => (w.County == "" || w.Country == "") && w.JSONResult != null);


            return unsetCountiesCount.ToList();
        }

      
        public void SetPlaceGeoData(int placeId, string results)
        {
            try
            {
                _persistedCacheContext.UpdateFTMPlaceCache(placeId, results);

                Debug.WriteLine("ID : " + placeId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("failed: " + e.Message);
            }

        }

        public void UpdateLatLons()
        {
            foreach (var fc in _persistedCacheContext.FTMPlaceCache.ToList())
            {
                var locationLat = Location.GetLocation(fc.JSONResult).lat;
                var locationLong = Location.GetLocation(fc.JSONResult).lng;

                _persistedCacheContext.UpdateFTMPlaceCacheLatLong(fc.FTMPlaceId,locationLat.ToString(),locationLong.ToString());
            }
        }

        public void SaveChanges()
        {
            _persistedCacheContext.SaveChanges();
        }


        public int GetUnsetUkCountiesCount()
        {
            var places = _persistedCacheContext.FTMPlaceCache.Where(w => w.County == "" || w.Country == "").ToList();

            return places.Count;
        }
        public int GetUnsetJsonResultCount()
        {
            var places = _persistedCacheContext.FTMPlaceCache.Where(w => w.JSONResult == null).ToList();

            return places.Count;
        }

        /// <summary>
        /// sets county and country values in ftmcache. 
        /// </summary>
        public List<ExtendedPlace> GetUnsetUkCounties()
        {
            List<FtmPlaceCache> places = _persistedCacheContext.FTMPlaceCache.Where(w => w.County == "" || w.Country == "").ToList();

            var results = new List<ExtendedPlace>();

            foreach (var place in places)
            {
                var locationInfo = Location.GetLocationInfo(place.JSONResult);

                if (!locationInfo.IsValid)
                    continue;

                place.Country = locationInfo.Country;
                place.County = "";

                results.Add(new ExtendedPlace()
                {
                    Place = place,
                    LocationInfo = locationInfo
                });
            }

            return results;


            //_iLog.WriteLine("FTMPlaceCache has ~" + foreignCounties + " foreign records");

            //_iLog.WriteLine("FTMPlaceCache has ~" + unsetCountiesCount + " unset records");
        }




        private static (List<Place> missingPlaces, List<Place> updatedPlaces)
    CheckForUpdates(PlacesContext context, List<Place> sourcePlaces, Ilog iLog, bool showInfo = false)
        {
            iLog.WriteLine("Finding missing and updated places");

            var cacheDictionary = new Dictionary<int, string>();

            // for performance reasons create a place cache of the existing records
            foreach (var p in context.FTMPlaceCache)
            {
                if (!cacheDictionary.ContainsKey(p.FTMPlaceId))
                    cacheDictionary.Add(p.FTMPlaceId, p.FTMOrginalName);
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
            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(this._persistedCacheContext, sourcePlaces, this._iLog);

            _iLog.WriteLine("Adding " + data.missingPlaces.Count + " missing places");

            if (data.missingPlaces.Count > 0)
            {
                int newId = this._persistedCacheContext.FTMPlaceCache.Count() + 1;

                foreach (var p in data.missingPlaces)
                {
                    this._persistedCacheContext.FTMPlaceCache.Add(new FtmPlaceCache()
                    {
                        Id = newId,
                        FTMOrginalName = p.Name,
                        JSONResult = null,
                        FTMOrginalNameFormatted = Location.FormatPlace(p.Name),
                        FTMPlaceId = p.Id,
                        Searched = false,
                        BadData = false
                    });

                    newId++;
                    this._persistedCacheContext.SaveChanges();
                }
            }

        }

        /// <summary>
        /// finds place ids in the ftmcache and sets them to null when
        /// the place ids have been changed.
        /// </summary>
        public void ResetUpdatedPlaces(List<Place> sourcePlaces)
        {

            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(this._persistedCacheContext, sourcePlaces, this._iLog);

            _iLog.WriteLine(data.updatedPlaces.Count + " updated places ");


            if (data.updatedPlaces.Count > 0)
            {
                foreach (var p in data.updatedPlaces)
                {
                    var cachedValue = _persistedCacheContext.FTMPlaceCache.FirstOrDefault(f => f.FTMPlaceId == p.Id);

                    if (cachedValue != null)
                    {
                        cachedValue.FTMOrginalName = p.Name;
                        cachedValue.JSONResult = null;
                        cachedValue.Country = "";
                        cachedValue.County = "";
                        cachedValue.FTMOrginalNameFormatted = Location.FormatPlace(p.Name);
                        cachedValue.Searched = false;
                    }

                    _persistedCacheContext.SaveChanges();
                }
            }

        }



        /// <summary>
        /// return list of all  places in place cache
        /// </summary>
        /// <returns></returns>
        public List<PlaceLookup> GetCachedPlaces()
        {
            var places = _persistedCacheContext.FTMPlaceCache
                .Where(w => w.BadData == false)
                .Select(s => new PlaceLookup() { placeid = s.FTMPlaceId, placeformatted = s.FTMOrginalNameFormatted, place = s.FTMOrginalName })
                .ToList();

            foreach (var f in places)
            {
                f.place = f.place.ToLower().Replace(" ", "").Replace(",","/").Replace("//", "/");
            }

            return places;
        }

        /// <summary>
        /// return list of entries in decrypt place cache that don't haven't been geolocated
        /// </summary>
        /// <returns></returns>
        public List<PlaceLookup> GetUnknownPlaces()
        {
            var places = _persistedCacheContext.FTMPlaceCache
                .Where(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]") && w.BadData == false)
                .Select(s => new PlaceLookup() { placeid = s.FTMPlaceId, placeformatted = s.FTMOrginalNameFormatted })
                .ToList();

            foreach (var f in places)
            {
                f.placeformatted = f.placeformatted.Replace("//", "").Replace("|", "");
            }

            return places;
        }

        public List<PlaceLookup> GetUnknownPlacesIgnoreSearchedAlready()
        {
            var places = _persistedCacheContext.FTMPlaceCache.Where(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]")
                                                                     && !w.Searched && !w.BadData)
                .Select(s => new PlaceLookup() { placeid = s.FTMPlaceId, placeformatted = s.FTMOrginalNameFormatted })
                .ToList();

            foreach (var f in places)
            {
                f.placeformatted = f.placeformatted.Replace("//", "").Replace("|", "");
            }

            return places;
        }
    }
}
