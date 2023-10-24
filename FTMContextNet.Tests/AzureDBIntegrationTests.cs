using FTMContextNet.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTMContextNet.Data;
using LoggingLib;
using ConfigHelper;
using FTMContextNet.Domain.Entities.Persistent.Cache;

namespace FTMContextNet.Tests
{
    

    public class AzureDBIntegrationTests
    {

        //Test to check we can read and write to the TreeRecordMapGroup table
        [Fact]
        public void TestTreeRecordMapGroupReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var trmg = new TreeRecordMapGroup()
            {
                Id = 999999,
                TreeName  = "treename",
                GroupName = "groupname",
                ImportId = 1,
                UserId = 1                
            };

            c.TreeRecordMapGroup.Add(trmg);

            c.SaveChanges();

            var trmgclone = c.TreeRecordMapGroup.FirstOrDefault(t=>t.Id == trmg.Id);

            Assert.NotNull(trmgclone);

            Assert.True(trmg.Equals(trmgclone));

            c.TreeRecordMapGroup.Remove(trmgclone);

            c.SaveChanges();

            trmgclone = c.TreeRecordMapGroup.FirstOrDefault(t=>t.Id == trmg.Id);

            Assert.Null(trmgclone);

        }

        //Test to check we can read and write to the PersonOrigins table
        [Fact]
        public void TestPersonOriginsReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var person = new PersonOrigin()
            {
                Id = 999999,
                PersonId = 1,
                Origin = "origin"
            };

            c.PersonOrigins.Add(person);

            c.SaveChanges();

            var pclone = c.PersonOrigins.FirstOrDefault(p=>p.Id == person.Id);

            Assert.NotNull(pclone);

            Assert.True(person.Equals(pclone));

            c.PersonOrigins.Remove(pclone);

            c.SaveChanges();

            pclone = c.PersonOrigins.FirstOrDefault(p=>p.Id == person.Id);

            Assert.Null(pclone);

        }

        //Test to check we can read and write to the IgnoreList table
        [Fact]
        public void TestIgnoreListReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var ignore = new IgnoreList()
            { 
                Id  = 999999,
                Person1 = "person1",
                Person2 = "person2"
            };

            c.IgnoreList.Add(ignore);

            c.SaveChanges();

            var iclone = c.IgnoreList.FirstOrDefault(i=>i.Id == ignore.Id);

            Assert.NotNull(iclone);

            Assert.True(ignore.Equals(iclone));

            c.IgnoreList.Remove(iclone);

            c.SaveChanges();

            iclone = c.IgnoreList.FirstOrDefault(i=>i.Id == ignore.Id);

            Assert.Null(iclone);

        }
       
        //Test to check we can read and write to the DupeEntries table
        [Fact]
        public void TestDupeEntriesReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());    

            var dupe = new DupeEntry(){
                Id = 999999, 
                PersonId = 1, 
                UserId = 1,
                BirthYearFrom = 1500,
                BirthYearTo = 1600,
                FirstName = "George",
                Surname = "Smith",
                Ident = "ident",
                ImportId = 1,
                Location = "location",
                Origin = "origin"
            };

            c.DupeEntries.Add(dupe);

            c.SaveChanges();

            var dclone = c.DupeEntries.FirstOrDefault(d=>d.Id == dupe.Id);

            Assert.NotNull(dclone);

            Assert.True(dupe.Equals(dclone));

            c.DupeEntries.Remove(dclone);

            c.SaveChanges();

            dclone = c.DupeEntries.FirstOrDefault(d=>d.Id == dupe.Id);

            Assert.Null(dclone);

        }   
        
        [Fact]
        public void TestFTMPersonViewReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var person = new FTMPersonView()
            {
                ImportId = 1,
                AltLat = 1,
                AltLong = 1,
                AltLocation = "altLocation",
                AltLocationDesc = "altLocationDesc",
                BirthFrom = 1500,
                BirthTo = 1600,
                BirthLat = 1,
                BirthLocation = "birthLocation",
                BirthLong = 1,
                DirectAncestor = true,
                FatherId = 1,
                FirstName = "George",
                Id = 999999,
                UserId = 1,
                LinkNode = false,
                LinkedLocations = "linkedLocations",
                LocationsCached = false,
                MotherId = 1,
                Origin = "origin",
                PersonId = 1,
                RootPerson = true,
                Surname = "surname"
            };

            c.FTMPersonView.Add(person);

            c.SaveChanges();

            var pclone = c.FTMPersonView.FirstOrDefault(f=>f.Id == person.Id);

            Assert.NotNull(pclone);


            Assert.True(person.Equals(pclone));

            c.FTMPersonView.Remove(pclone);

            c.SaveChanges();

            pclone = c.FTMPersonView.FirstOrDefault(f=>f.Id == person.Id);

            Assert.Null(pclone);
        }
         
        //Test to check we can read and write to the Relationships table
        [Fact]
        public void TestRelationshipsReadWrite(){
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var rel = new Relationships()
            {
                Id = 999999,
                BrideId = 1,
                GroomId = 1,
                DateStr = "dateStr",
                ImportId = 1,
                Location = "location",
                Notes = "notes",
                Origin = "origin",
                UserId = 1,
                Year = 1500
            };

            c.Relationships.Add(rel);

            c.SaveChanges();

            var rclone = c.Relationships.FirstOrDefault(r=>r.Id == rel.Id);

            Assert.NotNull(rclone);

            Assert.True(rel.Equals(rclone));

            c.Relationships.Remove(rclone);

            c.SaveChanges();

            rclone = c.Relationships.FirstOrDefault(r=>r.Id == rel.Id);

            Assert.Null(rclone);
        }

        //Test to check we can read and write to the TreeGroups table
        [Fact]
        public void TestTreeGroupsReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var tg = new TreeGroups()
            {
                Id = 999999,
                GroupName = "groupName",
                ImportId = 1,
                UserId = 1
            };

            c.TreeGroups.Add(tg);

            c.SaveChanges();

            var tgclone = c.TreeGroups.FirstOrDefault(t=>t.Id == tg.Id);

            Assert.NotNull(tgclone);

            Assert.True(tg.Equals(tgclone));

            c.TreeGroups.Remove(tgclone);

            c.SaveChanges();

            tgclone = c.TreeGroups.FirstOrDefault(t=>t.Id == tg.Id);

            Assert.Null(tgclone);
        }

        //Test to check we can read and write to the TreeImport table
        [Fact]
        public void TestTreeImportReadWrite(){
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var ti = new TreeImport(){
                Id = 999999,
                DateImported = "dateImported",
                FileName = "fileName",
                FileSize = "1",
                Selected = true,
                UserId = 1
            };
            

            c.TreeImport.Add(ti);

            c.SaveChanges();

            var ticlone = c.TreeImport.FirstOrDefault(t=>t.Id == ti.Id);

            Assert.NotNull(ticlone);

            Assert.True(ti.Equals(ticlone));

            c.TreeImport.Remove(ticlone);

            c.SaveChanges();

            ticlone = c.TreeImport.FirstOrDefault(t=>t.Id == ti.Id);

            Assert.Null(ticlone);
        }

        [Fact]
        public void QueryTable_DataReturned_WhenMethodCalled()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(iconfig, new Log());

            var r = c.FTMPersonView.FirstOrDefault();


          var d = c.DupeEntries.FirstOrDefault();

     //   var i = c.IgnoreList.FirstOrDefault();


          //  var p = c.PersonOrigins.FirstOrDefault();

          //var r = c.Relationships.FirstOrDefault();

         // var tr = c.TreeGroups.FirstOrDefault();

        // var ti = c.TreeImport.FirstOrDefault();

       // var tr = c.TreeRecord.FirstOrDefault();

            var trm =c.TreeRecordMapGroup.FirstOrDefault();

        }
    
    }
}
