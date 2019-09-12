using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class FamilyTable
    {
        [Column("FamilyID")]
        public int FamilyId { get; set; }
        [Column("FatherID")]
        public int? FatherId { get; set; }
        [Column("MotherID")]
        public int? MotherId { get; set; }
        [Column("ChildID")]
        public int? ChildId { get; set; }
        public int? HusbOrder { get; set; }
        public int? WifeOrder { get; set; }
        public int? IsPrivate { get; set; }
        public int? Proof { get; set; }
        public int? SpouseLabel { get; set; }
        public int? FatherLabel { get; set; }
        public int? MotherLabel { get; set; }
        public string Note { get; set; }
    }
}
