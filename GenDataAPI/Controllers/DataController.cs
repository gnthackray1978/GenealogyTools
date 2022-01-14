using System.Collections.Generic;
using System.Linq;
using AzureContext;
using ConfigHelper;
using FTMContext;
using FTMContext.Models;
using FTMContextNet;
using GenDataAPI.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenDataAPI.Controllers
{

    public class Upload
    {
        public string Value { get; set; }       
    }

    public class Info {
        public int RecordCount { get; set; }
        public List<string> Results { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        
        private readonly IMSGConfigHelper _iMSGConfigHelper;
        private readonly OutputHandler _outputHandler;
        private readonly FTMFacade _facade;

        public DataController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
        { 
            _iMSGConfigHelper = iMSGConfigHelper;
            _outputHandler = new OutputHandler(hubContext);
            _facade = new FTMFacade(_iMSGConfigHelper, _outputHandler);
        }
        
        
        // GET api/values
        public IEnumerable<PlaceLookup> Get()
        {
            return _facade.GetUnknownPlaces(75);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        // POST api/values
        public void Post(Upload upload)
        {
            


            switch (upload.Value) {
             
                case "addResetMissingPlaces":

                    _facade.UpdateMissingPlaces();
                    break;

                case "updatePlaceMetadata":

                    _facade.UpdatePlaceMetaData();
                    break;

                case "cleardata":

                    _facade.ClearData();
                    break;

                case "setOriginPerson":

                    _facade.SetOriginPerson();
                    break;


                case "setDateLocPop":
                    _facade.SetDateLocPop();
                    break;
                case "createDupeView":
                    _facade.CreateDupeView();
                    break;
                case "createTreeRecord":
                    _facade.CreateTreeRecord();
                    break;

                case "azureimport":
                    var az = new AzureDbImporter(_outputHandler, _iMSGConfigHelper);
                    _outputHandler.WriteLine("Importing into azure DB");
                    az.Import();
                    _outputHandler.WriteLine("Finished importing into azure DB");
                    break;

            }
        }
         
    }
}
