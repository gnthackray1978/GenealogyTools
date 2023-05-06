using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using ConfigHelper;
using FTMContextNet.Domain.Entities.Source;
using Microsoft.EntityFrameworkCore;
using PlaceLibNet.Model;

namespace FTMContextNet.Data
{

    public partial class FTMakerContext : DbContext
    {
        private IMSGConfigHelper _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }

        public FTMakerContext(IMSGConfigHelper config)
        {
            _configObj = config;
        }

        public FTMakerContext(DbContextOptions<FTMakerContext> options)
            : base(options)
        {
        }

        public List<string> DumpCount()
        {

            List<string> results = new List<string>();

            DumpRecordCount(results, Fact, "Fact");

            DumpRecordCount(results, Person, "Person");
            DumpRecordCount(results, Place, "Place");
            DumpRecordCount(results, Relationship, "Relationship");
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

        public static FTMakerContext CreateSourceDB(IMSGConfigHelper imsGConfigHelper)
        {
            var source = new FTMakerContext(imsGConfigHelper);

            return source;
        }

        public void DeleteAll()
        {
            var tp = GetCon();


            var command = tp.CreateCommand();
            tp.Open();

            command.CommandText = @"delete from Person";
            Console.WriteLine("Delete From Person");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from ChildRelationship";
            Console.WriteLine("Delete From ChildRelationship");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Deleted";
            Console.WriteLine("Delete From Deleted");
            command.ExecuteNonQuery();

            command.CommandText = @"delete from HistoryList";
            Console.WriteLine("Delete From HistoryList");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from MasterSource";
            Console.WriteLine("Delete From MasterSource");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from MediaLink";
            Console.WriteLine("Delete From MediaLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Note";
            Console.WriteLine("Delete From Note");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from PersonExternal";
            Console.WriteLine("Delete From PersonExternal");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from PersonGroup";
            Console.WriteLine("Delete From PersonGroup");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Publication";
            Console.WriteLine("Delete From Publication");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Repository";
            Console.WriteLine("Delete From Repository");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Setting";
            Console.WriteLine("Delete From Setting");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Source";
            Console.WriteLine("Delete From Source");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from SourceLink";
            Console.WriteLine("Delete From SourceLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Tag";
            Console.WriteLine("Delete From Tag");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from TagLink";
            Console.WriteLine("Delete From TagLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Task";
            Console.WriteLine("Delete From Task");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Tag";
            Console.WriteLine("Delete From Tag");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Place";
            Console.WriteLine("Delete From Place");
            command.ExecuteNonQuery();


            command.CommandText = @"delete from Relationship";
            Console.WriteLine("Delete From Relationship");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from MediaFile";
            Console.WriteLine("Delete From MediaFile");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from WebLink";
            Console.WriteLine("Delete From WebLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Fact";
            Console.WriteLine("Delete From Fact");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from FactType";
            Console.WriteLine("Delete From FactType");
            command.ExecuteNonQuery();
            tp.Close();




        }

        #region tables
        public virtual DbSet<ChildRelationship> ChildRelationship { get; set; }

        public virtual DbSet<Fact> Fact { get; set; }

        public virtual DbSet<Person> Person { get; set; }

        public virtual DbSet<Place> Place { get; set; }

        public virtual DbSet<Relationship> Relationship { get; set; }

        #endregion

        private SQLiteConnection GetCon()
        {

            string cs = "";

            if (_configObj.DNA_Match_File_IsEncrypted)
            {
                cs = "data source=\"" + _configObj.DNA_Match_File_Path
                                      + _configObj.DNA_Match_File_FileName
                                      + "\";synchronous=Off;pooling=False;journal mode=Memory;foreign keys=True;"
                                      + _configObj.FTMConString
                                      + ";datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
            }
            else
            {
                cs = "data source=\"" + _configObj.DNA_Match_File_Path
                                      + _configObj.DNA_Match_File_FileName
                                      + "\";pooling=False;journal mode=Memory;foreign keys=True;datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
            }

            _sqlConnection = new SQLiteConnection(cs);

            _sqlConnection.Flags |= SQLiteConnectionFlags.AllowNestedTransactions;

            return _sqlConnection;
        }

        #region onconfiguring onmodelcreating etc
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(GetCon());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChildRelationship>(entity =>
            {
                entity.Property(e => e.RelationshipId).HasColumnName("RelationshipID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");


            });

            modelBuilder.Entity<Fact>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasMaxLength(255);

                entity.Property(e => e.FactTypeId).HasColumnName("FactTypeID");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.PlaceId).HasColumnName("PlaceID");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.Text).HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BirthDate).HasMaxLength(255);

                entity.Property(e => e.BirthFactId).HasColumnName("BirthFactID");

                entity.Property(e => e.BirthPlace).HasMaxLength(1001);

                entity.Property(e => e.BirthPlaceId).HasColumnName("BirthPlaceID");

                entity.Property(e => e.DeathDate).HasMaxLength(255);

                entity.Property(e => e.DeathPlace).HasMaxLength(1001);

                entity.Property(e => e.DeathPlaceId).HasColumnName("DeathPlaceID");

                entity.Property(e => e.FamilyName).IsUnicode(false);

                entity.Property(e => e.FullName).IsUnicode(false);

                entity.Property(e => e.GivenName).IsUnicode(false);

                entity.Property(e => e.NameSuffix).IsUnicode(false);



                entity.Property(e => e.Title).IsUnicode(false);

            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DisplayName).HasMaxLength(512);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(475);
            });

            modelBuilder.Entity<Relationship>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Person1Id).HasColumnName("Person1ID");

                entity.Property(e => e.Person2Id).HasColumnName("Person2ID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        #endregion
    }
}
