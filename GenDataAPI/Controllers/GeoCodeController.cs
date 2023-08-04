using System.Collections.Generic;
using ConfigHelper;
using FTMContextNet;
using GenDataAPI.Hub;
using MediatR;
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
        private readonly IMediator _mediator;

        public GeoCodeController(IHubContext<NotificationHub> hubContext,
            IMSGConfigHelper iMSGConfigHelper, IMediator mediator)
        {
            _hubContext = hubContext;
            _iMSGConfigHelper = iMSGConfigHelper;
            _mediator = mediator;
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
            _facade.WriteGeoCodedData(value);
        }

        [HttpPut]
        public IActionResult Put()
        {
            _facade.UpdatePlaceMetaData();

            return Ok(true);
        }
        
    }
}
