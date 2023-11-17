using System;
using System.Collections.Generic;
using System.Linq;
using ConfigHelper;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using FTMContextNet.Domain.ExtensionMethods;
using LoggingLib;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data;

public partial class AzurePersistedCacheContext : DbContext, IPersistedCacheContext
{

    private IMSGConfigHelper _configObj { get; set; }
    private readonly IAzureDBHelpers _azureDbHelpers;

    private Ilog _logger { get; set; }

    public AzurePersistedCacheContext(IAzureDBHelpers azureDbHelpers, IMSGConfigHelper config, Ilog ilog)
    {
        _configObj = config;
        _logger = ilog;
        _azureDbHelpers = azureDbHelpers;
    }

    public AzurePersistedCacheContext(DbContextOptions<SQLitePersistedCacheContext> options)
        : base(options)
    {
    }

    public static AzurePersistedCacheContext Create(IAzureDBHelpers azureDbHelpers, IMSGConfigHelper imsgConfigHelper, Ilog logger)
    {
        return new AzurePersistedCacheContext(azureDbHelpers, imsgConfigHelper, logger);
    }

    #region tables

    public virtual DbSet<PersonOrigins> PersonOrigins { get; set; }
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

    public int BulkInsertMarriages(int importId, int userId, List<Relationships> marriages)
    {
        var connectionString = this.Database.GetConnectionString();

        var nextId = _azureDbHelpers.GetNextId("Relationships");
        
        var dt = _azureDbHelpers.CreateDataTable( "select top 1 * from dna.Relationships");



        int idx = nextId;

        foreach (var row in marriages)
        {
            row.Id = idx;
            row.ImportId = importId;
            row.UserId = userId;
            idx++;
        }

        foreach (var row in marriages)
        {
            dt.Rows.Add(row.Id,
                row.GroomId,
                row.BrideId,
                row.Notes,
                row.DateStr,
                row.Year,
                row.Location,
                row.Origin,
                row.ImportId,
                row.UserId
            );

            idx++;
        }

        using var copy = new SqlBulkCopy(connectionString);

        copy.DestinationTableName = "dna.Relationships";
        copy.BulkCopyTimeout = 600;
        copy.ColumnMappings.Add("Id", "Id");
        copy.ColumnMappings.Add("GroomId", "GroomId");
        copy.ColumnMappings.Add("BrideId", "BrideId");
        copy.ColumnMappings.Add("Notes", "Notes");
        copy.ColumnMappings.Add("DateStr", "DateStr");
        copy.ColumnMappings.Add("Year", "Year");
        copy.ColumnMappings.Add("Location", "Location");
        copy.ColumnMappings.Add("Origin", "Origin");
        copy.ColumnMappings.Add("ImportId", "ImportId");
        copy.ColumnMappings.Add("UserId", "UserId");
        copy.WriteToServer(dt);

        return 1;
    }
    
    public int BulkInsertFTMPersonView(int importId, int userId, List<FTMPersonView> ftmPersonViews)
    {
        var connectionString = this.Database.GetConnectionString();

        var nextId = _azureDbHelpers.GetNextId("FTMPersonView");

        var dt = _azureDbHelpers.CreateDataTable( "SELECT TOP 1 * FROM dna.FTMPersonView");
        
        int idx = nextId;

        foreach (var row in ftmPersonViews)
        {
            row.Id = idx;
            row.ImportId = importId;
            row.UserId = userId;
            idx++;
        }


        foreach (var row in ftmPersonViews)
        {
            dt.Rows.Add(row.Id,
                row.FirstName,
                row.Surname,
                row.BirthFrom,
                row.BirthTo,
                row.BirthLocation,
                row.BirthLat,
                row.BirthLong,
                row.AltLocationDesc,
                row.AltLocation,
                row.AltLat,
                row.AltLong,
                row.Origin,
                row.PersonId,
                row.FatherId,
                row.MotherId,
                row.DirectAncestor,
                row.LocationsCached,
                row.ImportId,
                row.RootPerson,
                row.LinkNode,
                
                
                row.UserId,
                
                row.LinkedLocations
            );
            
        }


        using var copy = new SqlBulkCopy(connectionString);

        copy.DestinationTableName = "dna.FTMPersonView";
        copy.BulkCopyTimeout = 600;
        copy.ColumnMappings.Add("Id", "ID");
        copy.ColumnMappings.Add("FirstName", "FirstName");
        copy.ColumnMappings.Add("Surname", "Surname");
        copy.ColumnMappings.Add("BirthFrom", "BirthFrom");
        copy.ColumnMappings.Add("BirthTo", "BirthTo");
        copy.ColumnMappings.Add("BirthLocation", "BirthLocation");
        copy.ColumnMappings.Add("BirthLat", "BirthLat");
        copy.ColumnMappings.Add("BirthLong", "BirthLong");
        copy.ColumnMappings.Add("AltLocationDesc", "AltLocationDesc");
        copy.ColumnMappings.Add("AltLocation", "AltLocation");
        copy.ColumnMappings.Add("AltLat", "AltLat");
        copy.ColumnMappings.Add("AltLong", "AltLong");
        copy.ColumnMappings.Add("Origin", "Origin");
        copy.ColumnMappings.Add("PersonId", "PersonId");
        copy.ColumnMappings.Add("FatherId", "FatherId");
        copy.ColumnMappings.Add("MotherId", "MotherId");
        copy.ColumnMappings.Add("DirectAncestor", "DirectAncestor");
        copy.ColumnMappings.Add("LocationsCached", "LocationsCached");
        copy.ColumnMappings.Add("ImportId", "ImportId");
        copy.ColumnMappings.Add("RootPerson", "RootPerson");
        copy.ColumnMappings.Add("LinkNode", "LinkNode");
        copy.ColumnMappings.Add("UserId", "UserId");
        copy.ColumnMappings.Add("LinkedLocations", "LinkedLocations");
        copy.WriteToServer(dt);

        return 1;
    }
    
   
    public void UpdatePersonLocations(int personId, string lng, string lat, string altLng, string altLat)
    {
        var connectionString = this.Database.GetDbConnection().ConnectionString;

        using var connection = new SqlConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE dna.FTMPersonView SET BirthLat = @BirthLat, BirthLong = @BirthLong, AltLat = @AltLat, AltLong = @AltLong WHERE Id = @Id";

        connection.Open();

        using var transaction = connection.BeginTransaction();

        command.Transaction = transaction;
        command.Prepare();

        command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = personId });
        command.Parameters.Add(new SqlParameter { ParameterName = "@BirthLat", Value = lat });
        command.Parameters.Add(new SqlParameter { ParameterName = "@BirthLong", Value = lng });
        command.Parameters.Add(new SqlParameter { ParameterName = "@AltLat", Value = altLat });
        command.Parameters.Add(new SqlParameter { ParameterName = "@AltLong", Value = altLng });

        command.ExecuteNonQuery();

        transaction.Commit();
       
    }

    public bool ImportExistsInPersons(int importId)
    {
        throw new System.NotImplementedException();
    }

    public int BulkInsertPersonOrigins(int userId, List<PersonOrigins> origins)
    {
        var connectionString = this.Database.GetDbConnection().ConnectionString;

        var nextId = _azureDbHelpers.GetNextId("PersonOrigins");

        var dt = _azureDbHelpers.CreateDataTable( "SELECT TOP 1 * FROM dna.PersonOrigins");


        int idx = nextId;
        

        foreach (var row in origins)
        {
            row.Id = idx;
            row.UserId = userId;
            idx++;
        }


        foreach (var row in origins)
        {
            dt.Rows.Add(row.Id,
                row.PersonId,
                row.Origin,
                row.DirectAncestor,
                row.ImportId,
                row.UserId
            );

            idx++;
        }

        using var copy = new SqlBulkCopy(this.Database.GetConnectionString());

        copy.DestinationTableName = "dna.PersonOrigins";
        copy.BulkCopyTimeout = 600;
        copy.ColumnMappings.Add("Id", "Id");
        copy.ColumnMappings.Add("PersonId", "PersonId");
        copy.ColumnMappings.Add("Origin", "Origin");
        copy.ColumnMappings.Add("DirectAncestor", "DirectAncestor");
        copy.ColumnMappings.Add("ImportId", "ImportId");
        copy.ColumnMappings.Add("UserId", "UserId");
        copy.WriteToServer(dt);

        return 1;
    }

    public int BulkInsertTreeRecord(int userId, List<TreeRecord> treeRecords)
    {
        if (treeRecords.Count <= 0) return 0;

        int idx = 0;

        if(TreeRecord.Any())
            idx = TreeRecord.Max(m=>m.Id) + 1;

        foreach (var tr in treeRecords)
        {
            tr.Id = idx;
            tr.UserId = userId;
            idx++;
        }

        this.TreeRecord.AddRange(treeRecords);

        return this.SaveChanges();
    }

    public int InsertGroups(int id, string groupName, int importId, int userId)
    {
        var connectionString = this.Database.GetDbConnection().ConnectionString;


        using var connection = new SqlConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dna.TreeGroups(Id, GroupName,ImportId, UserId) VALUES (@Id,@GroupName,@ImportId,@UserId);";
         
        connection.Open();

        using var transaction = connection.BeginTransaction();

        command.Transaction = transaction;
        command.Prepare();
        
        command.Parameters.Add(new SqlParameter {ParameterName = "@Id", Value = id });
        command.Parameters.Add(new SqlParameter {ParameterName = "@GroupName", Value = groupName });
        command.Parameters.Add(new SqlParameter {ParameterName = "@UserId", Value = userId });
        command.Parameters.Add(new SqlParameter {ParameterName = "@ImportId", Value = importId });
        
        command.ExecuteNonQuery();

        transaction.Commit();

        return id;
    }

    public int InsertRecordMapGroup(string groupName, string treeName, int importId, int userId)
    {
        var connectionString = this.Database.GetDbConnection().ConnectionString;

        var nextId = _azureDbHelpers.GetNextId("TreeRecordMapGroup");
        
        using var connection = new SqlConnection(connectionString);

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO dna.TreeRecordMapGroup(Id, TreeName, GroupName,ImportId, UserId) VALUES (@Id,@TreeName,@GroupName,@ImportId, @UserId);";

        connection.Open();

        using var transaction = connection.BeginTransaction();

        command.Transaction = transaction;
        command.Prepare();

        command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = nextId });
        command.Parameters.Add(new SqlParameter { ParameterName = "@TreeName", Value = treeName });
        command.Parameters.Add(new SqlParameter { ParameterName = "@GroupName", Value = groupName });
        command.Parameters.Add(new SqlParameter { ParameterName = "@UserId", Value = userId });
        command.Parameters.Add(new SqlParameter { ParameterName = "@ImportId", Value = importId });

        command.ExecuteNonQuery();

        transaction.Commit();
        
        return nextId; 
    }

    #endregion

    #region delete commands

    public void DeleteOrigins(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.PersonOrigins WHERE ImportId = " + importId);
    }

    public void DeleteDupes(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.DupeEntries WHERE ImportId = " + importId);
    }

    public void DeleteDupes()
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.DupeEntries");
    }

    public void DeletePersons(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.FTMPersonView WHERE ImportId = " + importId);
    }

    public void DeleteTreeRecord(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.TreeRecord WHERE Id = " + importId);
    }

    public void DeleteMarriages(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.Relationships WHERE ImportId = " + importId);
    }

    public void DeleteImports(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.TreeImport WHERE Id = " + importId);
    }

    public void DeleteTreeGroups(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.TreeGroups WHERE ImportId = " + importId);
    }

    public void DeleteRecordMapGroups(int importId)
    {
        _azureDbHelpers.RunCommand("DELETE FROM DNA.TreeRecordMapGroup WHERE ImportId = " + importId);
    }

    #endregion


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

        modelBuilder.Entity<PersonOrigins>(entity =>
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

            entity.Property(e => e.BirthLat).HasColumnType("decimal(14, 10)").HasConversion(
                v => v.ToDecimal(),
                v => v.ToString()
            );

            entity.Property(e => e.BirthLong).HasColumnType("decimal(14, 10)").HasConversion(
                v => v.ToDecimal(),
                v => v.ToString()
            );

            entity.Property(e => e.AltLat).HasColumnType("decimal(14, 10)").HasConversion(
                v => v.ToDecimal(),
                v => v.ToString()
            );

            entity.Property(e => e.AltLong).HasColumnType("decimal(14, 10)").HasConversion(
                v => v.ToDecimal(),
                v => v.ToString()
            );

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