using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class ConfigTable
    {
        [Column("RecID")]
        public int RecId { get; set; }
        public int? RecType { get; set; }
        public string Title { get; set; }
        public string DataRec { get; set; }
    }
}
