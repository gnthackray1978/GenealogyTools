using System.Threading;
using System.Threading.Tasks;
using AzureContext;
using ConfigHelper;
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
        private readonly IMediator _mediator;

        public DataController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper, IMediator mediator)
        {
            _iMSGConfigHelper = iMSGConfigHelper;
            _outputHandler = new OutputHandler(hubContext);
            _mediator = mediator;
        }
        
        [HttpPost]
        [Route("/data/persons/locations")]
        public async Task<IActionResult> AddPersonLocations()
        {
            var r = await _mediator
                .Send(new CreatePersonLocationsCommand()
                    , new CancellationToken(false));

            return this.ConvertResult(r);
        }

        [HttpPut]
        [Route("/data/persons/locations")]
        public async Task<IActionResult> UpdatePersonLocations()
        {
            var r = await _mediator
                .Send(new UpdatePersonLocationsCommand()
                    , new CancellationToken(false));
            
            return this.ConvertResult(r);
        }

        [HttpPost]
        [Route("/data/persons/add")]
        public async Task<IActionResult> AddPersons()
        {
            var r = await _mediator
                .Send(new CreatePersonAndRelationshipsCommand()
                    , new CancellationToken(false));

            return this.ConvertResult(r);
        }
        
        [HttpPost]
        [Route("/data/dupes")]
        public async Task<IActionResult> AddDupes()
        {
            var r = await _mediator
                .Send(new CreateDuplicateListCommand(), new CancellationToken(false));

            return this.ConvertResult(r);
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
