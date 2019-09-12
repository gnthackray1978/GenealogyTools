using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class PlaceTable
    {
        [Column("PlaceID")]
        public int PlaceId { get; set; }
        public int? PlaceType { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }
        public string Normalized { get; set; }
        public int? Latitude { get; set; }
        public int? Longitude { get; set; }
        public int? LatLongExact { get; set; }
        [Column("MasterID")]
        public int? MasterId { get; set; }
        public string Note { get; set; }
    }
}
