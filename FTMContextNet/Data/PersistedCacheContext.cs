using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection.Metadata;
using ConfigHelper; 
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data
{
    public interface IPersistedCacheContext
    {
        DbSet<FTMPersonOrigin> FTMPersonOrigins { get; set; }
        DbSet<TreeRecord> TreeRecords { get; set; }
        DbSet<TreeGroups> TreeGroups { get; set; }
        DbSet<TreeRecordMapGroup> TreeRecordMapGroup { get; set; }
        DbSet<DupeEntry> DupeEntries { get; set; }
        DbSet<FTMPersonView> FTMPersonView { get; set; }
        DbSet<FTMMarriage> FTMMarriages { get; set; }
        DbSet<FTMImport> FTMImport { get; set; }
        DbSet<IgnoreList> IgnoreList { get; set; }
        int BulkInsertMarriages(int nextId, int importId, List<FTMMarriage> marriages);
        int BulkInsertFTMPersonView(int nextId, int importId, List<FTMPersonView> ftmPersonViews);
        int BulkInsertFTMPersonOrigins(int nextId,int importId, List<FTMPersonOrigin> origins);

        int BulkInsertTreeRecords(List<TreeRecord> treeRecords);
        int InsertGroups(int nextId, string groupName);
        int InsertRecordMapGroup(int nextId, string groupName, string treeName);
        void DeleteOrigins();
        void DeleteDupes();
        void DeletePersons(int importId);
        void DeleteTreeRecords();
        void DeleteMarriages(int importId);
        void DeleteImports(int importId);
        void DeleteTreeGroups();
        void DeleteRecordMapGroups();

        int SaveChanges();
    }

    public partial class PersistedCacheContext : DbContext, IPersistedCacheContext
    {

        private IMSGConfigHelper _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }

        private Ilog _logger { get; set; }

        public PersistedCacheContext(IMSGConfigHelper config, Ilog ilog)
        {
            _configObj = config;
            _logger = ilog;
        }

        public PersistedCacheContext(DbContextOptions<PersistedCacheContext> options)
            : base(options)
        {
        }

        public static PersistedCacheContext Create(IMSGConfigHelper imsgConfigHelper, Ilog logger)
        {
            //var a = new FTMakerCacheContext(new ConfigObj
            //{
            //    Path = imsgConfigHelper.CacheData_Path,
            //    FileName = imsgConfigHelper.CacheData_FileName,
            //    IsEncrypted = imsgConfigHelper.CacheData_IsEncrypted
            //});


            return new PersistedCacheContext(imsgConfigHelper,logger);
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

        public int BulkInsertMarriages(int nextId, int importId, List<FTMMarriage> marriages)
        {
            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO FTMMarriages(Id, MarriageLocation, Origin, GroomId, BrideId, Notes, MarriageDateStr, MarriageYear, ImportId)" +
                                  " VALUES ($Id, $MarriageLocation, $Origin, $GroomId, $BrideId, $Notes, $MarriageDateStr, $MarriageYear, $ImportId);";
            

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$MarriageLocation", SqliteType.Text);
            command.Parameters.Add("$Origin", SqliteType.Text);
            command.Parameters.Add("$GroomId", SqliteType.Integer);
            command.Parameters.Add("$BrideId", SqliteType.Integer);
            command.Parameters.Add("$Notes", SqliteType.Text);
            command.Parameters.Add("$MarriageDateStr", SqliteType.Text);
            command.Parameters.Add("$MarriageYear", SqliteType.Integer);
            command.Parameters.Add("$ImportId", SqliteType.Integer);


            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            var idx = nextId;

            var total = marriages.Count;
            var counter = 1;

            foreach (var row in marriages)
            {
                command.Parameters["$Id"].Value = idx;
                command.Parameters["$MarriageLocation"].Value = row.MarriageLocation;
                command.Parameters["$Origin"].Value = row.Origin??"";
                command.Parameters["$GroomId"].Value = row.GroomId;
                command.Parameters["$BrideId"].Value = row.BrideId;
                command.Parameters["$Notes"].Value = row.Notes??"";
                command.Parameters["$MarriageDateStr"].Value = row.MarriageDateStr;
                command.Parameters["$MarriageYear"].Value = row.MarriageYear;
                command.Parameters["$ImportId"].Value = importId;
                command.ExecuteNonQuery();

                if (counter % 500 == 0)
                    _logger.ProgressUpdate(counter, total, "Inserting Marriage");

                idx++;
            }

            transaction.Commit();

            return idx;
        }

        public int BulkInsertFTMPersonView(int nextId, int importId, List<FTMPersonView> ftmPersonViews)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO FTMPersonView(Id, FirstName, Surname, BirthFrom, BirthTo, BirthLocation, BirthLat, BirthLong, AltLocationDesc, AltLocation, AltLat, AltLong, Origin, PersonId, LinkedLocations, FatherId, MotherId, LocationsCached, ImportId, DirectAncestor, RootPerson,LinkNode)" +
                                  " VALUES ($Id, $FirstName, $Surname, $BirthFrom, $BirthTo, $BirthLocation, $BirthLat, $BirthLong, $AltLocationDesc, $AltLocation, $AltLat, $AltLong, $Origin, $PersonId, $LinkedLocations, $FatherId, $MotherId, $LocationsCached, $ImportId, $DirectAncestor, $RootPerson,$LinkNode);";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$FirstName",SqliteType.Text);
            command.Parameters.Add("$Surname",SqliteType.Text);
            command.Parameters.Add("$BirthFrom",SqliteType.Text);
            command.Parameters.Add("$BirthTo",SqliteType.Text);
            command.Parameters.Add("$BirthLocation",SqliteType.Text);
            command.Parameters.Add("$BirthLat",SqliteType.Text);
            command.Parameters.Add("$BirthLong",SqliteType.Text);
            command.Parameters.Add("$AltLocationDesc",SqliteType.Text);
            command.Parameters.Add("$AltLocation",SqliteType.Text);
            command.Parameters.Add("$AltLat",SqliteType.Text);
            command.Parameters.Add("$AltLong",SqliteType.Text);
            command.Parameters.Add("$Origin",SqliteType.Text);
            command.Parameters.Add("$PersonId",SqliteType.Integer);
            command.Parameters.Add("$LinkedLocations",SqliteType.Text);
            command.Parameters.Add("$FatherId",SqliteType.Integer);
            command.Parameters.Add("$MotherId",SqliteType.Integer);
            command.Parameters.Add("$LocationsCached",SqliteType.Integer);
            command.Parameters.Add("$ImportId",SqliteType.Integer);
            command.Parameters.Add("$DirectAncestor",SqliteType.Integer);
            command.Parameters.Add("$RootPerson", SqliteType.Integer);
            command.Parameters.Add("$LinkNode", SqliteType.Integer);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            var idx = nextId;

            var total = ftmPersonViews.Count;
            var counter = 1;

            foreach (var row in ftmPersonViews)
            {
                command.Parameters["$Id"].Value = idx;
                command.Parameters["$FirstName"].Value = row.FirstName;
                command.Parameters["$Surname"].Value = row.Surname;
                command.Parameters["$BirthFrom"].Value = row.BirthFrom;
                command.Parameters["$BirthTo"].Value = row.BirthTo;
                command.Parameters["$BirthLocation"].Value = row.BirthLocation;
                command.Parameters["$BirthLat"].Value = row.BirthLat;
                command.Parameters["$BirthLong"].Value = row.BirthLong;
                command.Parameters["$AltLocationDesc"].Value = row.AltLocationDesc;
                command.Parameters["$AltLocation"].Value = row.AltLocation;
                command.Parameters["$AltLat"].Value = row.AltLat;
                command.Parameters["$AltLong"].Value = row.AltLong;
                command.Parameters["$Origin"].Value = row.Origin;
                command.Parameters["$PersonId"].Value = row.PersonId;
                command.Parameters["$LinkedLocations"].Value = row.LinkedLocations;
                command.Parameters["$FatherId"].Value = row.FatherId;
                command.Parameters["$MotherId"].Value = row.MotherId;
                command.Parameters["$LocationsCached"].Value = row.LocationsCached;
                command.Parameters["$ImportId"].Value = importId;
                command.Parameters["$DirectAncestor"].Value = row.DirectAncestor;
                command.Parameters["$RootPerson"].Value = row.RootPerson;
                command.Parameters["$LinkNode"].Value = row.LinkNode;
                command.ExecuteNonQuery();

                if(counter % 500 == 0)
                    _logger.ProgressUpdate(counter, total,"Inserting Persons");

                idx++;
            }

            transaction.Commit();

            return idx;
        }

        public int BulkInsertFTMPersonOrigins(int nextId,int importId, List<FTMPersonOrigin> origins)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO FTMPersonOrigins(Id, PersonId,Origin, DirectAncestor,ImportId)" +
                                  " VALUES ($Id,$PersonId,$Origin, $DirectAncestor,$ImportId);";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$PersonId", SqliteType.Integer);
            command.Parameters.Add("$Origin", SqliteType.Text);
            command.Parameters.Add("$ImportId", SqliteType.Integer);
            command.Parameters.Add("$DirectAncestor", SqliteType.Integer);
            
            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            var idx = nextId;
            foreach (var row in origins)
            {
                command.Parameters["$Id"].Value = idx;
                command.Parameters["$PersonId"].Value = row.Id;
                command.Parameters["$Origin"].Value = row.Origin;
                command.Parameters["$ImportId"].Value = row.ImportId;
                command.Parameters["$DirectAncestor"].Value = row.DirectAncestor;
                command.ExecuteNonQuery();
                idx++;
            }

            transaction.Commit();

            return idx;
        }

        public int BulkInsertTreeRecords(List<TreeRecord> treeRecords)
        {
            if (treeRecords.Count <= 0) return 0;
            
            int idx = TreeRecords.Count() + 1;
            
            foreach (var tr in treeRecords)
            {
                tr.Id = idx;
                idx++;
            }

            this.TreeRecords.AddRange(treeRecords);
            
            return this.SaveChanges();
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
