using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class PersonTable
    {
        [Column("PersonID")]
        public int PersonId { get; set; }
        [Column("UniqueID")]
        public string UniqueId { get; set; }
        public int? Sex { get; set; }
        public double? EditDate { get; set; }
        [Column("ParentID")]
        public int? ParentId { get; set; }
        [Column("SpouseID")]
        public int? SpouseId { get; set; }
        public int? Color { get; set; }
        public int? Relate1 { get; set; }
        public int? Relate2 { get; set; }
        public int? Flags { get; set; }
        public int? Living { get; set; }
        public int? IsPrivate { get; set; }
        public int? Proof { get; set; }
        public int? Bookmark { get; set; }
        public string Note { get; set; }
    }
}
