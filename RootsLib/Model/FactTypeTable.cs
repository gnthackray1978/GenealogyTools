using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class FactTypeTable
    {
        [Column("FactTypeID")]
        public int FactTypeId { get; set; }
        public int? OwnerType { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }
        public string GedcomTag { get; set; }
        public int? UseValue { get; set; }
        public int? UseDate { get; set; }
        public int? UsePlace { get; set; }
        public string Sentence { get; set; }
        public int? Flags { get; set; }
    }
}
