using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class AddressTable
    {
        [Column("AddressID")]
        public int AddressId { get; set; }
        public int? AddressType { get; set; }
        public string Name { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        [Column("URL")]
        public string Url { get; set; }
        public int? Latitude { get; set; }
        public int? Longitude { get; set; }
        public string Note { get; set; }
    }
}
