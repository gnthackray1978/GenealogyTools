using Castle.Components.DictionaryAdapter;
using FTMContextNet.Data;
using LoggingLib;
using ConfigHelper;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using FTMContextNet.Domain.ExtensionMethods;

namespace FTMContextNet.Tests
{
    public class AzureDBIntegrationTests
    {
        //Test to check we can read and write to the TreeRecordMapGroup table
        [Fact]
        public void TestTreeRecordMapGroupReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

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

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

            var person = new PersonOrigins()
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

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

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

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());    

            var dupe = new DupeEntry{
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
        public void TestInsertGroups()
        {
            var iconfig = new MSGConfigHelper();

            var adbhelper = new AzureDBHelpers(iconfig.MSGGenDB01);

            var c = new AzurePersistedCacheContext(adbhelper, iconfig, new Log());
            
            var id = adbhelper.GetNextId( "TreeGroups");

            c.InsertGroups(id, "test", 2, 1);

            var tg = c.TreeGroups.FirstOrDefault(f => f.Id == id);

            Assert.NotNull(tg);

            bool isEqual = tg != null && tg.Id == id && tg.ImportId == 2 && tg.UserId == 1 && tg.GroupName == "test";

            Assert.True(isEqual);

            if (tg != null)
            {
                c.TreeGroups.Remove(tg);
                c.SaveChanges();
            }

            var tg2 = c.TreeGroups.FirstOrDefault(f => f.Id == id);

            Assert.Null(tg2);
        }

        [Fact]
        public void TestRecordMapGroup()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

            var id =c.InsertRecordMapGroup("testgn","testtn", 2, 1);

            var tg = c.TreeRecordMapGroup.FirstOrDefault(f => f.Id == id);

            Assert.NotNull(tg);

            bool isEqual = tg != null && tg.Id == id && tg.ImportId == 2 && tg.UserId == 1 && tg.GroupName == "testgn" && tg.TreeName == "testtn";

            Assert.True(isEqual);

            if (tg != null)
            {
                c.TreeRecordMapGroup.Remove(tg);
                c.SaveChanges();
            }

            var tg2 = c.TreeRecordMapGroup.FirstOrDefault(f => f.Id == id);

            Assert.Null(tg2);
        }
        
        // function to test BulkInsertTreeRecord method
        [Fact]
        public void TestTreeRecordBulkInsert()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

            var treeRecord = new TreeRecord()
            {
                ImportId = 1,
                Origin = "origin",
                CM = 1,
                Located = true,
                Name = "name",
                PersonCount = 1,
            };

            var treeRecords = new EditableList<TreeRecord> { treeRecord };

            c.BulkInsertTreeRecord(1, treeRecords);

            var tclone = c.TreeRecord.FirstOrDefault(t => t.Id == treeRecord.Id);

            Assert.NotNull(tclone);

            Assert.True(treeRecord.Equals(tclone));

            c.TreeRecord.Remove(tclone);

            c.SaveChanges();

            tclone = c.TreeRecord.FirstOrDefault(t => t.Id == treeRecord.Id);

            Assert.Null(tclone);
        }

        // function to test BulkInsertPersonOrigins method
        [Fact]
        public void TestPersonOriginsBulkInsert()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

           
            var person = new PersonOrigins()
            {
                PersonId = 1,
                Origin = "origin"
            };

            var persons = new EditableList<PersonOrigins> { person };

            c.BulkInsertPersonOrigins( 1,  persons);

            var pclone = c.PersonOrigins.FirstOrDefault(p => p.Id == person.Id);

            Assert.NotNull(pclone);

            Assert.True(person.Equals(pclone));

            c.PersonOrigins.Remove(pclone);

            c.SaveChanges();

            pclone = c.PersonOrigins.FirstOrDefault(p => p.Id == person.Id);

            Assert.Null(pclone);
        }

        [Fact]
        public void TestFTMPersonViewReadWrite()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());
           
            int pk = c.FTMPersonView.Max(m => m.Id);

            pk++;

            var person = new FTMPersonView()
            {
                ImportId = 1,
                AltLat = "1",
                AltLong = "1",
                AltLocation = "altLocation",
                AltLocationDesc = "altLocationDesc",
                BirthFrom = 1500,
                BirthTo = 1600,
                BirthLat = "1",
                BirthLocation = "birthLocation",
                BirthLong = "1",
                DirectAncestor = true,
                FatherId = 1,
                FirstName = "George",
                Id = pk,
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

        [Fact]
        public void TestUpdatePersonLocations()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

            int pk = c.FTMPersonView.Max(m => m.Id);
            
            pk++;

            var person = new FTMPersonView()
            {
                ImportId = 1,
                AltLat = "1",
                AltLong = "1",
                AltLocation = "altLocation",
                AltLocationDesc = "altLocationDesc",
                BirthFrom = 1500,
                BirthTo = 1600,
                BirthLat = "1",
                BirthLocation = "birthLocation",
                BirthLong = "1",
                DirectAncestor = true,
                FatherId = 1,
                FirstName = "George",
                Id = pk,
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


            var d = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());
            d.UpdatePersonLocations(person.Id,"2","3","4","5");


            c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

            var pclone = c.FTMPersonView.FirstOrDefault(f => f.Id == person.Id);

            Assert.NotNull(pclone);

            var isEqual = pclone.AltLat.ToDecimal() == "5".ToDecimal() && pclone.AltLong.ToDecimal() == "4".ToDecimal() && pclone.BirthLat.ToDecimal() == "3".ToDecimal() &&
                          pclone.BirthLong.ToDecimal() == "2".ToDecimal();

            Assert.True(isEqual);

            c.FTMPersonView.Remove(pclone);

            c.SaveChanges();

            pclone = c.FTMPersonView.FirstOrDefault(f => f.Id == person.Id);

            Assert.Null(pclone);
        }

        // function to test BulkInsertMarriages method
        [Fact]
        public void TestMarriagesBulkInsert()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

          
            var marriage = new Relationships()
            {
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

            var marriages = new EditableList<Relationships> { marriage };

            c.BulkInsertMarriages(1, 1, marriages);

            var mclone = c.Relationships.FirstOrDefault(m => m.Id == marriage.Id);

            Assert.NotNull(mclone);

            Assert.True(mclone.Equals(marriage));

            c.Relationships.Remove(mclone);

            c.SaveChanges();

            mclone = c.Relationships.FirstOrDefault(f => f.Id == marriage.Id);

            Assert.Null(mclone);
        }

        // function to test BulkInsertFTMPersonView method
        [Fact]
        public void TestFTMPersonViewBulkInsert()
        {
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

            var person = new FTMPersonView()
            {
                ImportId = 5,
                AltLat = "1",
                AltLong = "1",
                AltLocation = "altLocation",
                AltLocationDesc = "altLocationDesc",
                BirthFrom = 1500,
                BirthTo = 1600,
                BirthLat = "1",
                BirthLocation = "birthLocation",
                BirthLong = "1",
                DirectAncestor = true,
                FatherId = 1,
                FirstName = "George",
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
            
            var persons = new EditableList<FTMPersonView> { person };

            c.BulkInsertFTMPersonView( 1, 1, persons);

            var pclone = c.FTMPersonView.FirstOrDefault(f => f.Id == person.Id);

            Assert.NotNull(pclone);


            Assert.True(person.Equals(pclone));

            c.FTMPersonView.Remove(pclone);

            c.SaveChanges();

            pclone = c.FTMPersonView.FirstOrDefault(f => f.Id == person.Id);

            Assert.Null(pclone);
        }

        //Test to check we can read and write to the Relationships table
        [Fact]
        public void TestRelationshipsReadWrite(){
            var iconfig = new MSGConfigHelper();

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

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

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

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

            var c = new AzurePersistedCacheContext(new AzureDBHelpers(iconfig.MSGGenDB01), iconfig, new Log());

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
        
      
    }
}
