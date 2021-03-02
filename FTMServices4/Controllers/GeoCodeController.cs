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
    public class Result
    { 
        public IEnumerable<PlaceLookup> Results { get; set; }
    }

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
        [HttpGet]
        public Result Get(string infoType)
        {
            var context = FTMakerCacheContext.CreateCacheDB();

            var result = new Result()
            {
                Results = FTMGeoCoding.GetUnknownPlaces(context, new OutputHandler(_hubContext)).Take(75)
            };

            return result;
        }
        // POST api/values
        [HttpPost]
        public void Post(PlaceLookup value)
        {
            //Debug.WriteLine("recieved: " + value.PlaceId + " " + value.results);
            var outputHandler = new OutputHandler(_hubContext);
            var cacheDB = FTMakerCacheContext.CreateCacheDB();
            cacheDB.SetPlaceGeoData(value.placeid, value.results);
        }

    }
}
