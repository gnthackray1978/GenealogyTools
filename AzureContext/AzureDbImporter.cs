using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AzureContext.Models;
using ConfigHelper;
using FTMContextNet.Data;
using FTMContextNet.Domain.ExtensionMethods;
using LoggingLib;
using PlaceLibNet.Data.Contexts;
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

        public void ImportPlaceCache()
        {
            var p = new PlacesContext(_imsgConfigHelper);

            var a = new AzurePlacesContext(_imsgConfigHelper);

            foreach (var place in p.PlaceCache)
            {
                a.InsertPlaceCache(place.Id,
                                    place.Name,
                                    place.NameFormatted,
                                    place.JSONResult,
                                    place.Country,
                                    place.County,
                                    place.Searched,
                                    place.BadData,
                                    place.Lat,
                                    place.Long,
                                    place.Src);
            }
        }

        public void Import()
        {
            var a = SQLitePersistedCacheContext.Create(_imsgConfigHelper, _console);

            var originDictionary = a.TreeRecord.ToDictionary(p => p.Name, p => p.Id);

            _console.WriteCounter("Emptying DB");

            using (var executor = new AzureDBContext(_imsgConfigHelper.MSGGenDB01))
            {
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeGroups");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeRecordMapGroup");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeRecord");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeImport");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.FTMPersonView");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.DupeEntries");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.IgnoreList");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.PersonOrigins");//
                executor.ExecuteCommand("TRUNCATE TABLE DNA.Relationships");
           //     executor.ExecuteCommand("TRUNCATE TABLE UKP.PlaceCache");//
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
                    FirstName = d.FirstName,
                    Ident = d.Ident,
                    Location = d.Location,
                    PersonId = d.PersonId,
                    ImportId = d.ImportId,
                    UserId = d.UserId
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
                
                if (d.Id == 815)
                {
                    Debug.WriteLine("test: " + d.Id);
                }

                ftmPersonViews.Add(new FTMPersonView()
                    {
                        Id = d.Id,
                        Origin = d.Origin,
                        Surname = d.Surname,
                        FirstName = d.FirstName,
                        PersonId = d.PersonId,
                        FatherId = d.FatherId,
                        MotherId = d.MotherId,
                        AltLat = d.AltLat.ToDecimal(),
                        AltLocation = d.AltLocation,
                        AltLocationDesc = d.AltLocationDesc,
                        AltLong = d.AltLong.ToDecimal(),
                        BirthFrom = d.BirthFrom,
                        BirthLat = d.BirthLat.ToDecimal(),
                        BirthLocation = d.BirthLocation,
                        BirthLong = d.BirthLong.ToDecimal(),
                        BirthTo = d.BirthTo,
                        DirectAncestor = d.DirectAncestor,
                        LinkedLocations = d.LinkedLocations,
                        ImportId = d.ImportId,
                        LinkNode = d.LinkNode,
                        RootPerson = d.RootPerson,
                        UserId = d.UserId,
                    });
               
            }

            AzureDBContext.BulkInsert(_imsgConfigHelper.MSGGenDB01, ftmPersonViews);

            _console.WriteCounter("Adding new tree records");

            foreach (var d in a.TreeRecord)
            {
                destination.TreeRecord.Add(new Models.TreeRecord()
                {
                    Id = d.Id,
                    Origin = d.Origin??"",
                    CM = d.CM,
                    Name = d.Name,
                    PersonCount = d.PersonCount,
                    Located = d.Located,
                    UserId = d.UserId,
                    ImportId = d.ImportId
                });

            }

            destination.SaveChanges(); 

            _console.WriteCounter("Adding new marriages");

            foreach (var d in a.Relationships)
            {
                destination.Relationships.Add(new Models.Relationships()
                {
                    Id = d.Id, 
                    BrideId = d.BrideId,
                    DateStr = d.DateStr,
                    GroomId = d.GroomId,
                    Location = d.Location,
                    Notes = d.Notes,
                    Year = d.Year,
                    Origin = d.Origin,
                    ImportId = d.ImportId,
                    UserId = d.UserId
                });

            }

            destination.SaveChanges();

            _console.WriteCounter("Adding new tree group records");

            foreach (var d in a.TreeGroups)
            {
               // var id = 0;

                //if (!string.IsNullOrEmpty(d.GroupName) && originDictionary.ContainsKey(d.GroupName))
                //    id = originDictionary[d.GroupName];
                Debug.WriteLine(d.Id);
                destination.TreeGroups.Add(new Models.TreeGroups()
                {
                    Id = d.Id,
                    GroupName = d.GroupName,
                    ImportId = d.ImportId,
                    UserId = d.UserId
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
                    GroupName = d.GroupName, 
                    TreeName = d.TreeName,
                    ImportId = d.ImportId,
                    UserId = d.UserId
                });

            }

            destination.SaveChanges();
 
            foreach(var d in a.PersonOrigins)
            {
                destination.PersonOrigins.Add(new Models.PersonOrigins()
                {
                    Id = d.Id,
                    Origin = d.Origin,
                    PersonId = d.PersonId,
                    ImportId = d.ImportId,
                    UserId = d.UserId
                });

            }

            destination.SaveChanges();

            _console.WriteCounter("Adding new tree import records");

            foreach(var d in a.TreeImport){
                destination.TreeImport.Add(new Models.TreeImport()
                {
                    Id = d.Id, 
                    DateImported = d.DateImported, 
                    UserId = d.UserId,
                    FileName = d.FileName 
                               
                               ?? "",
                    Selected = d.Selected,
                    FileSize = d.FileSize??"0"

                    });
            }

            destination.SaveChanges();

            _console.WriteCounter("Finished");
        }

         
    }
}
