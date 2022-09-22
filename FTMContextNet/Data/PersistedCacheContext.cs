using System.Collections.Generic;
using System.Data.SQLite;
using ConfigHelper;
using FTMContextNet.Data.Interfaces;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data
{

    public partial class PersistedCacheContext : DbContext, IPlace
    {

        private IMSGConfigHelper _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }

        public PersistedCacheContext(IMSGConfigHelper config)
        {
            _configObj = config;
        }

        public PersistedCacheContext(DbContextOptions<FTMakerContext> options)
            : base(options)
        {
        }

        public static PersistedCacheContext Create(IMSGConfigHelper imsgConfigHelper)
        {
            //var a = new FTMakerCacheContext(new ConfigObj
            //{
            //    Path = imsgConfigHelper.CacheData_Path,
            //    FileName = imsgConfigHelper.CacheData_FileName,
            //    IsEncrypted = imsgConfigHelper.CacheData_IsEncrypted
            //});


            return new PersistedCacheContext(imsgConfigHelper);
        }

        public virtual DbSet<FTMPersonOrigin> FTMPersonOrigins { get; set; }
        public virtual DbSet<TreeRecord> TreeRecords { get; set; }
        public virtual DbSet<DupeEntry> DupeEntries { get; set; }
        public virtual DbSet<FTMPersonView> FTMPersonView { get; set; }
        public virtual DbSet<FTMMarriage> FTMMarriages { get; set; }
        public virtual DbSet<FtmPlaceCache> FTMPlaceCache { get; set; }

        public void DeleteOrigins()
        {
            using var connection = GetCon();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM FTMPersonOrigins";

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            this.Database.CloseConnection();
            this.Database.OpenConnection();

        }

        public int BulkInsertFTMPersonOrigins(int nextId, List<int> addedPersons, string origin)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO FTMPersonOrigins(Id, PersonId,Origin)" +
                                  " VALUES ($Id,$PersonId,$Origin);";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$PersonId", SqliteType.Integer);
            command.Parameters.Add("$Origin", SqliteType.Text);
                
            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            var idx = nextId;
            foreach (var row in addedPersons)
            {
                command.Parameters["$Id"].Value = idx;
                command.Parameters["$PersonId"].Value = row;
                command.Parameters["$Origin"].Value = origin;
                        
                command.ExecuteNonQuery();
                idx++;
            }

            transaction.Commit();

            return idx;
        }


        public void DeleteDupes()
        {
            using (var connection = GetCon())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM DupeEntries";

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeletePersons()
        {
            using (var connection = GetCon())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM FTMPersonView";

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteTreeRecords()
        {
            using (var connection = GetCon())
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM TreeRecords";

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteMarriages()
        {


            using (var connection = GetCon())
            {
                var command = connection.CreateCommand();


                command.CommandText = "DELETE FROM FTMMarriages";


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
                                      + "\";synchronous=Off;pooling=False;foreign keys=True;"
                                      + _configObj.FTMConString + ";datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False";
            }
            else
            {

                cs = "data source=\"" + _configObj.CacheData_Path
                                      + _configObj.CacheData_FileName
                                      + "\";pooling=False;foreign keys=True";
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
