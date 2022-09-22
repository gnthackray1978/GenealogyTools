using FTMContextNet.Domain.Entities.Persistent.Cache;
using GoogleMapsHelpers;

namespace FTMContextNet.Domain.Transient
{
    public class ExtendedPlace
    {
        public FtmPlaceCache Place { get; set; }

        public LocationInfo LocationInfo { get; set; }

        public bool ProspectivePostal() {

            if (Place.County == "" && LocationInfo.PostalTown != "")
            {
                return true;
            }

            return false;
        }

        public bool ProspectivePolitical()
        {
            if (Place.County == "" && LocationInfo.Political != "")
            {
                return true;
            }

            return false;
        }
    }
}
