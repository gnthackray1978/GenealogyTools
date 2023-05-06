using System.Collections.Generic;
using System.Data.SQLite;
using ConfigHelper; 
using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data
{

    public partial class PersistedCacheContext : DbContext
    {

        private IMSGConfigHelper _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }

        public PersistedCacheContext(IMSGConfigHelper config)
        {
            _configObj = config;
        }

        public PersistedCacheContext(DbContextOptions<PersistedCacheContext> options)
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
        public virtual DbSet<TreeGroups> TreeGroups { get; set; }
        public virtual DbSet<TreeRecordMapGroup> TreeRecordMapGroup { get; set; }
        public virtual DbSet<DupeEntry> DupeEntries { get; set; }
        public virtual DbSet<FTMPersonView> FTMPersonView { get; set; }
        public virtual DbSet<FTMMarriage> FTMMarriages { get; set; }

        public virtual DbSet<FTMImport> FTMImport { get; set; }

        public virtual DbSet<IgnoreList> IgnoreList { get; set; }

        public void UpdateFTMPlaceCache(int placeId, string results)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE FTMPlaceCache SET JSONResult = $JSONResult, Country = '', County = '', Searched = 1, BadData = 1 WHERE FTMPlaceId = $FTMPlaceId;";

            command.Parameters.Add("$FTMPlaceId", SqliteType.Integer);
            command.Parameters.Add("$JSONResult", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
          
            command.Parameters["$FTMPlaceId"].Value = placeId;
            command.Parameters["$JSONResult"].Value = results; 
            
            command.ExecuteNonQuery();
          
            transaction.Commit();
            
        }

        public int BulkInsertFTMPersonOrigins(int nextId, Dictionary<int, bool> addedPersons, string origin, string fullName ="")
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO FTMPersonOrigins(Id, PersonId,Origin, DirectAncestor)" +
                                  " VALUES ($Id,$PersonId,$Origin, $DirectAncestor);";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$PersonId", SqliteType.Integer);
            command.Parameters.Add("$Origin", SqliteType.Text);
            command.Parameters.Add("$DirectAncestor", SqliteType.Integer);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            var idx = nextId;
            foreach (var row in addedPersons)
            {
                command.Parameters["$Id"].Value = idx;
                command.Parameters["$PersonId"].Value = row.Key;
                command.Parameters["$Origin"].Value = fullName.ToLower().Contains("group") ? fullName : origin;
                command.Parameters["$DirectAncestor"].Value = row.Value;
                command.ExecuteNonQuery();
                idx++;
            }

            transaction.Commit();

            return idx;
        }

        public int InsertGroups(int nextId, string groupName)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO TreeGroups(Id, GroupName) VALUES ($Id,$GroupName);";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$GroupName", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            
            command.Parameters["$Id"].Value = nextId;
            command.Parameters["$GroupName"].Value = groupName;
            command.ExecuteNonQuery();
              
            transaction.Commit();

            return nextId;
        }
        public int InsertRecordMapGroup(int nextId, string groupName, string treeName)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO TreeRecordMapGroup(Id, TreeName, GroupName) VALUES ($Id,$TreeName,$GroupName);";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$TreeName", SqliteType.Text);
            command.Parameters.Add("$GroupName", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = nextId;
            command.Parameters["$GroupName"].Value = groupName;
            command.Parameters["$TreeName"].Value = treeName;
            command.ExecuteNonQuery();

            transaction.Commit();

            return nextId;
        }


        #region delete commands

        public void DeleteOrigins()
        {
            RunCommand("DELETE FROM FTMPersonOrigins");
        }

        public void DeleteDupes()
        {
            RunCommand("DELETE FROM DupeEntries");
        }

        public void DeletePersons(int importId)
        {
            RunCommand("DELETE FROM FTMPersonView WHERE ImportId = " + importId);
        }

        public void DeleteTreeRecords()
        {
            RunCommand("DELETE FROM TreeRecords");
        }

        public void DeleteMarriages(int importId)
        {
            RunCommand("DELETE FROM FTMMarriages WHERE ImportId = " + importId); ;
        }

        public void DeleteImports(int importId)
        {
            RunCommand("DELETE FROM FTMImport WHERE Id = " + importId); ;
        }

        public void DeleteTreeGroups()
        {
            RunCommand("DELETE FROM TreeGroups");
        }


        public void DeleteRecordMapGroups()
        {
            RunCommand("DELETE FROM TreeRecordMapGroup");
        }

        #endregion

        private void RunCommand(string commandText)
        {
            using var connection = GetCon();

            var command = connection.CreateCommand();


            command.CommandText = commandText;


            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            this.Database.CloseConnection();
            this.Database.OpenConnection();
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
