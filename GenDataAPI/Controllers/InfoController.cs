using System.Linq;
using ConfigHelper;
using FTMContext;
using FTMContext.Models;
using GenDataAPI.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenDataAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMSGConfigHelper _iMSGConfigHelper;

        public InfoController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
        {
            _hubContext = hubContext;
            _iMSGConfigHelper = iMSGConfigHelper;
        }

        public Info Get(string infoType)
        {
            var returnVal = new Info();

            var count = 0;

            _hubContext.Clients.All.SendAsync("Notify", "hello");

            switch (infoType)
            {
                case "unknown_places_count":
                    using (var context = FTMakerCacheContext.CreateCacheDB(_iMSGConfigHelper))
                    {
                        count = FTMGeoCoding.GetUnknownPlaces(context, new OutputHandler()).Count();
                    }

                    returnVal = new Info()
                    {
                        RecordCount = count
                    };

                    break;
                     
            }

            return returnVal;

        }
    }
}
