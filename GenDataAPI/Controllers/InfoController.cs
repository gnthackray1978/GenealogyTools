using System.Linq;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Models.Read;
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
        private readonly FTMFacade _facade;

        public InfoController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
        {
            _hubContext = hubContext;
            _iMSGConfigHelper = iMSGConfigHelper;
            _facade = new FTMFacade(_iMSGConfigHelper, new OutputHandler(hubContext));
        }

        public InfoModel Get(string infoType)
        {       
        //    _hubContext.Clients.All.SendAsync("Notify", "Fetching record counts");
            
            return _facade.GetInfo();

        }
    }
}
