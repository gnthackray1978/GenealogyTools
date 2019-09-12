using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class AddressLinkTable
    {
        [Column("LinkID")]
        public int LinkId { get; set; }
        public int? OwnerType { get; set; }
        [Column("AddressID")]
        public int? AddressId { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        public int? AddressNum { get; set; }
        public string Details { get; set; }
    }
}
