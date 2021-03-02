using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using FTMContext.lib;
using Microsoft.Data.Sqlite;
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

        public string LinkedLocations { get; set; }
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


    public partial class TreeRecord
    {
        public int Id { get; set; }
        public int PersonCount { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }
        public int CM { get; set; }
    
    }

    public partial class FTMPersonOrigin
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Origin { get; set; }
        
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
                Path = @"C:\Users\george\Documents\Software MacKiev\Family Tree Maker\",
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

        public DupeEntry CreateNewDupeEntry(int dupeId, FTMPersonView person, int personId, string ident)
        {

            var dupeEntry = new DupeEntry
            {
                Id = dupeId,
                Ident = ident,
                PersonId = personId,
                BirthYearFrom = person.BirthFrom,
                BirthYearTo = person.BirthTo,
                Origin = person.Origin,
                Location = GoogleGeoCodingHelpers.FormatPlace(person.BirthLocation),
                ChristianName = person.FirstName,
                Surname = person.Surname
            };

            return dupeEntry;
        }


        public virtual DbSet<FTMPersonOrigin> FTMPersonOrigins { get; set; }
        public virtual DbSet<TreeRecord> TreeRecords { get; set; }
        public virtual DbSet<DupeEntry> DupeEntries { get; set; }
        public virtual DbSet<FTMPersonView> FTMPersonView { get; set; }
        public virtual DbSet<FTMPlaceCache> FTMPlaceCache { get; set; }


        public void DeleteTempData()
        {
          

            using (var connection = this.GetCon())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM FTMPersonOrigins";


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                command.CommandText = "DELETE FROM DupeEntries";


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                command.CommandText = "DELETE FROM FTMPersonView";


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                command.CommandText = "DELETE FROM TreeRecord";


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }


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
