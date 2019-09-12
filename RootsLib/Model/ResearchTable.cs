using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class ResearchTable
    {
        [Column("TaskID")]
        public int TaskId { get; set; }
        public int? TaskType { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        public int? OwnerType { get; set; }
        public string RefNumber { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Date3 { get; set; }
        public long? SortDate1 { get; set; }
        public long? SortDate2 { get; set; }
        public long? SortDate3 { get; set; }
        public string Filename { get; set; }
        public string Details { get; set; }
    }
}
