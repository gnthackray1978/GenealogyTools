using FTMContext;
using FTMContext.Models;
using System.Linq;
using System.Web.Http;

namespace FTMServices.Controllers
{
    public class InfoController : ApiController
    {
        public Info Get(string infoType)
        {
            var returnVal = new Info();

            var count = 0;

            Global.LogMessage("Getting unknown place count");

            switch (infoType)
            {
                case "unknown_places_count":
                    using (var context = FTMakerCacheContext.CreateCacheDB())
                    {
                        count = FTMGeoCoding.GetUnknownPlaces(context, new OutputHandler()).Count();
                    }

                    returnVal = new Info()
                    {
                        RecordCount = count
                    };

                    break;

                //case "destination_contents":
                //    var ftmMakerContext = FTMakerContext.CreateDestinationDB();
                //    var r = ftmMakerContext.DumpCount();

                //    returnVal = new Info()
                //    {
                //        RecordCount = r.Count,
                //        Results = r
                //    };

                //    break;
            }

            return returnVal;

        }
    }
}
