using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Diagnostics;
using System.Linq;
using PlaceLibNet.Data.Repositories;

namespace PlaceLibNet.Domain
{

    public class PlaceRecordCollection : IEnumerable<PlaceRecordItem>
    {
        private int _duplicateCount = 0;
        private int _invalidPlaces = 0;
        public List<PlaceRecordItem> Items { get; set; }

        public int Count => Items.Count;

        public int InvalidLocationsCount => _invalidPlaces;

        public int DuplicateLocationsCount => _duplicateCount;

        public PlaceRecordCollection(List<string> places)
        {
            this.InsertRange(places);
        }

        public void InsertRange(List<string> places)
        {
            foreach (var place in places)
            {
                Insert(place);
            }
        }

        public void Insert(string place)
        {

            //if (place.Contains("wooburn"))
            //{
            //    Debug.WriteLine("");
            //}

            Items ??= new List<PlaceRecordItem>();

            string formattedPlace = FormatPlace(place);

            if (isValid(formattedPlace))
            {
                var placeLh = new PlaceRecordItem()
                {
                    Place = place,
                    PlaceFormatted = formattedPlace
                };

                if (Items.All(a => a.PlaceFormatted != placeLh.PlaceFormatted))
                    Items.Add(placeLh);
                else
                    _duplicateCount++;
            }
            else
            {
                _invalidPlaces++;
            }


        }

        private string FormatPlace(string place)
        {
            place =  place
                .ToLower()
                .Replace(" ", "")
                .Replace(",", "/")
                .Replace("//", "/");
            
            place = PlaceRepository.DeleteNonAlphaNumericExceptSlash(place);

            place = PlaceRepository.ReplaceSlashesWithSingleSlash(place);
            
            return place;
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
                if (count > 1)
                {
                    return true;
                }
            }

            


            return false;
        }

        public IEnumerator<PlaceRecordItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }


    public class PlaceRecordItem : IComparable<PlaceRecordItem>
    {
        public string PlaceFormatted { get; set; }

        public string Place { get; set; }

        public int GoogleCacheId { get; set; }

        public int PlaceLibId { get; set; }

        public string Country { get; set; }

        public string County { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public int CompareTo(PlaceRecordItem obj)
        {
            return this.PlaceFormatted.CompareTo(obj.PlaceFormatted);
        }

    }
}
