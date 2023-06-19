using System.Collections.Generic;
using System.Linq;
using Misc;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching
{
    public class PlaceLibCoordCache
    {
        private readonly List<PlaceSearchCoordSubset> _places;

        private readonly List<CountyDto> _lowerCaseCounties;

        private readonly IPlaceNameFormatter _placeNameFormatter;

        public PlaceLibCoordCache(List<PlaceSearchCoordSubset> places,
                               List<CountyDto> lowerCaseCounties, 
                               IPlaceNameFormatter placeNameFormatter)
        {
            _placeNameFormatter = placeNameFormatter;

            _places = places;
            _lowerCaseCounties = lowerCaseCounties;

            _places.Sort((s, y) => s.Placesort.CompareTo(y.Placesort));

        }

        private string validateCounty(string county)
        {
            county = county.ToLower();

            var result = "";
            foreach (var countyDto in _lowerCaseCounties.Where(countyDto => county == countyDto.County))
            {
                result = countyDto.County;
                break;
            }

            if (string.IsNullOrEmpty(result)) return null;

            return StringTools.Capitalize(result);
        }

        public PlaceSearchCoordSubset Search(IEnumerable<string> places, string county)
        {
            var validatedCounty = validateCounty(county);

            foreach (var placeName in places)
            {
                var placeCoords = PlaceSearchCoordSubset(placeName, validatedCounty);

                if (placeCoords != null)
                    break;
            }

            return null;
        }

        /// <summary>
        /// Search place library database
        /// </summary>
        /// <param name="placeName">LOWERCASE</param>
        /// <param name="county">CAMELCASE</param>
        /// <returns></returns>
        private PlaceSearchCoordSubset PlaceSearchCoordSubset(string placeName, string county)
        {
            int placeSearchCoord = _places.BinarySearch(new PlaceSearchCoordSubset()
                { Placesort = _placeNameFormatter.FormatComponent(placeName) },
                    Comparer<PlaceSearchCoordSubset>
                        .Create((s, y) => s.Placesort.CompareTo(y.Placesort)));

            int idx = placeSearchCoord - 5;

            while (idx <= placeSearchCoord + 5)
            {
                //Ctyhistnm is camel case in the db
                if (_places[idx].Ctyhistnm == county)
                {
                    return _places[idx];
                }

                idx++;
            }

            return null;
        }
    }
}
