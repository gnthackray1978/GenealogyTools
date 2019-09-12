using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class GroupTable
    {
        [Column("RecID")]
        public int RecId { get; set; }
        [Column("GroupID")]
        public int? GroupId { get; set; }
        [Column("StartID")]
        public int? StartId { get; set; }
        [Column("EndID")]
        public int? EndId { get; set; }
    }
}
