using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class ChildTable
    {
        [Column("RecID")]
        public int RecId { get; set; }
        [Column("ChildID")]
        public int? ChildId { get; set; }
        [Column("FamilyID")]
        public int? FamilyId { get; set; }
        public int? RelFather { get; set; }
        public int? RelMother { get; set; }
        public int? ChildOrder { get; set; }
        public int? IsPrivate { get; set; }
        public int? ProofFather { get; set; }
        public int? ProofMother { get; set; }
        public string Note { get; set; }
    }
}
