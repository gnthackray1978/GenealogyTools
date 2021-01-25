using ConsoleTools;
using FTMContext;
using FTMContext.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace FTMServices4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoCodeController : ControllerBase
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        public GeoCodeController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }


        // GET api/values
        public IEnumerable<PlaceLookup> Get()
        {
            var context = FTMakerCacheContext.CreateCacheDB();

            return FTMGeoCoding.GetUnknownPlaces(context, new OutputHandler(_hubContext)).Take(75);
        }
        // POST api/values
        public void Post([FromBody]PlaceLookup value)
        {
            //Debug.WriteLine("recieved: " + value.PlaceId + " " + value.results);
            var outputHandler = new OutputHandler(_hubContext);
            var cacheDB = FTMakerCacheContext.CreateCacheDB();
            cacheDB.SetPlaceGeoData(value.PlaceId, value.results);
        }

    }
}
