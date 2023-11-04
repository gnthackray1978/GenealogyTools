using System;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class PersonOrigins: IEquatable<PersonOrigins>
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Origin { get; set; }
        public bool DirectAncestor { get; set; }
        public int ImportId { get; set; }
        public int UserId { get; set; }

        //Function to implement getHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + PersonId.GetHashCode();
                hash = hash * 23 + Origin.GetHashCode();
                hash = hash * 23 + DirectAncestor.GetHashCode();
                hash = hash * 23 + ImportId.GetHashCode();
                hash = hash * 23 + UserId.GetHashCode();
                return hash;
            }
        }

        //Function to implement Equals
        public bool Equals(PersonOrigins other)
        {
            if (this.Id != other.Id) return false;
            if (this.PersonId != other.PersonId) return false;
            if (this.Origin != other.Origin) return false;
            if (this.DirectAncestor != other.DirectAncestor) return false;
            if (this.ImportId != other.ImportId) return false;
            if (this.UserId != other.UserId) return false;

            return true;
        }

        //Function to implement Equals
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PersonOrigins);
        }
    }
}