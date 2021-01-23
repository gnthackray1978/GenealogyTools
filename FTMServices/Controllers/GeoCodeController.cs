using ConsoleTools;
using FTMContext;
using FTMContext.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace FTMServices.Controllers
{
    public class GeoCodeController : ApiController
    {
        // GET api/values
        public IEnumerable<PlaceLookup> Get()
        {
            var context = FTMakerCacheContext.CreateCacheDB();

            return FTMGeoCoding.GetUnknownPlaces(context, new ConsoleWrapper()).Take(75);
        }
        // POST api/values
        public void Post([FromBody]PlaceLookup value)
        {
            Debug.WriteLine("recieved: " + value.PlaceId + " " + value.results);
            var outputHandler = new OutputHandler();
            var cacheDB = FTMakerCacheContext.CreateCacheDB();
            cacheDB.SetPlaceGeoData(value.PlaceId, value.results);
        }

    }
}
