using ConsoleTools;
using FTMContext;
using FTMContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FTMServices.Controllers
{

    public class Upload
    {
        public string Value { get; set; }       
    }

    public class Info {
        public int RecordCount { get; set; }
        public List<string> Results { get; set; }
    }

    public class DataController : ApiController
    {
        // GET api/values
        public IEnumerable<PlaceLookup> Get()
        {
            var outputHandler = new OutputHandler();

            var context = FTMakerCacheContext.CreateCacheDB();

            return FTMGeoCoding.GetUnknownPlaces(context, outputHandler).Take(75);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]Upload upload)
        {
            //Debug.WriteLine(upload.Value);
            var outputHandler = new OutputHandler();
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

                case "setOriginPerson":

                    var ftmMostRecentAncestor = new FTMMostRecentAncestor(sourceDB, outputHandler);

                    ftmMostRecentAncestor.MarkMostRecentAncestor();

                    outputHandler.WriteLine("Finished set");

                    break;
                case "setDateLocPop":

                    var ftmDupe = new FTMDupeDataMethods(sourceDB, cacheDB, outputHandler);

                    ftmDupe.Run();

                    break;
                case "createDupeView":

                    var pg = new PersonGrouper(sourceDB, cacheDB, outputHandler);

                    pg.PopulateDupeEntries();
                    break;

                    //

            }

        }
         
    }
}
