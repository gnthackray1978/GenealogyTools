using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class ExclusionTable
    {
        [Column("RecID")]
        public int RecId { get; set; }
        public int? ExclusionType { get; set; }
        [Column("ID1")]
        public int? Id1 { get; set; }
        [Column("ID2")]
        public int? Id2 { get; set; }
    }
}
