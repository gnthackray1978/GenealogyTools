using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class WitnessTable
    {
        [Column("WitnessID")]
        public int WitnessId { get; set; }
        [Column("EventID")]
        public int? EventId { get; set; }
        [Column("PersonID")]
        public int? PersonId { get; set; }
        public int? WitnessOrder { get; set; }
        public int? Role { get; set; }
        public string Sentence { get; set; }
        public string Note { get; set; }
        public string Given { get; set; }
        public string Surname { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }
}
