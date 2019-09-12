using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class NameTable
    {
        [Column("NameID")]
        public int NameId { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        public string Surname { get; set; }
        public string Given { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string Nickname { get; set; }
        public int? NameType { get; set; }
        public string Date { get; set; }
        public long? SortDate { get; set; }
        public int? IsPrimary { get; set; }
        public int? IsPrivate { get; set; }
        public int? Proof { get; set; }
        public double? EditDate { get; set; }
        public string Sentence { get; set; }
        public string Note { get; set; }
        public int? BirthYear { get; set; }
        public int? DeathYear { get; set; }
    }
}
