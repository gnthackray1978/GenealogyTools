using System.Text.RegularExpressions;
using System;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class TreeRecord: IEquatable<TreeRecord>
    {
        public static TreeRecord CreateFromOrigin(string origin, string locationCsv, int personCount, int importId)
        {
            Regex re = new Regex(@"\d+");

            Match m = re.Match(origin);

            int cmVal = 0;

            if (m.Success)
            {
                int.TryParse(m.Value, out cmVal);
            }

            re = new Regex(@"[fF]\d+");

            m = re.Match(origin);

            var tr =new TreeRecord(){
                PersonCount = personCount,
                Name = origin,
                Origin = locationCsv,
                CM = cmVal,
                Located = m.Success,
                ImportId = importId
            };

            return tr;
        }

        public int Id { get; set; }
        public int PersonCount { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }
        public int CM { get; set; }

        public bool Located { get; set; }

        public int ImportId { get; set; }

        public int UserId { get; set; }

        //Function to implement getHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                hash = hash * 23 + PersonCount.GetHashCode();
                hash = hash * 23 + Origin.GetHashCode();
                hash = hash * 23 + Name.GetHashCode();
                hash = hash * 23 + CM.GetHashCode();
                hash = hash * 23 + Located.GetHashCode();
                hash = hash * 23 + ImportId.GetHashCode();
                hash = hash * 23 + UserId.GetHashCode();
                return hash;
            }
        }

        //Function to implement Equals
        public bool Equals(TreeRecord other)
        {
            if (this.Id != other.Id) return false;
            if (this.PersonCount != other.PersonCount) return false;
            if (this.Origin != other.Origin) return false;
            if (this.Name != other.Name) return false;
            if (this.CM != other.CM) return false;
            if (this.Located != other.Located) return false;
            if (this.ImportId != other.ImportId) return false;
            if (this.UserId != other.UserId) return false;

            return true;
        }

        //Function to implement Equals
        public override bool Equals(object obj)
        {
            return this.Equals(obj as TreeRecord);
        }
    }
}