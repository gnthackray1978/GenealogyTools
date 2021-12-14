using System;
using AzureContext.Models;
using ConfigHelper;
using ConsoleTools;
using FTMContext.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AzureContext
{
    public class AzureDbImporter
    {
        private IMSGConfigHelper _imsgConfigHelper;
        private IConsoleWrapper _console;

        public AzureDbImporter(IConsoleWrapper console, IMSGConfigHelper imsgConfigHelper)
        {
            _console = console;
            _imsgConfigHelper = imsgConfigHelper;
        }

        public void Import()
        {
            var a = FTMakerCacheContext.CreateCacheDB(_imsgConfigHelper);

            _console.WriteCounter("Emptying TreeRecord FTMPersonView and DupeEntries");

            using (var executor = new AzureDBContext(_imsgConfigHelper.MSGGenDB01))
            {
                executor.ExecuteCommand("TRUNCATE TABLE DNA.TreeRecord");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.FTMPersonView");
                executor.ExecuteCommand("TRUNCATE TABLE DNA.DupeEntries");
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


            _console.WriteCounter("Adding new FTM Person view");

            foreach (var d in a.FTMPersonView)
            {
                destination.FTMPersonView.Add(new Models.FTMPersonView()
                {
                    Id = d.Id,
                    Origin = d.Origin??"".Replace(" ",""),
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
                    BirthTo = d.BirthTo
                });

            }

            destination.SaveChanges();

            _console.WriteCounter("Adding new tree records");

            foreach (var d in a.TreeRecords)
            {
                destination.TreeRecord.Add(new Models.TreeRecord()
                {
                    Id = d.Id,
                    Origin = d.Origin??"".Replace(" ", ""),
                    CM = d.CM,
                    Name = d.Name,
                    PersonCount = d.PersonCount,
                    Located = d.Located
                });

            }

            destination.SaveChanges();

        }

         
    }
}
