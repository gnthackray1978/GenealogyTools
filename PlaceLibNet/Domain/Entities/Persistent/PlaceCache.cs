using System;

namespace PlaceLibNet.Domain.Entities.Persistent
{
    public partial class PlaceCache : IEquatable<PlaceCache>
    {

        public int Id { get; set; }

        public int AltId { get; set; }

        public string Name { get; set; }
        public string NameFormatted { get; set; }
        public string JSONResult { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public bool Searched { get; set; }

        public bool BadData { get; set; }

        public string Lat { get; set; }
        public string Long { get; set; }

        public string Src { get; set; }

        public DateTime DateCreated { get; set; }

        //Function to implement getHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + AltId.GetHashCode();
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + NameFormatted.GetHashCode();
                hash = hash * 23 + JSONResult.GetHashCode();
                hash = hash * 23 + County.GetHashCode();
                hash = hash * 23 + Country.GetHashCode();
                hash = hash * 23 + Searched.GetHashCode();
                hash = hash * 23 + BadData.GetHashCode();
                hash = hash * 23 + Lat.GetHashCode();
                hash = hash * 23 + Long.GetHashCode();
                hash = hash * 23 + Src.GetHashCode();
                return hash;
            }
        }
        
       
        // function to compare the properties of 2 PlaceCache objects
        public bool Equals(PlaceCache other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   AltId == other.AltId &&
                   Name == other.Name &&
                   NameFormatted == other.NameFormatted &&
                   JSONResult == other.JSONResult &&
                   County == other.County &&
                   Country == other.Country &&
                   Searched == other.Searched &&
                   BadData == other.BadData &&
                   Lat == other.Lat &&
                   Long == other.Long &&
                   Src == other.Src &&
                   DateCreated == other.DateCreated;
        }



    }
}

