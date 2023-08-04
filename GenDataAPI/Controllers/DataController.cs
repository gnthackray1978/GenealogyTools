using System.Diagnostics;
using System.Threading;
using AzureContext;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.UserServices.CreatePersonLocationsInCache;
using FTMContextNet.Application.UserServices.GetInfoList;
using FTMContextNet.Domain.Commands;
using GenDataAPI.Hub;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenDataAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        
        private readonly IMSGConfigHelper _iMSGConfigHelper;
        private readonly OutputHandler _outputHandler;
        //private readonly FTMFacade _facade;
        private readonly IMediator _mediator;

        public DataController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper, IMediator mediator)
        {
            _iMSGConfigHelper = iMSGConfigHelper;

            _outputHandler = new OutputHandler(hubContext);
           // _facade = new FTMFacade(_iMSGConfigHelper, _outputHandler);
            _mediator = mediator;
        }
        
        [HttpPost]
        [Route("/data/persons/locations")]
        public IActionResult AddPersonLocations()
        {
            return this.ConvertResult(_mediator
                .Send(new CreatePersonLocationsCommand(), new CancellationToken(false)).Result);
        }

        [HttpPut]
        [Route("/data/persons/locations")]
        public IActionResult UpdatePersonLocations()
        {
            return this.ConvertResult(_mediator
                .Send(new UpdatePersonLocationsCommand(), new CancellationToken(false)).Result);
        }

        [HttpPost]
        [Route("/data/persons/add")]
        public IActionResult AddPersons()
        {
            return this.ConvertResult(_mediator
                .Send(new CreatePersonAndRelationshipsCommand(), new CancellationToken(false)).Result);
        }

        [HttpPost]
        [Route("/data/dupes")]
        public IActionResult AddDupes()
        {
            return this.ConvertResult(_mediator
                .Send(new CreateDuplicateListCommand(), new CancellationToken(false)).Result);
        }



        [HttpPost]
        [Route("/data/azure")]
        public IActionResult AddAzure()
        {
            var az = new AzureDbImporter(_outputHandler, _iMSGConfigHelper);
            _outputHandler.WriteLine("Importing into azure DB");
            az.Import();
            _outputHandler.WriteLine("Finished importing into azure DB");

            return Ok(true);
        }
         
         
    }
}
