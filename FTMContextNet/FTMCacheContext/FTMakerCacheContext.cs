using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using ConfigHelper;
using FTMContext.lib;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FTMContext.Models
{
    public partial class FTMakerCacheContext : DbContext
    {



        private IMSGConfigHelper _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }

        public FTMakerCacheContext(IMSGConfigHelper config)
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

        public static FTMakerCacheContext CreateCacheDB(IMSGConfigHelper imsgConfigHelper)
        {
            //var a = new FTMakerCacheContext(new ConfigObj
            //{
            //    Path = imsgConfigHelper.CacheData_Path,
            //    FileName = imsgConfigHelper.CacheData_FileName,
            //    IsEncrypted = imsgConfigHelper.CacheData_IsEncrypted
            //});

          
            return new FTMakerCacheContext(imsgConfigHelper);
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
                    cachedPlace.Searched = true;
                }

                this.SaveChanges();
                Debug.WriteLine("ID : " + placeId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("failed: " + e.Message);
            }

        }

        public DupeEntry CreateNewDupeEntry(int dupeId, FTMPersonView person, string ident)
        {

            var dupeEntry = new DupeEntry
            {
                Id = dupeId,
                Ident = ident,
                PersonId = person.PersonId,
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
        public virtual DbSet<FtmPlaceCache> FTMPlaceCache { get; set; }


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

                command.CommandText = "DELETE FROM TreeRecords";


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }


        private SQLiteConnection GetCon()
        {

            string cs = "";


            //string key = ConString.FTMConString;

            if (_configObj.CacheData_IsEncrypted)
            {
                cs = "data source=\"" + _configObj.CacheData_Path 
                                      + _configObj.CacheData_FileName 
                                      + "\";synchronous=Off;pooling=False;journal mode=Memory;foreign keys=True;" 
                                      + _configObj.FTMConString + ";datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
            }
            else
            {
                
                cs = "data source=\"" + _configObj.CacheData_Path 
                                      + _configObj.CacheData_FileName 
                                      + "\";pooling=False;journal mode=Memory;foreign keys=True;datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
            }

            _sqlConnection = new SQLiteConnection(cs);

            _sqlConnection.Flags |= SQLiteConnectionFlags.AllowNestedTransactions;

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
