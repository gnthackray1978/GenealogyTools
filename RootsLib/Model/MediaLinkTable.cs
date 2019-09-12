using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class MediaLinkTable
    {
        [Column("LinkID")]
        public int LinkId { get; set; }
        [Column("MediaID")]
        public int? MediaId { get; set; }
        public int? OwnerType { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        public int? IsPrimary { get; set; }
        public int? Include1 { get; set; }
        public int? Include2 { get; set; }
        public int? Include3 { get; set; }
        public int? Include4 { get; set; }
        public int? SortOrder { get; set; }
        public int? RectLeft { get; set; }
        public int? RectTop { get; set; }
        public int? RectRight { get; set; }
        public int? RectBottom { get; set; }
        public string Note { get; set; }
        public string Caption { get; set; }
        public string RefNumber { get; set; }
        public string Date { get; set; }
        public long? SortDate { get; set; }
        public string Description { get; set; }
    }
}
