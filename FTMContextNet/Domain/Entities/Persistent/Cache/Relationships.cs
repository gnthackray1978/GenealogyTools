using QuickGed.Types;
using System;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class Relationships: IEquatable<Relationships>
    {
        public static Relationships Create(RelationSubSet r)
        {
            return new Relationships()
            {
                BrideId = r.Person1Id.GetValueOrDefault(),
                GroomId = r.Person2Id.GetValueOrDefault(),
                DateStr = r.DateStr,
                Year = r.DateYear,
                Notes = r.Text,
                Origin = r.Origin,
                Location = r.PlaceName
            };
        }

        public int Id { get; set; }
        public int GroomId { get; set; }

        public int BrideId { get; set; }

        public string Origin { get; set; }

        public string Location { get; set; }

        public string Notes { get; set; }

        public string DateStr { get; set; }

        public int Year { get; set; }

        public int ImportId { get; set; }

        public int UserId { get; set; }

        //Function to implement getHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + GroomId.GetHashCode();
                hash = hash * 23 + BrideId.GetHashCode();
                hash = hash * 23 + Origin.GetHashCode();
                hash = hash * 23 + Location.GetHashCode();
                hash = hash * 23 + Notes.GetHashCode();
                hash = hash * 23 + DateStr.GetHashCode();
                hash = hash * 23 + Year.GetHashCode();
                hash = hash * 23 + ImportId.GetHashCode();
                hash = hash * 23 + UserId.GetHashCode();
                return hash;
            }
        }

        //Function to implement Equals
        public bool Equals(Relationships other)
        {
            if (this.Id != other.Id) return false;
            if (this.GroomId != other.GroomId) return false;
            if (this.BrideId != other.BrideId) return false;
            if (this.Origin != other.Origin) return false;
            if (this.Location != other.Location) return false;
            if (this.Notes != other.Notes) return false;
            if (this.DateStr != other.DateStr) return false;
            if (this.Year != other.Year) return false;
            if (this.ImportId != other.ImportId) return false;
            if (this.UserId != other.UserId) return false;

            return true;
        }

        //Function to implement Equals
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Relationships);
        }
    }
}