using QuickGed.Types;
using System;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class FTMPersonView: IEquatable<FTMPersonView>
    {
        public static FTMPersonView Create(Person person) {
            var fTmPersonView = new FTMPersonView
            {
               // Id = idCounter,
                PersonId = person.Id,
                BirthFrom = person.BirthYearFrom,
                BirthTo = person.BirthYearTo,
                AltLat = "0",
                AltLocation = !string.IsNullOrEmpty(person.DeathLocation) ? person.DeathLocation : person.Residence,
                AltLocationDesc = !string.IsNullOrEmpty(person.DeathLocation) ? "Burial" : person.ResidenceDescription,
                AltLong = "0",
                BirthLocation = person.BirthLocation,
                BirthLat = "0",
                BirthLong = "0",
                FirstName = person.Forename,
                Surname = person.FamilyName,
                Origin = person.Origin?? "",
                DirectAncestor = person.IsDirectAncestor,
                LinkedLocations = person.AllLocations(),
                FatherId = person.FatherId,
                MotherId = person.MotherId,
                LinkNode = person.IsLinkNode,
                RootPerson = person.IsRootPerson
            };

            return fTmPersonView;
        }

        public int Id { get; set; }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public int BirthFrom { get; set; }
        public int BirthTo { get; set; }

        public string BirthLocation { get; set; }
        public string BirthLat { get; set; }
        public string BirthLong { get; set; }

        public string AltLocationDesc { get; set; }
        public string AltLocation { get; set; }
        public string AltLat { get; set; }
        public string AltLong { get; set; }

        public string Origin { get; set; }

        public bool DirectAncestor { get; set; }

        public int PersonId { get; set; }

        public int FatherId { get; set; }

        public int MotherId { get; set; }

        public string LinkedLocations { get; set; }

        public int ImportId { get; set; }

        public bool LocationsCached { get; set; }

        public bool RootPerson { get; set; }

        public bool LinkNode { get; set; }

        public int UserId { get; set; }

        //Function to implement getHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + FirstName.GetHashCode();
                hash = hash * 23 + Surname.GetHashCode();
                hash = hash * 23 + BirthFrom.GetHashCode();
                hash = hash * 23 + BirthTo.GetHashCode();
                hash = hash * 23 + BirthLocation.GetHashCode();
                hash = hash * 23 + BirthLat.GetHashCode();
                hash = hash * 23 + BirthLong.GetHashCode();
                hash = hash * 23 + AltLocationDesc.GetHashCode();
                hash = hash * 23 + AltLocation.GetHashCode();
                hash = hash * 23 + AltLat.GetHashCode();
                hash = hash * 23 + AltLong.GetHashCode();
                hash = hash * 23 + Origin.GetHashCode();
                hash = hash * 23 + DirectAncestor.GetHashCode();
                hash = hash * 23 + PersonId.GetHashCode();
                hash = hash * 23 + FatherId.GetHashCode();
                hash = hash * 23 + MotherId.GetHashCode();
                hash = hash * 23 + LinkedLocations.GetHashCode();
                hash = hash * 23 + ImportId.GetHashCode();
                hash = hash * 23 + LocationsCached.GetHashCode();
                hash = hash * 23 + RootPerson.GetHashCode();
                hash = hash * 23 + LinkNode.GetHashCode();
                hash = hash * 23 + UserId.GetHashCode();
                return hash;
            }
        }
    
        //Function to implement Equals
        public bool Equals(FTMPersonView other)
        {
            if (this.Id != other.Id) return false;
            if (this.FirstName != other.FirstName) return false;
            if (this.Surname != other.Surname) return false;
            if (this.BirthFrom != other.BirthFrom) return false;
            if (this.BirthTo != other.BirthTo) return false;
            if (this.BirthLocation != other.BirthLocation) return false;
            if (this.BirthLat != other.BirthLat) return false;
            if (this.BirthLong != other.BirthLong) return false;
            if (this.AltLocationDesc != other.AltLocationDesc) return false;
            if (this.AltLocation != other.AltLocation) return false;
            if (this.AltLat != other.AltLat) return false;
            if (this.AltLong != other.AltLong) return false;
            if (this.Origin != other.Origin) return false;
            if (this.DirectAncestor != other.DirectAncestor) return false;
            if (this.PersonId != other.PersonId) return false;
            if (this.FatherId != other.FatherId) return false;
            if (this.MotherId != other.MotherId) return false;
            if (this.LinkedLocations != other.LinkedLocations) return false;
            if (this.ImportId != other.ImportId) return false;
            if (this.LocationsCached != other.LocationsCached) return false;
            if (this.RootPerson != other.RootPerson) return false;
            if (this.LinkNode != other.LinkNode) return false;
            if (this.UserId != other.UserId) return false;

            return true;
        }

        //Function to implement Equals
        public override bool Equals(object obj)
        {
            return this.Equals(obj as FTMPersonView);
        }
    }
}