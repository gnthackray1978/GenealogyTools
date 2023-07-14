using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Misc;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching
{
    public interface IPlaceLibCoordCache
    {
        string validateCounty(string county);

        /// <summary>
        /// Searchs for PlaceSubset with Lat and Long in it for given places and county
        /// </summary>
        /// <param name="places">ANY CASE</param>
        /// <param name="county">ANY CASE</param>
        /// <returns></returns>
        PlaceSearchCoordSubset Search(IEnumerable<string> places, string county);

        /// <summary>
        /// Search place library database
        /// </summary>
        /// <param name="placeName">LOWERCASE</param>
        /// <param name="county">CAMELCASE</param>
        /// <returns></returns>
        PlaceSearchCoordSubset PlaceSearchCoordSubset(string placeName, string county);
    }

    public class PlaceLibCoordCache : IPlaceLibCoordCache
    {
        private readonly IPlaceRepository _placeRepository;

        private readonly List<PlaceSearchCoordSubset> _places;

        private readonly List<CountyDto> _lowerCaseCounties;

        private readonly IPlaceNameFormatter _placeNameFormatter;
        /// <summary>
        /// Places table caching object
        /// </summary>
        /// <param name="placeRepository"></param>
        /// <param name="placeNameFormatter"></param>
        public PlaceLibCoordCache(IPlaceRepository placeRepository, IPlaceNameFormatter placeNameFormatter)
        {
            _placeNameFormatter = placeNameFormatter;

            var places = placeRepository.GetPlaceLibCoords();

            var lowerCaseCounties = placeRepository.GetCounties(true);

            if (places == null || lowerCaseCounties== null || placeNameFormatter== null)
                throw new NullReferenceException();

            if (places.Count == 0 || lowerCaseCounties.Count == 0)
                throw new InvalidDataException();

            if(Char.IsUpper(lowerCaseCounties.First().County,0))
            {
                throw new InvalidDataException("Counties collection should be all lowercase");
            }

            _places = places;
            _lowerCaseCounties = lowerCaseCounties;
           // _lowerCaseCounties.Sort();
            _places.Sort((s, y) => s.Placesort.CompareTo(y.Placesort));

        }

        public string validateCounty(string county)
        {
            var result = _lowerCaseCounties
                .FirstOrDefault(countyDto => county == countyDto.County)?.County;

            if (string.IsNullOrEmpty(result)) return null;

            return StringTools.Capitalize(result);
        }


        public string getCounty(string test)
        {
            return _lowerCaseCounties.FirstOrDefault(f => test.Contains(f.County))?.County;
        }


        /// <summary>
        /// Searchs for PlaceSubset with Lat and Long in it for given places and county
        /// </summary>
        /// <param name="places">ANY CASE</param>
        /// <param name="county">ANY CASE</param>
        /// <returns></returns>
        public PlaceSearchCoordSubset Search(IEnumerable<string> places, string county)
        {
            county = county.ToLower();

            //try and get a county.
            var validatedCounty = validateCounty(county) ?? getCounty(county);

            if (validatedCounty != null) validatedCounty = StringTools.Capitalize(validatedCounty);

            return places
                .Select(placeName => PlaceSearchCoordSubset(placeName, validatedCounty))
                .FirstOrDefault(placeCoords => placeCoords != null);
        }

        /// <summary>
        /// Search place library database
        /// </summary>
        /// <param name="placeName">LOWERCASE</param>
        /// <param name="county">CAMELCASE</param>
        /// <returns></returns>
        public PlaceSearchCoordSubset PlaceSearchCoordSubset(string placeName, string county)
        {
            int placeSearchCoord = _places.BinarySearch(new PlaceSearchCoordSubset()
                { Placesort = _placeNameFormatter.FormatComponent(placeName) },
                    Comparer<PlaceSearchCoordSubset>
                        .Create((s, y) => s.Placesort.CompareTo(y.Placesort)));

            if (placeSearchCoord < 0)
                return null;

            // we've got no way to narrow this down
            // if there are multiple places with this name
            // so just return the first one.
            if(county == null)
                return _places[placeSearchCoord];
            
            int idx = placeSearchCoord - 5;

            while (idx <= placeSearchCoord + 5)
            {
                //Ctyhistnm is camel case in the db
                if (idx >= 0 && idx < _places.Count)
                {
                    if (_places[idx].Ctyhistnm == county)
                    {
                        return _places[idx];
                    }
                }

                idx++;
            }

            return null;
        }
    }
}
