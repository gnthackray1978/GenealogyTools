using FTMContext;
using FTMContext.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConfigHelper;
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
        private readonly IMSGConfigHelper _iMSGConfigHelper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public GeoCodeController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
        {
            _hubContext = hubContext;
            _iMSGConfigHelper = iMSGConfigHelper;
        }


        // GET api/values
        [HttpGet]
        public Result Get(string infoType)
        {
            var context = FTMakerCacheContext.CreateCacheDB(_iMSGConfigHelper);

            var result = new Result()
            {
               // Results = FTMGeoCoding.GetUnknownPlaces(context, new OutputHandler(_hubContext)).Take(75)
                Results = FTMGeoCoding.GetUnknownPlacesIgnoreSearchedAlready(context, new OutputHandler(_hubContext)).Take(75)
            };

            return result;
        }
        // POST api/values
        [HttpPost]
        public void Post(PlaceLookup value)
        {
            //Debug.WriteLine("recieved: " + value.PlaceId + " " + value.results);
            var outputHandler = new OutputHandler(_hubContext);
            var cacheDB = FTMakerCacheContext.CreateCacheDB(_iMSGConfigHelper);
            cacheDB.SetPlaceGeoData(value.placeid, value.results);
        }

    }
}
