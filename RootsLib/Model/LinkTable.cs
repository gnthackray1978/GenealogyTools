using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class LinkTable
    {
        [Column("LinkID")]
        public int LinkId { get; set; }
        [Column("extSystem")]
        public int? ExtSystem { get; set; }
        public int? LinkType { get; set; }
        [Column("rmID")]
        public int? RmId { get; set; }
        [Column("extID")]
        public string ExtId { get; set; }
        public int? Modified { get; set; }
        [Column("extVersion")]
        public string ExtVersion { get; set; }
        [Column("extDate")]
        public double? ExtDate { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
    }
}
