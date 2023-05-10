using GoogleMapsHelpers;
using PlaceLib.Model;

namespace PlaceLibNet.Domain
{
    public class ExtendedPlace
    {
        public PlaceCache Place { get; set; }

        public LocationInfo LocationInfo { get; set; }

        public bool ProspectivePostal()
        {

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
