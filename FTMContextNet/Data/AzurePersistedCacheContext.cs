using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using ConfigHelper;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data;

public partial class AzurePersistedCacheContext : DbContext, IPersistedCacheContext
{

    private IMSGConfigHelper _configObj { get; set; }
    private SQLiteConnection _sqlConnection { get; set; }

    private Ilog _logger { get; set; }

    public AzurePersistedCacheContext(IMSGConfigHelper config, Ilog ilog)
    {
        _configObj = config;
        _logger = ilog;
    }

    public AzurePersistedCacheContext(DbContextOptions<SQLitePersistedCacheContext> options)
        : base(options)
    {
    }

    public static AzurePersistedCacheContext Create(IMSGConfigHelper imsgConfigHelper, Ilog logger)
    {
        return new AzurePersistedCacheContext(imsgConfigHelper, logger);
    }

    #region tables

    public virtual DbSet<PersonOrigin> PersonOrigins { get; set; }
    public virtual DbSet<TreeRecord> TreeRecord { get; set; }
    public virtual DbSet<TreeGroups> TreeGroups { get; set; }
    public virtual DbSet<TreeRecordMapGroup> TreeRecordMapGroup { get; set; }
    public virtual DbSet<DupeEntry> DupeEntries { get; set; }
    public virtual DbSet<FTMPersonView> FTMPersonView { get; set; }
    public virtual DbSet<Relationships> Relationships { get; set; }

    public virtual DbSet<TreeImport> TreeImport { get; set; }

    public virtual DbSet<IgnoreList> IgnoreList { get; set; }

    #endregion

    #region writes

    public int BulkInsertMarriages(int nextId, int importId, int userId, List<Relationships> marriages)
    {
        var connectionString = this.Database.GetDbConnection().ConnectionString;


        using var connection = new SqliteConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Relationships(Id, Location, Origin, GroomId, BrideId, Notes, DateStr, Year, ImportId, UserId)" +
                              " VALUES ($Id, $Location, $Origin, $GroomId, $BrideId, $Notes, $DateStr, $Year, $ImportId, $UserId);";


        command.Parameters.Add("$Id", SqliteType.Integer);
        command.Parameters.Add("$Location", SqliteType.Text);
        command.Parameters.Add("$Origin", SqliteType.Text);
        command.Parameters.Add("$GroomId", SqliteType.Integer);
        command.Parameters.Add("$BrideId", SqliteType.Integer);
        command.Parameters.Add("$Notes", SqliteType.Text);
        command.Parameters.Add("$DateStr", SqliteType.Text);
        command.Parameters.Add("$Year", SqliteType.Integer);
        command.Parameters.Add("$ImportId", SqliteType.Integer);
        command.Parameters.Add("$UserId", SqliteType.Integer);

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
            command.Parameters["$Location"].Value = row.Location;
            command.Parameters["$Origin"].Value = row.Origin ?? "";
            command.Parameters["$GroomId"].Value = row.GroomId;
            command.Parameters["$BrideId"].Value = row.BrideId;
            command.Parameters["$Notes"].Value = row.Notes ?? "";
            command.Parameters["$DateStr"].Value = row.DateStr;
            command.Parameters["$Year"].Value = row.Year;
            command.Parameters["$ImportId"].Value = importId;
            command.Parameters["$UserId"].Value = userId;
            command.ExecuteNonQuery();

            if (counter % 500 == 0)
                _logger.ProgressUpdate(counter, total, "Inserting Marriage");

            idx++;
        }

        transaction.Commit();

        return idx;
    }

    public int BulkInsertFTMPersonView(int nextId, int importId, int userId, List<FTMPersonView> ftmPersonViews)
    {

        var connectionString = this.Database.GetDbConnection().ConnectionString;


        using var connection = new SqliteConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO FTMPersonView(Id, FirstName, Surname, BirthFrom, BirthTo, BirthLocation, BirthLat, BirthLong, AltLocationDesc, AltLocation, AltLat, AltLong, Origin, PersonId, LinkedLocations, FatherId, MotherId, LocationsCached, ImportId, DirectAncestor, RootPerson,LinkNode, UserId)" +
                              " VALUES ($Id, $FirstName, $Surname, $BirthFrom, $BirthTo, $BirthLocation, $BirthLat, $BirthLong, $AltLocationDesc, $AltLocation, $AltLat, $AltLong, $Origin, $PersonId, $LinkedLocations, $FatherId, $MotherId, $LocationsCached, $ImportId, $DirectAncestor, $RootPerson,$LinkNode, $UserId);";

        command.Parameters.Add("$Id", SqliteType.Integer);
        command.Parameters.Add("$FirstName", SqliteType.Text);
        command.Parameters.Add("$Surname", SqliteType.Text);
        command.Parameters.Add("$BirthFrom", SqliteType.Text);
        command.Parameters.Add("$BirthTo", SqliteType.Text);
        command.Parameters.Add("$BirthLocation", SqliteType.Text);
        command.Parameters.Add("$BirthLat", SqliteType.Text);
        command.Parameters.Add("$BirthLong", SqliteType.Text);
        command.Parameters.Add("$AltLocationDesc", SqliteType.Text);
        command.Parameters.Add("$AltLocation", SqliteType.Text);
        command.Parameters.Add("$AltLat", SqliteType.Text);
        command.Parameters.Add("$AltLong", SqliteType.Text);
        command.Parameters.Add("$Origin", SqliteType.Text);
        command.Parameters.Add("$PersonId", SqliteType.Integer);
        command.Parameters.Add("$LinkedLocations", SqliteType.Text);
        command.Parameters.Add("$FatherId", SqliteType.Integer);
        command.Parameters.Add("$MotherId", SqliteType.Integer);
        command.Parameters.Add("$LocationsCached", SqliteType.Integer);
        command.Parameters.Add("$ImportId", SqliteType.Integer);
        command.Parameters.Add("$DirectAncestor", SqliteType.Integer);
        command.Parameters.Add("$RootPerson", SqliteType.Integer);
        command.Parameters.Add("$LinkNode", SqliteType.Integer);
        command.Parameters.Add("$UserId", SqliteType.Integer);

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
            command.Parameters["$UserId"].Value = userId;
            command.ExecuteNonQuery();

            if (counter % 500 == 0)
                _logger.ProgressUpdate(counter, total, "Inserting Persons");

            idx++;
        }

        transaction.Commit();

        return idx;
    }

    public void UpdatePersonLocations(int personId, string lng, string lat, string altLng, string altLat)
    {
        var connectionString = this.Database.GetDbConnection().ConnectionString;

        using var connection = new SqliteConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE FTMPersonView SET BirthLat = $BirthLat, BirthLong = $BirthLong, AltLat = $AltLat, AltLong = $AltLong WHERE Id = $Id";

        command.Parameters.Add("$Id", SqliteType.Integer);
        command.Parameters.Add("$BirthLat", SqliteType.Text);
        command.Parameters.Add("$BirthLong", SqliteType.Text);
        command.Parameters.Add("$AltLat", SqliteType.Text);
        command.Parameters.Add("$AltLong", SqliteType.Text);

        connection.Open();

        using var transaction = connection.BeginTransaction();

        command.Transaction = transaction;
        command.Prepare();


        command.Parameters["$Id"].Value = personId;
        command.Parameters["$BirthLat"].Value = lat;
        command.Parameters["$BirthLong"].Value = lng;
        command.Parameters["$AltLat"].Value = altLat;
        command.Parameters["$AltLong"].Value = altLng;
        command.ExecuteNonQuery();


        transaction.Commit();

    }

    public bool ImportExistsInPersons(int importId)
    {
        throw new System.NotImplementedException();
    }

    public int BulkInsertPersonOrigins(int nextId, int userId, List<PersonOrigin> origins)
    {

        var connectionString = this.Database.GetDbConnection().ConnectionString;


        using var connection = new SqliteConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO PersonOrigins(Id, PersonId,Origin, DirectAncestor,ImportId,UserId)" +
                              " VALUES ($Id,$PersonId,$Origin, $DirectAncestor,$ImportId, $UserId);";

        command.Parameters.Add("$Id", SqliteType.Integer);
        command.Parameters.Add("$PersonId", SqliteType.Integer);
        command.Parameters.Add("$Origin", SqliteType.Text);
        command.Parameters.Add("$ImportId", SqliteType.Integer);
        command.Parameters.Add("$DirectAncestor", SqliteType.Integer);
        command.Parameters.Add("$UserId", SqliteType.Integer);

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
            command.Parameters["$UserId"].Value = userId;
            command.ExecuteNonQuery();
            idx++;
        }

        transaction.Commit();

        return idx;
    }

    public int BulkInsertTreeRecord(List<TreeRecord> treeRecords)
    {
        if (treeRecords.Count <= 0) return 0;

        int idx = TreeRecord.Count() + 1;

        foreach (var tr in treeRecords)
        {
            tr.Id = idx;
            idx++;
        }

        this.TreeRecord.AddRange(treeRecords);

        return this.SaveChanges();
    }

    public int InsertGroups(int nextId, string groupName, int importId, int userId)
    {

        var connectionString = this.Database.GetDbConnection().ConnectionString;


        using var connection = new SqliteConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO TreeGroups(Id, GroupName,ImportId, UserId) VALUES ($Id,$GroupName,$ImportId,$UserId);";

        command.Parameters.Add("$Id", SqliteType.Integer);
        command.Parameters.Add("$GroupName", SqliteType.Text);
        command.Parameters.Add("$UserId", SqliteType.Integer);
        command.Parameters.Add("$ImportId", SqliteType.Integer);

        connection.Open();

        using var transaction = connection.BeginTransaction();

        command.Transaction = transaction;
        command.Prepare();

        command.Parameters["$Id"].Value = nextId;
        command.Parameters["$GroupName"].Value = groupName;
        command.Parameters["$UserId"].Value = userId;
        command.Parameters["$ImportId"].Value = importId;
        command.ExecuteNonQuery();

        transaction.Commit();

        return nextId;
    }
    public int InsertRecordMapGroup(int nextId, string groupName, string treeName, int importId, int userId)
    {

        var connectionString = this.Database.GetDbConnection().ConnectionString;


        using var connection = new SqliteConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO TreeRecordMapGroup(Id, TreeName, GroupName,ImportId, UserId) VALUES ($Id,$TreeName,$GroupName,$ImportId, $UserId);";

        command.Parameters.Add("$Id", SqliteType.Integer);
        command.Parameters.Add("$TreeName", SqliteType.Text);
        command.Parameters.Add("$GroupName", SqliteType.Text);
        command.Parameters.Add("$UserId", SqliteType.Integer);
        command.Parameters.Add("$ImportId", SqliteType.Integer);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        command.Transaction = transaction;
        command.Prepare();

        command.Parameters["$Id"].Value = nextId;
        command.Parameters["$GroupName"].Value = groupName;
        command.Parameters["$TreeName"].Value = treeName;
        command.Parameters["$UserId"].Value = userId;
        command.Parameters["$ImportId"].Value = userId;
        command.ExecuteNonQuery();

        transaction.Commit();

        return nextId;
    }

    #endregion

    #region delete commands

    public void DeleteOrigins(int importId)
    {
        RunCommand("DELETE FROM PersonOrigins WHERE ImportId = " + importId);
    }

    public void DeleteDupes(int importId)
    {
        RunCommand("DELETE FROM DupeEntries WHERE ImportId = " + importId);
    }

    public void DeleteDupes()
    {
        RunCommand("DELETE FROM DupeEntries");
    }


    public void DeletePersons(int importId)
    {
        RunCommand("DELETE FROM FTMPersonView WHERE ImportId = " + importId);
    }

    public void DeleteTreeRecord(int importId)
    {
        RunCommand("DELETE FROM TreeRecord WHERE Id = " + importId);
    }

    public void DeleteMarriages(int importId)
    {
        RunCommand("DELETE FROM Relationships WHERE ImportId = " + importId); ;
    }

    public void DeleteImports(int importId)
    {
        RunCommand("DELETE FROM TreeImport WHERE Id = " + importId); ;
    }

    public void DeleteTreeGroups(int importId)
    {
        RunCommand("DELETE FROM TreeGroups WHERE ImportId = " + importId);
    }

    public void DeleteRecordMapGroups(int importId)
    {
        RunCommand("DELETE FROM TreeRecordMapGroup WHERE ImportId = " + importId);
    }

    #endregion

    public void RunCommand(string queryString)
    {
        using SqlConnection connection = new SqlConnection(_configObj.MSGGenDB01);

        var command = new SqlCommand(queryString, connection);

        try
        {
            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }




    #region config

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configObj.MSGGenDB01);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IgnoreList>(entity =>
        {
            entity.ToTable("IgnoreList", "DNA");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Person1).HasMaxLength(500);

            entity.Property(e => e.Person1).HasMaxLength(500);
            
        });

        modelBuilder.Entity<PersonOrigin>(entity =>
        {
            entity.ToTable("PersonOrigins", "DNA");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Origin).HasMaxLength(500);
        });

        modelBuilder.Entity<DupeEntry>(entity =>
        {
            entity.ToTable("DupeEntries", "DNA");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Ident).HasMaxLength(500);

            entity.Property(e => e.Origin).HasMaxLength(500);

            entity.Property(e => e.FirstName).HasMaxLength(500);

            entity.Property(e => e.Surname).HasMaxLength(500);

        });

        modelBuilder.Entity<FTMPersonView>(entity =>
        {
            entity.ToTable("FTMPersonView", "DNA");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.FirstName).HasMaxLength(500);

            entity.Property(e => e.Surname).HasMaxLength(500);

            entity.Property(e => e.Origin).HasMaxLength(250);

            entity.Property(e => e.BirthLat).HasColumnType("decimal(14, 10)");

            entity.Property(e => e.BirthLong).HasColumnType("decimal(14, 10)");

            entity.Property(e => e.AltLat).HasColumnType("decimal(14, 10)");

            entity.Property(e => e.AltLong).HasColumnType("decimal(14, 10)");

        });

        modelBuilder.Entity<Relationships>(entity =>
        {
            entity.ToTable("Relationships", "DNA");

            entity.Property(e => e.Id).ValueGeneratedNever();


        });

        modelBuilder.Entity<TreeRecord>(entity =>
        {
            entity.ToTable("TreeRecord", "DNA");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name);
            entity.Property(e => e.Origin).HasMaxLength(250);
        });

        modelBuilder.Entity<TreeGroups>(entity =>
        {
            entity.ToTable("TreeGroups", "DNA");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.GroupName).HasMaxLength(500);
        });
        modelBuilder.Entity<TreeImport>(entity =>
        {
            entity.ToTable("TreeImport", "DNA");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(100);
        });
        modelBuilder.Entity<TreeRecordMapGroup>(entity =>
        {
            entity.ToTable("TreeRecordMapGroup", "DNA");
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

    

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    #endregion
}