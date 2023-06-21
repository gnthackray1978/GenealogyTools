using System;
using System.Collections.Generic;
using System.Linq;

namespace FTMContextNet.Domain.Entities.NonPersistent.Person
{
    public class PersonPlace : IComparable<PersonPlace>
    {
        public string PlaceFormatted { get; set; }

        public string Place { get; set; }

        public int GoogleCacheId { get; set; }

        public int PlaceLibId { get; set; }

        public string Country { get; set; }

        public string County { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public int CompareTo(PersonPlace obj)
        {
            return PlaceFormatted.CompareTo(obj.PlaceFormatted);
        }

        public IEnumerable<string> GetComponents()
        {
            var placeParts = PlaceFormatted.Split("/").SkipLast(2);

            return placeParts;
        }

    }
}
