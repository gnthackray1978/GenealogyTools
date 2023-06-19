using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching
{
    public class CountySearch
    {
        private readonly List<PlacePair> _placeSort;
        private readonly List<PlacePair> _place15;
        private readonly IPlaceNameFormatter _placeNameFormatter;
        
        public CountySearch(List<Places> countyData, IPlaceNameFormatter placeNameFormatter)
        {
            _placeNameFormatter = placeNameFormatter;

            _place15 = countyData.Select(s => new PlacePair() { Place = _placeNameFormatter.Format(s.Place15nm), County = s.Ctyhistnm }).ToList();

            _place15.Sort((s, y) => s.Place.CompareTo(y.Place));


            //place sort is already lower case and stripped of whitespace

            _placeSort = countyData.Select(s => new PlacePair() { Place = s.Placesort, County = s.Ctyhistnm }).ToList();

            _placeSort.Sort((s, y) => s.Place.CompareTo(y.Place));
        }
        
        public string Search(string searchString)
        {
            searchString = _placeNameFormatter.Format(searchString);
            
            string county = "";

            var placedbResult = _place15.BinarySearch(new PlacePair() { Place = searchString }
                , Comparer<PlacePair>.Create((a, b) => a.Place.CompareTo(b.Place)));

            if (placedbResult >= 0)
            {
                county = _place15[placedbResult].County;
            }
            else
            {
                placedbResult = _placeSort
                    .BinarySearch(new PlacePair() { Place = searchString }
                        , Comparer<PlacePair>.Create((a, b) => a.Place.CompareTo(b.Place)));

                if (placedbResult >= 0)
                {
                    county = _placeSort[placedbResult].County;
                }
            }

            return county;
        }
    }
}
