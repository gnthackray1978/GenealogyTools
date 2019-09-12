using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class EventTable
    {
        [Column("EventID")]
        public int EventId { get; set; }
        public int? EventType { get; set; }
        public int? OwnerType { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        [Column("FamilyID")]
        public int? FamilyId { get; set; }
        [Column("PlaceID")]
        public int? PlaceId { get; set; }
        [Column("SiteID")]
        public int? SiteId { get; set; }
        public string Date { get; set; }
        public long? SortDate { get; set; }
        public int? IsPrimary { get; set; }
        public int? IsPrivate { get; set; }
        public int? Proof { get; set; }
        public int? Status { get; set; }
        public double? EditDate { get; set; }
        public string Sentence { get; set; }
        public string Details { get; set; }
        public string Note { get; set; }
    }
}
