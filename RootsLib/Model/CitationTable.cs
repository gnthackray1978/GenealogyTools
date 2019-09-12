using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class CitationTable
    {
        [Column("CitationID")]
        public int CitationId { get; set; }
        public int? OwnerType { get; set; }
        [Column("SourceID")]
        public int? SourceId { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        public string Quality { get; set; }
        public int? IsPrivate { get; set; }
        public string Comments { get; set; }
        public string ActualText { get; set; }
        public string RefNumber { get; set; }
        public int? Flags { get; set; }
        public string Fields { get; set; }
    }
}
