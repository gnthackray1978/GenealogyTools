using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using ConfigHelper;
using FTMContextNet.Data;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Domain.Entities.Persistent;

namespace PlaceLibNet.Tests
{
    public class AzureDBIntegationTests
    {
        [Fact]
        public void TestTreeRecordBulkInsert()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePlacesContext( iconfig);

            var placeid = c.GetNextId("PlaceCache");

            var pc = new PlaceCache()
            {
                Id = placeid,
                AltId = placeid,
                County = "county",
                Country = "test",
                BadData = false,
                JSONResult = "jsonresult",
                Lat = "0",
                Long = "0",
                Name = "name",
                NameFormatted = "nameformatted",
                Searched = false,
                Src = "src"
            };

          
            c.InsertPlaceCache(pc.Id,pc.Name,pc.NameFormatted,pc.JSONResult,pc.Country,
                pc.County,pc.Searched,pc.BadData,pc.Lat,pc.Long,pc.Src);

            var tclone = c.PlaceCache.FirstOrDefault(t => t.Id == pc.Id);

            Assert.NotNull(tclone);

            Assert.True(pc.Equals(tclone));

            c.PlaceCache.Remove(tclone);

            c.SaveChanges();

            tclone = c.PlaceCache.FirstOrDefault(t => t.Id == pc.Id);

            Assert.Null(tclone);
        }
    }
}
