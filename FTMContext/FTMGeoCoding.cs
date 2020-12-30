using FTMContext.lib;
using FTMContext.Models;
using Newtonsoft.Json;
using PlaceLib.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTMContext
{
    public class FTMGeoCoding
    {
        public static Location GetLocation(string jsonResult)
        {
        
            //var a = new FTMakerContext(new ConfigObj
            //{
            //    Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
            //    FileName = @"decrrypted.db",
            //    IsEncrypted = false
            //});

            //var first = a.FTMPlaceCache.First();

            if(jsonResult==null)
                return new Location();

            var _root = JsonConvert.DeserializeObject<List<Result>>(jsonResult);

            if (_root != null && _root.Count > 0 && _root.First().geometry!=null)
                return _root.First().geometry.location;
            else
                return new Location();

        }

        /// <summary>
        /// sets county and country values in ftmcache. 
        /// </summary>
        public static void UpdateFTMCacheMetaData()
        {
            var p = new PlacesContext();

            var a = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });
            var unsetCountiesCount = a.FTMPlaceCache.Where(w => w.County == "" || w.Country == "").Count();

            Console.WriteLine("FTMPlaceCache has ~" + unsetCountiesCount + " unset records");
            Console.WriteLine("Updating FTMPlaceCache");
            int foreignCounties = 0;
            foreach (var place in a.FTMPlaceCache.Where(w=>w.County == "" || w.Country == "")) {
                if (place.JSONResult != null)
                {

                    var _root = JsonConvert.DeserializeObject<List<Result>>(place.JSONResult);

                    if (_root == null || _root.Count ==0) continue;

                    var country = GoogleGeoCodingHelpers.GetType(_root, "country","");

                    place.Country = "";
                    place.County = "";

                    if (country != "United Kingdom")
                    {
                        place.Country = country;
                        foreignCounties++;
                    }
                    else
                    {
                        var postal_town = GoogleGeoCodingHelpers.GetType(_root, "postal_town","");
                        var political = GoogleGeoCodingHelpers.GetType(_root, "political", "");
                        var state = GoogleGeoCodingHelpers.GetType(_root, "administrative_area_level_1", "");
                        var county = GoogleGeoCodingHelpers.GetType(_root, "administrative_area_level_2", state);

                        if (county == "" && postal_town != "")
                        {
                            county = p.SearchPlacesDBForCounty(postal_town);                            
                        }

                        if (county == "" && political != "")
                        {
                            if (HistoricCounties.Get.Contains(political))
                            {
                                county = political;
                            }

                            if (county == "")
                                county = p.SearchPlacesDBForCounty(political);
                        }


                        place.Country = state;

                        place.County = county;
                    }
                }
              
                a.SaveChanges();
            }

            unsetCountiesCount = a.FTMPlaceCache.Where(w => w.County == "" || w.Country == "").Count();

            Console.WriteLine("FTMPlaceCache has ~" + foreignCounties + " foreign records");

            Console.WriteLine("FTMPlaceCache has ~" + unsetCountiesCount + " unset records");
        }

        

        /// <summary>
        /// return list of entries in decrypt place cache that don't haven't been geolocated
        /// </summary>
        /// <returns></returns>
        public static List<PlaceLookup> GetUnknownPlaces()
        {

            var places = new List<PlaceLookup>();

            using (var context = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            }))
            {

                places = context.FTMPlaceCache.Where(w => (w.JSONResult == null || w.JSONResult == "null"))
                    .Select(s => new PlaceLookup() { PlaceId = s.FTMPlaceId, PlaceFormatted = s.FTMOrginalNameFormatted })
                    .ToList();

            };

            return places;
        }
      
        /// <summary>
        /// add missing places into the place cache. ready to be looked up by the geocoder
        /// </summary>
        public static void AddMissingPlaces()
        {
            var a = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });

            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(a);


            Console.WriteLine("Adding " + data.missingPlaces.Count + " missing places");

            if (data.missingPlaces.Count > 0)
            {
                int newId = a.FTMPlaceCache.Count() +1;

                foreach (var p in data.missingPlaces)
                {

                    a.FTMPlaceCache.Add(new Models.FTMPlaceCache()
                    {
                        Id = newId,
                        FTMOrginalName = p.Name,
                        JSONResult = null,
                        FTMOrginalNameFormatted = GoogleGeoCodingHelpers.FormatPlace(p.Name),
                        FTMPlaceId = p.Id
                    });

                    newId++;
                    a.SaveChanges();
                }
            }

        }

       

        /// <summary>
        /// finds place ids in the ftmcache and sets them to null when
        /// the place ids have been changed.
        /// </summary>
        public static void ResetUpdatedPlaces()
        {
            var a = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });

            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(a);

            Console.WriteLine("Resetting " + data.updatedPlaces.Count + " places press any key to continue");
            Console.ReadKey();

            if (data.updatedPlaces.Count > 0)
            {
                foreach (var p in data.updatedPlaces)
                {
                    var cachedValue = a.FTMPlaceCache.FirstOrDefault(f => f.FTMPlaceId == p.Id);

                    if (cachedValue != null)
                    {
                        cachedValue.FTMOrginalName = p.Name;
                        cachedValue.JSONResult = null;
                        cachedValue.Country = "";
                        cachedValue.County = "";
                        cachedValue.FTMOrginalNameFormatted = GoogleGeoCodingHelpers.FormatPlace(p.Name);
                    }

                    a.SaveChanges();
                }
            }

        }
        
        public static (List<Place> missingPlaces, List<Place> updatedPlaces) CheckForUpdates(FTMakerContext a, bool showInfo = false)
        {

            Console.WriteLine("Checking for updated places");
            //    ExtractFTMDB();



            var placeDictionary = new Dictionary<int, string>();
            var cacheDictionary = new Dictionary<int, string>();

            foreach (var p in a.FTMPlaceCache)
            {
                if (!cacheDictionary.ContainsKey(p.FTMPlaceId))
                    cacheDictionary.Add(p.FTMPlaceId, p.FTMOrginalName);
            }

            List<Place> missingPlaces = new List<Place>();
            List<Place> updatedPlaces = new List<Place>();

            foreach (var p in a.Place)
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
                Console.WriteLine(updatedPlaces.Count + " updated places ");

                foreach (var m in updatedPlaces)
                {
                    Console.WriteLine("Missing Place : " + m.Name);
                }

                Console.WriteLine(missingPlaces.Count + " missing places ");

                foreach (var m in missingPlaces)
                {
                    Console.WriteLine("Missing Place : " + m.Name);
                }
            }
            return (missingPlaces, updatedPlaces);
        }
        

       

    }
}
