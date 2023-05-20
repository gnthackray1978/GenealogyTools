using System;
using System.Collections.Generic;
using System.Linq;
using AzureContext.Models;
using ConfigHelper;
using FTMContextNet.Data;
using LoggingLib;
using FTMPersonView = AzureContext.Models.FTMPersonView;

namespace AzureContext
{
    public class AzureDbImporter
    {
        private IMSGConfigHelper _imsgConfigHelper;
        private Ilog _console;

        public AzureDbImporter(Ilog console, IMSGConfigHelper imsgConfigHelper)
        {
            _console = console;
            _imsgConfigHelper = imsgConfigHelper;
        }

        public void Import()
        {

            


            var a = PersistedCacheContext.Create(_imsgConfigHelper, _console);

            var originDictionary = a.TreeRecords.ToDictionary(p => p.Name, p => p.Id);

            _console.WriteCounter("Emptying TreeRecord FTMPersonView and DupeEntries");

            using (var executor = new AzureDBContext(_imsgConfigHelper.MSGGenDB01))
            {
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeGroups");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeRecordMapGroup");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeRecord");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.FTMPersonView");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.DupeEntries");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.Relationships");
            }

            _console.WriteCounter("Adding new dupes");

            var destination = new AzureDBContext(_imsgConfigHelper.MSGGenDB01);

            foreach (var d in a.DupeEntries)
            {
                destination.DupeEntries.Add(new Models.DupeEntry()
                {
                    Id = d.Id,
                    Origin = d.Origin,
                    Surname = d.Surname,
                    BirthYearFrom = d.BirthYearFrom,
                    BirthYearTo = d.BirthYearTo,
                    FirstName = d.ChristianName,
                    Ident = d.Ident,
                    Location = d.Location,
                    PersonId = d.PersonId
                });

            }

            destination.SaveChanges();


            _console.WriteCounter("Adding " + a.FTMPersonView.Count() + " FTM Person view records ");


            List<FTMPersonView> ftmPersonViews = new List<FTMPersonView>();

            foreach (var d in a.FTMPersonView)
            {
                var id = 0;

                if (!string.IsNullOrEmpty(d.Origin) && originDictionary.ContainsKey(d.Origin))
                    id = originDictionary[d.Origin];

                ftmPersonViews.Add(new FTMPersonView()
                {
                    Id = d.Id,
                    Origin = id,
                    Surname = d.Surname,
                    FirstName = d.FirstName,
                    PersonId = d.PersonId,
                    FatherId = d.FatherId,
                    MotherId = d.MotherId,
                    AltLat = d.AltLat,
                    AltLocation = d.AltLocation,
                    AltLocationDesc = d.AltLocationDesc,
                    AltLong = d.AltLong,
                    BirthFrom = d.BirthFrom,
                    BirthLat = d.BirthLat,
                    BirthLocation = d.BirthLocation,
                    BirthLong = d.BirthLong,
                    BirthTo = d.BirthTo,
                    DirectAncestor = d.DirectAncestor
                });
                
            }

            AzureDBContext.BulkInsert(_imsgConfigHelper.MSGGenDB01, ftmPersonViews);

            _console.WriteCounter("Adding new tree records");

            foreach (var d in a.TreeRecords)
            {
                destination.TreeRecord.Add(new Models.TreeRecord()
                {
                    ID = d.Id,
                    Origin = d.Origin??"",
                    CM = d.CM,
                    Name = d.Name,
                    PersonCount = d.PersonCount,
                    Located = d.Located
                });

            }

            destination.SaveChanges(); 

            _console.WriteCounter("Adding new marriages");

            foreach (var d in a.FTMMarriages)
            {
                destination.Relationships.Add(new Models.Relationships()
                {
                    Id = d.Id, 
                    BrideId = d.BrideId,
                    DateStr = d.MarriageDateStr,
                    GroomId = d.GroomId,
                    Location = d.MarriageLocation,
                    Notes = d.Notes,
                    Year = d.MarriageYear,
                    Origin = d.Origin
                    
                });

            }

            destination.SaveChanges();

            _console.WriteCounter("Adding new tree group records");

            foreach (var d in a.TreeGroups)
            {
                var id = 0;

                if (!string.IsNullOrEmpty(d.GroupName) && originDictionary.ContainsKey(d.GroupName))
                    id = originDictionary[d.GroupName];

                destination.TreeGroups.Add(new Models.TreeGroups()
                {
                    Id = d.Id,
                    GroupName = d.GroupName,
                    GroupId = id
                });

            }

            destination.SaveChanges();

            _console.WriteCounter("Adding new tree records map groups");

            foreach (var d in a.TreeRecordMapGroup)
            {
                var groupId = 0;

                if (!string.IsNullOrEmpty(d.GroupName) && originDictionary.ContainsKey(d.GroupName))
                    groupId = originDictionary[d.GroupName];

                var treeId = 0;

                if (!string.IsNullOrEmpty(d.TreeName) && originDictionary.ContainsKey(d.TreeName))
                    treeId = originDictionary[d.TreeName];

                destination.TreeRecordMapGroup.Add(new Models.TreeRecordMapGroup()
                {
                    Id = d.Id, 
                    GroupId = groupId,
                    TreeId = treeId
                });

            }

            destination.SaveChanges();

        }

         
    }
}
