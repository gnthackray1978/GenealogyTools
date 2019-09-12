using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class RoleTable
    {
        [Column("RoleID")]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int? EventType { get; set; }
        public int? RoleType { get; set; }
        public string Sentence { get; set; }
    }
}
