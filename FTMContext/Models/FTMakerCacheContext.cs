using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using FTMContext.lib;
using Microsoft.EntityFrameworkCore;

namespace FTMContext.Models
{


    public partial class FTMPersonView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public int BirthFrom { get; set; }
        public int BirthTo { get; set; }

        public string BirthLocation { get; set; }
        public double BirthLat { get; set; }
        public double BirthLong { get; set; }

        public string AltLocationDesc { get; set; }
        public string AltLocation { get; set; }
        public double AltLat { get; set; }
        public double AltLong { get; set; }

        public string Origin { get; set; }
        public int PersonId { get; set; }
    }

    public partial class DupeEntry
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Ident { get; set; }
        public string Origin { get; set; }
        public int BirthYearFrom { get; set; }
        public int BirthYearTo { get; set; }
        public string Location { get; set; }
        public string ChristianName { get; set; }
        public string Surname { get; set; }
    }

    public partial class FTMPlaceCache
    {

        public int Id { get; set; }

        public int FTMPlaceId { get; set; }

        public string FTMOrginalName { get; set; }
        public string FTMOrginalNameFormatted { get; set; }
        public string JSONResult { get; set; }
        public string County { get; set; }
        public string Country { get; set; }

    }


    public partial class FTMakerCacheContext : DbContext
    {



        private ConfigObj _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }

        public FTMakerCacheContext(ConfigObj config)
        {
            _configObj = config;
        }

        public FTMakerCacheContext(DbContextOptions<FTMakerContext> options)
            : base(options)
        {
        }

        public List<string> DumpCount()
        {

            List<string> results = new List<string>();

            DumpRecordCount(results, FTMPersonView, "FTMPersonView");
            DumpRecordCount(results, FTMPlaceCache, "FTMPlaceCache");
            DumpRecordCount(results, DupeEntries, "DupeEntries");
  
            return results;
        }

        private void DumpRecordCount<t>(List<string> results, DbSet<t> set, string name) where t : class
        {
            string result = "";

            var count = set.Count();

            if (count > 0)
                result = name + " " + set.Count();

            if (result != "")
                results.Add(result);
        }

        public static FTMakerCacheContext CreateCacheDB()
        {
            var a = new FTMakerCacheContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"cacheData.db",
                IsEncrypted = false
            });

            return a;
        }

    

        public void SetPlaceGeoData(int placeId, string results)
        {
            try
            {
                var cachedPlace = this.FTMPlaceCache.FirstOrDefault(f => f.FTMPlaceId == placeId);

                if (cachedPlace != null)
                {
                    cachedPlace.JSONResult = results;
                    cachedPlace.Country = "";
                    cachedPlace.County = "";
                }

                this.SaveChanges();
                Debug.WriteLine("ID : " + placeId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("failed: " + e.Message);
            }

        }

        public DupeEntry CreateNewDupeEntry(int dupeId, DbSet<Person> Person, List<Fact> facts, int personId, string ident)
        {

            var dupeEntry = new DupeEntry();

            dupeEntry.Id = dupeId;

            var cpFact = FTMTools.GetFact(facts, personId);

            var person = Person.FirstOrDefault(f1 => f1.Id == personId);

            var birthString = "";

            if (cpFact.BirthYearFrom == cpFact.BirthYearTo)
                birthString = cpFact.BirthYearFrom.ToString();
            else
                birthString = cpFact.BirthYearFrom.ToString() + " - " + cpFact.BirthYearTo.ToString();

            string result = cpFact.Origin + " , " + birthString + " , " + GoogleGeoCodingHelpers.FormatPlace(person.BirthPlace) + " , " + person.GivenName + " , " + person.FamilyName;

            dupeEntry.Ident = ident;
            dupeEntry.PersonId = personId;
            dupeEntry.BirthYearFrom = cpFact.BirthYearFrom;
            dupeEntry.BirthYearTo = cpFact.BirthYearTo;
            dupeEntry.Origin = cpFact.Origin;
            dupeEntry.Location = GoogleGeoCodingHelpers.FormatPlace(person.BirthPlace);
            dupeEntry.ChristianName = person.GivenName;
            dupeEntry.Surname = person.FamilyName;

            return dupeEntry;
        }


        public virtual DbSet<DupeEntry> DupeEntries { get; set; }
        public virtual DbSet<FTMPersonView> FTMPersonView { get; set; }
        public virtual DbSet<FTMPlaceCache> FTMPlaceCache { get; set; }
     
        private SQLiteConnection GetCon()
        {

            string cs = "";


            string key = ConString.FTMConString;

            if (_configObj.IsEncrypted)
            {
                //cs = "data source=\"" + _configObj.Path + _configObj.FileName + "\";synchronous=Off;pooling=False;journal mode=Memory;foreign keys=True;"+ key + ";datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";

                cs = "data source=\"" + _configObj.Path + _configObj.FileName + "\";synchronous=Off;pooling=False;journal mode=Memory;foreign keys=True;" + key + ";datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
                _sqlConnection = new System.Data.SQLite.SQLiteConnection(cs);

                _sqlConnection.Flags |= SQLiteConnectionFlags.AllowNestedTransactions;
            }
            else
            {
                //cs = "data source=\"" + _configObj.Path + _configObj.FileName + "\";synchronous=Off;pooling=False;journal mode=Memory;foreign keys=True;datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
                cs = "data source=\"" + _configObj.Path + _configObj.FileName + "\";pooling=False;journal mode=Memory;foreign keys=True;datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
                _sqlConnection = new System.Data.SQLite.SQLiteConnection(cs);

                _sqlConnection.Flags |= SQLiteConnectionFlags.AllowNestedTransactions;
            }

            return _sqlConnection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(GetCon());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
