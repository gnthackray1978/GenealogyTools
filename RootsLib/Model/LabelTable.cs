using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class LabelTable
    {
        [Column("LabelID")]
        public int LabelId { get; set; }
        public int? LabelType { get; set; }
        public int? LabelValue { get; set; }
        public string LabelName { get; set; }
        public string Description { get; set; }
    }
}
