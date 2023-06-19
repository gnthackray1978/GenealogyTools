using PlaceLibNet.Domain.Entities;

namespace FTMContextNet.Domain.Entities.NonPersistent
{
    public class FTMPlaceCacheSubset
    {

        public int Id { get; set; }

        public int FTMPlaceId { get; set; }

        public string County { get; set; }
        public string Country { get; set; }
        public string FTMOrginalNameFormatted { get; set; }
        public double location_lat { get; set; }
        public double location_long { get; set; }
        public DupeLocInfoTypes DupeLocInfoType { get; set; }
    }
}
