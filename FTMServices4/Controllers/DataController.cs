using FTMContext;
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
        private readonly IHubContext<NotificationHub> _hubContext;

        public DataController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }


        // GET api/values
        public IEnumerable<PlaceLookup> Get()
        {
            var outputHandler = new OutputHandler(_hubContext);

            var context = FTMakerCacheContext.CreateCacheDB();

            return FTMGeoCoding.GetUnknownPlaces(context, outputHandler).Take(75);
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
            //Debug.WriteLine(upload.Value);
            var outputHandler = new OutputHandler(_hubContext);
            var cacheDB = FTMakerCacheContext.CreateCacheDB();
            var sourceDB = FTMakerContext.CreateSourceDB();

            switch (upload.Value) {
                case "backupAndDecryptFTMDB":

                   
                    FTMTools.ExtractFTMDB(sourceDB, outputHandler);

                    break;

                case "addResetMissingPlaces":
                    
                    
                    var sourcePlaces = sourceDB.Place.ToList();


                    outputHandler.WriteLine("Adding missing places");



                    FTMGeoCoding.AddMissingPlaces(sourcePlaces, cacheDB, outputHandler);


                    outputHandler.WriteLine("Updating places where required");


                    FTMGeoCoding.ResetUpdatedPlaces(sourcePlaces, cacheDB, outputHandler);
                     
                    outputHandler.WriteLine("Updating Place Names");
                    

                    break;

                case "updatePlaceMetadata":

                    FTMGeoCoding.UpdateFTMCacheMetaData(cacheDB, outputHandler);

                    outputHandler.WriteLine("Finished FTM Meta Data");

                    break;

                case "cleardata":

                    outputHandler.WriteLine("Clearing existing data");

                    cacheDB.DeleteTempData();

                    outputHandler.WriteLine("Finished Deleting data");

                    break;

                case "setOriginPerson":

                    var ftmMostRecentAncestor = new FTMMostRecentAncestor(sourceDB, cacheDB, outputHandler);

                    ftmMostRecentAncestor.MarkMostRecentAncestor();

                    outputHandler.WriteLine("Finished Setting Origin Person");

                    break;


                case "setDateLocPop":

                    var ftmDupe = new FTMViewCreator(sourceDB, cacheDB, outputHandler);

                    ftmDupe.Run();

                    outputHandler.WriteLine("Finished Setting Date Loc Pop");

                    break;
                case "createDupeView":

                    var pg = new PersonGrouper(sourceDB, cacheDB, outputHandler);

                    pg.PopulateDupeEntries();

                    outputHandler.WriteLine("Finished Creating Dupe View");
                    break;
                case "createTreeRecord":
                    FTMTreeRecordCreator ftmTreeRecordCreator = new FTMTreeRecordCreator(sourceDB, cacheDB, outputHandler);

                    ftmTreeRecordCreator.Create();

                    outputHandler.WriteLine("Finished Creating Tree Record View");

                    break;

            }

        }
         
    }
}
