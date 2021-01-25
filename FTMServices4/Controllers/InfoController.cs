﻿using FTMContext;
using FTMContext.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace FTMServices4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public InfoController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Info Get(string infoType)
        {
            var returnVal = new Info();

            var count = 0;

            _hubContext.Clients.All.SendAsync("Notify", "hello");

            switch (infoType)
            {
                case "unknown_places_count":
                    using (var context = FTMakerCacheContext.CreateCacheDB())
                    {
                        count = FTMGeoCoding.GetUnknownPlaces(context, new OutputHandler()).Count();
                    }

                    returnVal = new Info()
                    {
                        RecordCount = count
                    };

                    break;

                //case "destination_contents":
                //    var ftmMakerContext = FTMakerContext.CreateDestinationDB();
                //    var r = ftmMakerContext.DumpCount();

                //    returnVal = new Info()
                //    {
                //        RecordCount = r.Count,
                //        Results = r
                //    };

                //    break;
            }

            return returnVal;

        }
    }
}
