using FTMContext;
using FTMContext.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    }

    public class InfoController : ApiController
    {
        public Info Get()
        {
            var count = FTMGeoCoding.GetUnknownPlaces().Count();

            return new Info() {
                RecordCount = count
            };
        }
    }

    public class DataController : ApiController
    {
        // GET api/values
        public IEnumerable<PlaceLookup> Get()
        {
            return FTMGeoCoding.GetUnknownPlaces().Take(75);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]PlaceLookup value)
        {
            Debug.WriteLine("recieved: " + value.PlaceId + " " + value.results);

            FTMakerContext.SetPlaceGeoData(value.PlaceId,value.results);
           
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
