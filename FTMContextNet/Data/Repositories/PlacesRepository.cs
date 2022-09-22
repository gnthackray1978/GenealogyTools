using System.Linq;
using System.Text.RegularExpressions;
using PlaceLib.Model;

namespace FTMContextNet.Data.Repositories
{
    public class PlacesRepository
    {
        protected PlacesContext _placesContext;

        public PlacesRepository(PlacesContext placesContext)
        {
            this._placesContext = placesContext;
        }




        public string SearchPlacesDBForCounty(string searchString)
        {
            string county = "";

            var placedbResult = _placesContext.Places.FirstOrDefault(w => w.Place15nm == searchString);

            if (placedbResult != null)
            {
                county = placedbResult.Ctyhistnm;
            }
            else
            {
                var stripped = searchString.ToLower();
                stripped = Regex.Replace(stripped, " ", "");

                placedbResult = _placesContext.Places.FirstOrDefault(w => w.Placesort == stripped);

                if (placedbResult != null)
                {
                    county = placedbResult.Ctyhistnm;
                }
            }

            return county;
        }

    }
}
