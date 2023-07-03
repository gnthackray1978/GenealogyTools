using System.Collections.Generic;
using System.Linq;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Models.Read;
using GenDataAPI.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlaceLibNet.Application.Models.Read;

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

        [HttpGet]
        [Route("/info/people")]
        public InfoModel GetPeopleInfo()
        {
            return _facade.GetInfo();
        }

        [HttpGet]
        [Route("/info/places")]
        public PlaceInfoModel GetPlaceInfo()
        {
            return _facade.GetPlaceInfo();
        }

        [HttpGet]
        [Route("/info/gedfiles")]
        public IEnumerable<ImportModel> GetGedFileInfo()
        {
            return _facade.ReadImports();
        }
    }
}
