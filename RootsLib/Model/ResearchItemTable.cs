using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class ResearchItemTable
    {
        [Column("ItemID")]
        public int ItemId { get; set; }
        [Column("LogID")]
        public int? LogId { get; set; }
        public string Date { get; set; }
        public int? SortDate { get; set; }
        public string RefNumber { get; set; }
        public string Repository { get; set; }
        public string Goal { get; set; }
        public string Source { get; set; }
        public string Result { get; set; }
    }
}
