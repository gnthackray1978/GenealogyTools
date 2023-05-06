using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ConfigHelper;
using LoggingLib;
using PlaceLib;
using PlaceLib.Model;
using PlaceLibNet.Data.Contexts;

namespace PlaceLibNet.Data.Repositories
{
    public class PlacesRepository
    {
        protected PlacesContext _placesContext;

        public PlacesRepository(PlacesContext placesContext, Ilog logger)
        {
            this._placesContext = placesContext;

        }

        public List<PlaceDto> GetPlaces()
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());
            
            var places = placecontext.Places.Where(w => w.Ctyhistnm != "").Select(s => new PlaceDto()
            {
                County = s.Ctyhistnm.ToLower(),
                Country = s.Ctry15nm,
                Place = s.Place15nm.ToLower()
            }).Distinct().ToList();

            return places;
        }

        public List<CountyDto> GetCounties(bool originalCase = false)
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            var counties = placecontext.Places.Select(s => new CountyDto()
            {
                County = originalCase ? s.Ctyhistnm.ToLower() : s.Ctyhistnm,
                Country = s.Ctry15nm

            }).Distinct().Where(w => w.County.Trim() != "").ToList();

            return counties;
        }

        public string SearchPlacesDBForCounty(string searchString)
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            string county = "";

            var placedbResult = placecontext.Places.FirstOrDefault(w => w.Place15nm == searchString);

            if (placedbResult != null)
            {
                county = placedbResult.Ctyhistnm;
            }
            else
            {
                var stripped = searchString.ToLower();
                stripped = Regex.Replace(stripped, " ", "");

                placedbResult = placecontext.Places.FirstOrDefault(w => w.Placesort == stripped);

                if (placedbResult != null)
                {
                    county = placedbResult.Ctyhistnm;
                }
            }

            return county;
        }
        
        public Places SearchPlaces(string searchString, string county)
        {
            using var placecontext = new PlacesContext(new MSGConfigHelper());

            Debug.WriteLine(searchString + "/"+ county);
            Places placedbResult = placecontext
                .Places
                .Where(w => w.Placesort == searchString && w.Ctyhistnm == county)
                .OrderBy(o=>o.Place15cd)
                .FirstOrDefault();
            
            return placedbResult;
        }
    }
}
