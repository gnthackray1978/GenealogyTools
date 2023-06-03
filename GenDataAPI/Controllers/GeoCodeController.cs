using System.Collections.Generic;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Models.Read;
using GenDataAPI.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Application.Models.Write;

namespace GenDataAPI.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class GeoCodeController : ControllerBase
    {
        private readonly IMSGConfigHelper _iMSGConfigHelper;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly FTMFacade _facade;

        public GeoCodeController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
        {
            _hubContext = hubContext;
            _iMSGConfigHelper = iMSGConfigHelper;
            _facade = new FTMFacade(_iMSGConfigHelper, new OutputHandler(_hubContext));
        }


        // GET api/values
        [HttpGet]
        public ResultModel Get(string infoType)
        {         
            var result = new ResultModel()
            {
                Results = _facade.GetPlaceNotGeocoded(300)
            };

            return result;
        }
        // POST api/values
        [HttpPost]
        public void Post(GeoCodeResultModel value)
        {
            _facade.UpdatePlaceGeoData(value);
        }

        [HttpPut]
        public IActionResult Put()
        {
            _facade.UpdatePlaceMetaData();

            return Ok(true);
        }
    }
}
