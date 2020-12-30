using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DNAGedLib.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GenDBContext.Models
{
    public class DebugLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers;

        public DebugLoggerProvider()
        {
            _loggers = new ConcurrentDictionary<string, ILogger>();
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, new DebugLogger());
        }
    }

    public class DebugLogger : ILogger
    {
        public void Log<TState>(
            LogLevel logLevel, EventId eventId,
            TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                Debug.WriteLine(formatter(state, exception));
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }

    public enum PlaceCriteria
    {
        ForEnglishCounties,
        ForMappings,
        ForAllUnknownCounties
    }

    public partial class DNAGEDContext : DbContext
    {


        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddProvider(new DebugLoggerProvider()); });

        public virtual DbSet<MatchDetail> MatchDetail { get; set; }
        public virtual DbSet<MatchGroups> MatchGroups { get; set; }
        public virtual DbSet<MatchIcw> MatchIcw { get; set; }
        public virtual DbSet<MatchTrees> MatchTrees { get; set; }
        public virtual DbSet<Persons> Persons { get; set; }
        public virtual DbSet<TreePersons> TreePersons { get; set; }
        public virtual DbSet<MyPersons> MyPersons { get; set; }
        public virtual DbSet<PersonsOfInterest> PersonsOfInterest { get; set; }
        public virtual DbSet<PersonGroups> PersonGroups { get; set; }
        public virtual DbSet<MatchKitName> MatchKitName { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  optionsBuilder.UseLoggerFactory(MyLoggerFactory);

            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-KGS70RI\SQL2016EX;Initial Catalog=DNAGED;Integrated Security=SSPI;");
                //C:\Users\GeorgePickworth\Downloads\
                optionsBuilder.UseSqlite(@"Data Source=C:\Users\GeorgePickworth\Documents\genDB.db");
            }


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                //entity.HasOne(d => d.MatchGu)
                //    .WithMany(p => p.MatchDetail)
                //    .HasForeignKey(d => d.MatchGuid)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_MatchDetail_MatchGroups");
            });

            modelBuilder.Entity<PersonGroups>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<MatchKitName>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<MatchGroups>(entity =>
            {
                entity.HasKey(e => e.Id);

                // entity.Property(e => e.MatchGuid).ValueGeneratedNever();

                entity.Property(e => e.GroupName).IsUnicode(false);

                entity.Property(e => e.Note).IsUnicode(false);

                entity.Property(e => e.TestAdminDisplayName).IsUnicode(false);

                entity.Property(e => e.TestDisplayName).IsUnicode(false);

                entity.Property(e => e.TreeId).IsUnicode(false);


            });

            modelBuilder.Entity<MatchIcw>(entity =>
            {
                entity.ToTable("MatchICW");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Icqadmin)
                    .HasColumnName("ICQAdmin")
                    .IsUnicode(false);

                entity.Property(e => e.Icwid).HasColumnName("ICWId");

                entity.Property(e => e.Icwname)
                    .HasColumnName("ICWName")
                    .IsUnicode(false);

                entity.Property(e => e.MatchAdmin).IsUnicode(false);

                entity.Property(e => e.MatchName).IsUnicode(false);

                entity.Property(e => e.Source).IsUnicode(false);

                //entity.HasOne(d => d.Match)
                //    .WithMany(p => p.MatchIcw)
                //    .HasForeignKey(d => d.MatchId)
                //    .HasConstraintName("FK_MatchICW_MatchGroups");
            });

            modelBuilder.Entity<MatchTrees>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                //entity.HasOne(d => d.Match)
                //    .WithMany(p => p.MatchTrees)
                //    .HasForeignKey(d => d.MatchId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                ////    .HasConstraintName("FK_MatchTrees_MatchGroups");

                //entity.HasOne(d => d.Person)
                //    .WithMany(p => p.MatchTrees)
                //    .HasForeignKey(d => d.PersonId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_MatchTrees_Persons");
            });

            modelBuilder.Entity<Persons>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasMany(d => d.MatchTrees)
                  .WithOne(p => p.Person).HasForeignKey(d => d.PersonId)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_MatchTrees_Persons");
            });

            modelBuilder.Entity<TreePersons>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();             
            });

            modelBuilder.Entity<MyPersons>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

            });

            modelBuilder.Entity<PersonsOfInterest>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

            });
        }

        private static object SetCol(string val)
        {
            if (val == null)
            {
                return DBNull.Value;
            }
            else
            {
                return val;
            }
        }

        public void DeleteLocalFamilyTreePersonsFromPersons()
        {           
            var oldPersons = this.Persons.Where(pid => pid.RootsEntry);

            Console.WriteLine("Removing  " + oldPersons.Count() + " old records");

            this.RemoveRange(oldPersons);

            this.SaveChanges();           
        }

        public void DeleteTreePersons()
        {
            var connectionString = this.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqliteConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM TreePersons";


                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();


            }

        }

        public void BulkInsertTreePersons(List<Persons> rows)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using (var connection = new SqliteConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO TreePersons(Id, ChristianName,Surname,FatherId, MotherId, BirthDate, BirthYear,BirthPlace, " +
                                      "BirthCounty,BirthCountry,DeathDate,DeathYear,DeathPlace, DeathCounty,DeathCountry, " +
                                      "Memory,CreatedDate,RootsEntry,Fix,EnglishParentsChecked,CountyUpdated, AmericanParentsChecked,CountryUpdated)" +
                                      " VALUES ($Id,$ChristianName,$Surname,$FatherId,$MotherId,$BirthDate, $BirthYear,$BirthPlace," +
                                      "$BirthCounty,$BirthCountry,$DeathDate,$DeathYear,$DeathPlace,$DeathCounty,$DeathCountry," +
                                      "$Memory,$CreatedDate,$RootsEntry,$Fix,$EnglishParentsChecked,$CountyUpdated,$AmericanParentsChecked,$CountryUpdated);";

                command.Parameters.Add("$Id", SqliteType.Integer);
                command.Parameters.Add("$ChristianName", SqliteType.Text);
                command.Parameters.Add("$Surname", SqliteType.Text);
                command.Parameters.Add("$FatherId", SqliteType.Integer);
                command.Parameters.Add("$MotherId", SqliteType.Integer);

                command.Parameters.Add("$BirthDate", SqliteType.Text);
                command.Parameters.Add("$BirthYear", SqliteType.Integer);
                command.Parameters.Add("$BirthPlace", SqliteType.Text);
                command.Parameters.Add("$BirthCounty", SqliteType.Text);
                command.Parameters.Add("$BirthCountry", SqliteType.Text);

                command.Parameters.Add("$DeathDate", SqliteType.Text);
                command.Parameters.Add("$DeathYear", SqliteType.Integer);
                command.Parameters.Add("$DeathPlace", SqliteType.Text);
                command.Parameters.Add("$DeathCounty", SqliteType.Text);
                command.Parameters.Add("$DeathCountry", SqliteType.Text);

                command.Parameters.Add("$Memory", SqliteType.Text);
                command.Parameters.Add("$CreatedDate", SqliteType.Text);
                command.Parameters.Add("$RootsEntry", SqliteType.Integer);
                command.Parameters.Add("$Fix", SqliteType.Text);

                command.Parameters.Add("$EnglishParentsChecked", SqliteType.Integer);
                command.Parameters.Add("$CountyUpdated", SqliteType.Integer);
                command.Parameters.Add("$AmericanParentsChecked", SqliteType.Integer);
                command.Parameters.Add("$CountryUpdated", SqliteType.Integer);

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    command.Transaction = transaction;
                    command.Prepare();
                    int idx = 0;
                    foreach (var row in rows)
                    {
                        command.Parameters["$Id"].Value = idx;
                        command.Parameters["$ChristianName"].Value = SetCol(row.ChristianName);
                        command.Parameters["$Surname"].Value = SetCol(row.Surname);
                        command.Parameters["$BirthYear"].Value = row.BirthYear;
                        command.Parameters["$BirthPlace"].Value = SetCol(row.BirthPlace);
                        command.Parameters["$BirthCounty"].Value = SetCol(row.BirthCounty);
                        command.Parameters["$BirthCountry"].Value = SetCol(row.BirthCountry);
                        command.Parameters["$FatherId"].Value = row.FatherId;
                        command.Parameters["$MotherId"].Value = row.MotherId;

                        command.Parameters["$BirthDate"].Value = SetCol(row.BirthDate);
                        command.Parameters["$BirthYear"].Value = row.BirthYear;
                        command.Parameters["$BirthPlace"].Value = SetCol(row.BirthPlace);
                        command.Parameters["$BirthCounty"].Value = SetCol(row.BirthCounty);
                        command.Parameters["$BirthCountry"].Value = SetCol(row.BirthCountry);

                        command.Parameters["$DeathDate"].Value = SetCol(row.DeathDate);
                        command.Parameters["$DeathYear"].Value = row.DeathYear;
                        command.Parameters["$DeathPlace"].Value = SetCol(row.DeathPlace);
                        command.Parameters["$DeathCounty"].Value = SetCol(row.DeathCounty);
                        command.Parameters["$DeathCountry"].Value = SetCol(row.DeathCountry);

                        command.Parameters["$Memory"].Value = SetCol(row.Memory);
                        command.Parameters["$CreatedDate"].Value = row.CreatedDate;
                        command.Parameters["$RootsEntry"].Value = row.RootsEntry;
                        command.Parameters["$Fix"].Value = row.Fix;

                        command.Parameters["$EnglishParentsChecked"].Value = row.EnglishParentsChecked;
                        command.Parameters["$CountyUpdated"].Value = row.CountyUpdated;
                        command.Parameters["$AmericanParentsChecked"].Value = row.AmericanParentsChecked;
                        command.Parameters["$CountryUpdated"].Value = row.CountryUpdated;



                        command.ExecuteNonQuery();
                        idx++;
                    }

                    transaction.Commit();
                }
            }

        }
        
        public List<PersonsOfInterest> FillPersonsBySQLServer()
        {
            List<PersonsOfInterest> personsOfInterests = new List<PersonsOfInterest>();

            using (SqlConnection connection = new SqlConnection(this.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT dbo.Persons.Id, dbo.Persons.ChristianName, dbo.Persons.Surname, dbo.Persons.BirthYear, dbo.Persons.BirthPlace, dbo.Persons.BirthCounty, dbo.Persons.BirthCountry, dbo.MatchGroups.TestDisplayName, \r\n                         dbo.MatchGroups.TestAdminDisplayName, dbo.MatchGroups.TreeId AS TreeURL, dbo.MatchGroups.TestGuid, dbo.MatchGroups.Confidence, dbo.MatchGroups.SharedCentimorgans AS SharedCM, dbo.Persons.CreatedDate, \r\n                         dbo.Persons.RootsEntry, dbo.Persons.Memory, dbo.MatchKitName.Id AS KitIId, dbo.MatchKitName.Name, dbo.MatchTrees.CreatedDate AS MTCreated\r\nFROM            dbo.MatchTrees INNER JOIN\r\n                         dbo.MatchGroups ON dbo.MatchTrees.MatchId = dbo.MatchGroups.MatchGuid INNER JOIN\r\n                         dbo.MatchKitName ON dbo.MatchGroups.TestGuid = dbo.MatchKitName.Id RIGHT OUTER JOIN\r\n                         dbo.Persons ON dbo.MatchTrees.PersonId = dbo.Persons.Id\r\nWHERE        (dbo.Persons.BirthCountry = \'England\') OR\r\n                         (dbo.Persons.BirthCountry = \'Wales\') OR\r\n                         (dbo.Persons.RootsEntry = 1)",
                    connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        long idx = 0;
                        long personIdOut = 0;
                        while (reader.Read())
                        {
                            idx = AddPerson(reader, personsOfInterests, idx, ref personIdOut);
                        }
                    }
                }
            }

            return personsOfInterests;
        }

        public List<PersonsOfInterest> FillPersonsBySQLite()
        {

            var personsOfInterests = new List<PersonsOfInterest>();

            using (var connection = new SqliteConnection(this.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                long idx = 0;

                using (var command = new SqliteCommand(
                    "SELECT Persons.Id, Persons.ChristianName, Persons.Surname, Persons.BirthYear, Persons.BirthPlace, Persons.BirthCounty, Persons.BirthCountry, MatchGroups.TestDisplayName, MatchGroups.TestAdminDisplayName, MatchGroups.TreeId AS TreeURL, MatchGroups.TestGuid, MatchGroups.Confidence, MatchGroups.SharedCentimorgans AS SharedCM, Persons.CreatedDate,  Persons.RootsEntry, Persons.Memory, MatchKitName.Id AS KitIId, MatchKitName.Name, MatchTrees.CreatedDate AS MTCreated" +
                    " FROM Persons LEFT OUTER JOIN MatchTrees on Persons.Id = MatchTrees.PersonId INNER JOIN MatchGroups ON MatchGroups.MatchGuid = MatchTrees.MatchId INNER JOIN MatchKitName ON MatchGroups.TestGuid = MatchKitName.Id" +
                    " WHERE ((Persons.BirthCountry = 'England') OR (Persons.BirthCountry = 'Wales')) AND (Persons.RootsEntry = 0)",
                    connection))
                {
                    using (var reader = command.ExecuteReader())
                    {

                       
                        long personIdOut = 0;
                        while (reader.Read())
                        {
                            idx = AddPerson(reader, personsOfInterests, idx, ref personIdOut);
                        }
                    }
                }


                using (var command = new SqliteCommand(
                    "SELECT Id, ChristianName, Surname, BirthYear, BirthPlace, BirthCounty, BirthCountry, " +
                    "'' AS TestDisplayName, '' AS TestAdminDisplayName, '' AS TreeURL, '' AS TestGuid, 0 AS Confidence, " +
                    "0 AS SharedCM, CreatedDate,  RootsEntry, Memory, '' AS KitIId, " +
                    "'' AS Name, '' AS MTCreated" +
                    " FROM TreePersons" +
                    " Order By TreePersons.Id",
                    connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        
                        long personIdOut = 0;
                        while (reader.Read())
                        {
                            idx = AddPerson(reader, personsOfInterests, idx, ref personIdOut);
                        }
                    }
                }
            }



            return personsOfInterests;
        }


        public List<long> GetPersonsOfUnknownOrigins(bool isEnglish)
        {
            var personsOfUnknownOrigins = new List<long>();

            List<long> GetPersonIds(string table)
            {
                var personsBornInEngland = new List<long>();

                using (var connection = new SqliteConnection(this.Database.GetDbConnection().ConnectionString))
                {
                    connection.Open();


                    string qry = "SELECT Id FROM " + table + " WHERE BirthCountry = 'Unknown' AND EnglishParentsChecked = 0";

                    if (!isEnglish)
                        qry = "SELECT Id FROM " + table + " WHERE BirthCountry = 'Unknown' AND AmericanParentsChecked = 0";

                    using (var command = new SqliteCommand( qry, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long.TryParse(reader.GetValue(0).ToString(), out long personId);
                             
                                    personsBornInEngland.Add(personId);
                            }
                        }
                    }


                }

                return personsBornInEngland;
            }
             
            personsOfUnknownOrigins.AddRange(GetPersonIds("Persons"));
            personsOfUnknownOrigins.AddRange(GetPersonIds("TreePersons"));

            return personsOfUnknownOrigins;
        }

        public List<(long personId, string place)> FillPersonMapPlacesBySQLite(bool countryUpdated,
                                                            PlaceCriteria placeCriteria)
        {

       

            List<(long personId, string place)> GetBirthPlace(string table)
            {
                var personsBornInEngland = new List<(long personId, string place)>();

                using (var connection = new SqliteConnection(this.Database.GetDbConnection().ConnectionString))
                {
                    connection.Open();
                    long idx = 0;

                    string qry = @"SELECT Id, BirthPlace FROM " + table + " WHERE ";

                    string whereClause = "";

                    switch (placeCriteria)
                    {
                        case PlaceCriteria.ForAllUnknownCounties:
                            whereClause = "BirthCountry IN ('England','Scotland','Wales','Unknown') AND " +
                                          "BirthCounty IN ('Unknown', 'BirthCounty') AND BirthPlace <> ''";
                               
                            break;
                        case PlaceCriteria.ForEnglishCounties:
                            whereClause =  "BirthCountry = 'England' AND BirthCounty = 'Unknown' AND BirthPlace <> ''";
                            break;
                        case PlaceCriteria.ForMappings:
                            whereClause = "BirthCountry = 'Unknown' AND BirthPlace <> '' AND BirthPlace IS NOT NULL AND CountryUpdated = " + (countryUpdated ? "1": "0") + "";
                            break;
                    }

                    qry += whereClause;

                    using (var command = new SqliteCommand(qry,connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long.TryParse(reader.GetValue(0).ToString(), out long id);
                               
                                personsBornInEngland.Add((id, reader.GetValue(1).ToString()));
                            }
                        }
                    }


                }

                return personsBornInEngland;
            }


           

            List<(long personId, string place)> personsList;
                        
            personsList = GetBirthPlace("Persons");
          
            personsList.AddRange(GetBirthPlace("TreePersons"));
       

            return personsList;
        }

        public List<(long? FatherId, long? MotherId)> GetPersonsOfGivenNationality(string origin)
        {
            var results = new List<(long? FatherId, long? MotherId)>();

            List<(long? FatherId, long? MotherId)> GetParents(string table)
            {
                var personsBornInEngland = new List<(long? FatherId, long? MotherId)>();

                using (var connection = new SqliteConnection(this.Database.GetDbConnection().ConnectionString))
                {
                    connection.Open();
                  

                    using (var command = new SqliteCommand(
                        "SELECT FatherId, MotherId FROM "+ table + " WHERE BirthCountry = '" + origin + "'",
                        connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long.TryParse(reader.GetValue(0).ToString(), out long fatherId);
                                long.TryParse(reader.GetValue(0).ToString(), out long motherId);

                                if(fatherId!= 0 || motherId!=0)
                                    personsBornInEngland.Add((fatherId, motherId));
                            }
                        }
                    }


                }

                return personsBornInEngland;
            }



            results.AddRange(GetParents("Persons"));
            results.AddRange(GetParents("TreePersons"));



            return results;
        }


        public void ClearPersonsOfInterest()
        {
            this.Database.ExecuteSqlRaw("DELETE FROM PersonsOfInterest");

        }

        public void BulkInsertPersonsOfInterest(List<PersonsOfInterest> rows)
        {
            var connectionString = this.Database.GetDbConnection().ConnectionString;

            using (var connection = new SqliteConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO PersonsOfInterest(Id, PersonId, ChristianName,Surname,BirthYear,BirthPlace, BirthCounty,BirthCountry,TestDisplayName,TestAdminDisplayName,TreeURL,TestGuid,Confidence,SharedCentimorgans,CreatedDate,RootsEntry,Memory,KitId,Name )" +
                                      " VALUES ($Id,$PersonId,$ChristianName,$Surname,$BirthYear,$BirthPlace,$BirthCounty,$BirthCountry,$TestDisplayName,$TestAdminDisplayName,$TreeURL," +
                                      "$TestGuid,$Confidence,$SharedCentimorgans,$CreatedDate,$RootsEntry,$Memory,$KitId,$Name );";

                command.Parameters.Add("$Id", SqliteType.Integer);
                command.Parameters.Add("$PersonId", SqliteType.Integer);
                command.Parameters.Add("$ChristianName", SqliteType.Text);
                command.Parameters.Add("$Surname", SqliteType.Text);
                command.Parameters.Add("$BirthYear", SqliteType.Integer);
                command.Parameters.Add("$BirthPlace", SqliteType.Text);
                command.Parameters.Add("$BirthCounty", SqliteType.Text);
                command.Parameters.Add("$BirthCountry", SqliteType.Text);
                command.Parameters.Add("$TestDisplayName", SqliteType.Text);
                command.Parameters.Add("$TestAdminDisplayName", SqliteType.Text);
                command.Parameters.Add("$TreeURL", SqliteType.Text);
                command.Parameters.Add("$TestGuid", SqliteType.Blob);
                command.Parameters.Add("$Confidence", SqliteType.Real);
                command.Parameters.Add("$SharedCentimorgans", SqliteType.Real);
                command.Parameters.Add("$CreatedDate", SqliteType.Text);
                command.Parameters.Add("$RootsEntry", SqliteType.Text);
                command.Parameters.Add("$Memory", SqliteType.Text);
                command.Parameters.Add("$KitId", SqliteType.Blob);
                command.Parameters.Add("$Name", SqliteType.Text);



                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    command.Transaction = transaction;
                    command.Prepare();
                    int idx = 0;
                    foreach (var row in rows)
                    {
                        command.Parameters["$Id"].Value = idx;
                        command.Parameters["$PersonId"].Value = row.PersonId;
                        command.Parameters["$ChristianName"].Value = SetCol(row.ChristianName);
                        command.Parameters["$Surname"].Value = SetCol(row.Surname);
                        command.Parameters["$BirthYear"].Value = row.BirthYear;
                        command.Parameters["$BirthPlace"].Value = SetCol(row.BirthPlace);
                        command.Parameters["$BirthCounty"].Value = SetCol(row.BirthCounty);
                        command.Parameters["$BirthCountry"].Value = SetCol(row.BirthCountry);
                        command.Parameters["$TestDisplayName"].Value = SetCol(row.TestDisplayName);// row.TestDisplayName != null ? row.TestDisplayName : DBNull.Value;                         
                        command.Parameters["$TestAdminDisplayName"].Value = SetCol(row.TestAdminDisplayName);
                        command.Parameters["$TreeURL"].Value = SetCol(row.TreeURL);
                        command.Parameters["$TestGuid"].Value = row.TestGuid;
                        command.Parameters["$Confidence"].Value = row.Confidence;
                        command.Parameters["$SharedCentimorgans"].Value = row.SharedCentimorgans;
                        command.Parameters["$CreatedDate"].Value = row.CreatedDate;
                        command.Parameters["$RootsEntry"].Value = row.RootsEntry;
                        command.Parameters["$Memory"].Value = SetCol(row.Memory);
                        command.Parameters["$KitId"].Value = row.KitId;
                        command.Parameters["$Name"].Value = SetCol(row.Name);


                        command.ExecuteNonQuery();
                        idx++;
                    }

                    transaction.Commit();
                }
            }

        }

        public void BulkInsertPersons(List<Persons> rows)
        {

            var connectionString = this.Database.GetDbConnection().ConnectionString;


            using (var connection = new SqliteConnection(connectionString))
            {
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Persons(Id, ChristianName,Surname,FatherId, MotherId, BirthDate, BirthYear,BirthPlace, " +
                                      "BirthCounty,BirthCountry,DeathDate,DeathYear,DeathPlace, DeathCounty,DeathCountry, " +
                                      "Memory,CreatedDate,RootsEntry,Fix,EnglishParentsChecked,CountyUpdated, AmericanParentsChecked,CountryUpdated)" +
                                      " VALUES ($Id,$ChristianName,$Surname,$FatherId,$MotherId,$BirthDate, $BirthYear,$BirthPlace," +
                                      "$BirthCounty,$BirthCountry,$DeathDate,$DeathYear,$DeathPlace,$DeathCounty,$DeathCountry," +
                                      "$Memory,$CreatedDate,$RootsEntry,$Fix,$EnglishParentsChecked,$CountyUpdated,$AmericanParentsChecked,$CountryUpdated);";

                command.Parameters.Add("$Id", SqliteType.Integer);
                command.Parameters.Add("$ChristianName", SqliteType.Text);
                command.Parameters.Add("$Surname", SqliteType.Text);
                command.Parameters.Add("$FatherId", SqliteType.Integer);
                command.Parameters.Add("$MotherId", SqliteType.Integer);

                command.Parameters.Add("$BirthDate", SqliteType.Text);
                command.Parameters.Add("$BirthYear", SqliteType.Integer);
                command.Parameters.Add("$BirthPlace", SqliteType.Text);
                command.Parameters.Add("$BirthCounty", SqliteType.Text);
                command.Parameters.Add("$BirthCountry", SqliteType.Text);

                command.Parameters.Add("$DeathDate", SqliteType.Text);
                command.Parameters.Add("$DeathYear", SqliteType.Integer);
                command.Parameters.Add("$DeathPlace", SqliteType.Text);
                command.Parameters.Add("$DeathCounty", SqliteType.Text);
                command.Parameters.Add("$DeathCountry", SqliteType.Text);

                command.Parameters.Add("$Memory", SqliteType.Text);
                command.Parameters.Add("$CreatedDate", SqliteType.Text);
                command.Parameters.Add("$RootsEntry", SqliteType.Integer);
                command.Parameters.Add("$Fix", SqliteType.Text);

                command.Parameters.Add("$EnglishParentsChecked", SqliteType.Integer);
                command.Parameters.Add("$CountyUpdated", SqliteType.Integer);
                command.Parameters.Add("$AmericanParentsChecked", SqliteType.Integer);
                command.Parameters.Add("$CountryUpdated", SqliteType.Integer);

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    command.Transaction = transaction;
                    command.Prepare();
                    int idx = 0;
                    foreach (var row in rows)
                    {
                        command.Parameters["$Id"].Value = idx;
                        command.Parameters["$ChristianName"].Value = SetCol(row.ChristianName);
                        command.Parameters["$Surname"].Value = SetCol(row.Surname);
                        command.Parameters["$BirthYear"].Value = row.BirthYear;
                        command.Parameters["$BirthPlace"].Value = SetCol(row.BirthPlace);
                        command.Parameters["$BirthCounty"].Value = SetCol(row.BirthCounty);
                        command.Parameters["$BirthCountry"].Value = SetCol(row.BirthCountry);
                        command.Parameters["$FatherId"].Value = row.FatherId;
                        command.Parameters["$MotherId"].Value = row.MotherId;

                        command.Parameters["$BirthDate"].Value = SetCol(row.BirthDate);
                        command.Parameters["$BirthYear"].Value = row.BirthYear;
                        command.Parameters["$BirthPlace"].Value = SetCol(row.BirthPlace);
                        command.Parameters["$BirthCounty"].Value = SetCol(row.BirthCounty);
                        command.Parameters["$BirthCountry"].Value = SetCol(row.BirthCountry);

                        command.Parameters["$DeathDate"].Value = SetCol(row.DeathDate);
                        command.Parameters["$DeathYear"].Value = row.DeathYear;
                        command.Parameters["$DeathPlace"].Value = SetCol(row.DeathPlace);
                        command.Parameters["$DeathCounty"].Value = SetCol(row.DeathCounty);
                        command.Parameters["$DeathCountry"].Value = SetCol(row.DeathCountry);

                        command.Parameters["$Memory"].Value = SetCol(row.Memory);
                        command.Parameters["$CreatedDate"].Value = row.CreatedDate;
                        command.Parameters["$RootsEntry"].Value = row.RootsEntry;
                        command.Parameters["$Fix"].Value = row.Fix;

                        command.Parameters["$EnglishParentsChecked"].Value = row.EnglishParentsChecked;
                        command.Parameters["$CountyUpdated"].Value = row.CountyUpdated;
                        command.Parameters["$AmericanParentsChecked"].Value = row.AmericanParentsChecked;
                        command.Parameters["$CountryUpdated"].Value = row.CountryUpdated;



                        command.ExecuteNonQuery();
                        idx++;
                    }

                    transaction.Commit();
                }
            }

        }

        public void BulkUpdatePersonsNationality(List<long> unknownOriginsPersons,
            HashSet<long> knownNationalityParent, bool isEnglish, IProgress<string> progress)
        {
            var s = this.Database.GetDbConnection().ConnectionString;

            void UpdateCheckStatus( string table, string field)
            {
                using (var connection = new SqliteConnection(s))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE " + table + " SET "+ field + " = 1 WHERE Id = $Id;";
                    command.Parameters.Add("$Id", SqliteType.Integer);

                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        command.Transaction = transaction;
                        command.Prepare();
                        foreach (var row in unknownOriginsPersons)
                        {
                            command.Parameters["$Id"].Value = row;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }

            void UpdateBirthCountry(string table, string nationality)
            {
                using (var connection = new SqliteConnection(s))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE " + table + " SET BirthCountry = '"+ nationality + "' WHERE Id = $Id;";
                    command.Parameters.Add("$Id", SqliteType.Integer);

                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        command.Transaction = transaction;
                        command.Prepare();
                        foreach (var row in knownNationalityParent)
                        {
                            command.Parameters["$Id"].Value = row;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }

            if (isEnglish)
            {
                UpdateCheckStatus("Persons", "EnglishParentsChecked");
                progress.Report("Persons EnglishParentsChecked Checked");

                UpdateCheckStatus("TreePersons", "EnglishParentsChecked");
                progress.Report("TreePersons EnglishParentsChecked Checked");

                UpdateBirthCountry("Persons", "England");
                progress.Report("Persons Set Birth Country as England based on parents");

                UpdateBirthCountry("TreePersons", "England");
                progress.Report("TreePersons Set Birth Country as England based on parents");
            }
            else
            {
                UpdateCheckStatus("Persons", "AmericanParentsChecked");
                progress.Report("Persons AmericanParentsChecked Checked");

                UpdateCheckStatus("TreePersons", "AmericanParentsChecked");
                progress.Report("TreePersons AmericanParentsChecked Checked");

                UpdateBirthCountry("Persons", "USA");
                progress.Report("Persons Set Birth Country as USA based on parents");

                UpdateBirthCountry("TreePersons", "USA");
                progress.Report("TreePersons Set Birth Country as USA based on parents");
            }


        }

        public void BulkUpdatePersonsCounty(Dictionary<long, string> places)
        {
            var s = this.Database.GetDbConnection().ConnectionString;

            void UpdateCounty(string table)
            {
                using (var connection = new SqliteConnection(s))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE " + table + " SET BirthCounty = $county WHERE Id = $Id;";
                    command.Parameters.Add("$Id", SqliteType.Integer);
                    command.Parameters.Add("$county", SqliteType.Text);
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        command.Transaction = transaction;
                        command.Prepare();
                        foreach (var row in places)
                        {
                            command.Parameters["$Id"].Value = row.Key;
                            command.Parameters["$county"].Value = row.Value;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }

            UpdateCounty("Persons");
            UpdateCounty("TreePersons");     
        }

        public void BulkUpdatePersonsCountry(Dictionary<long, string> places)
        {
            var s = this.Database.GetDbConnection().ConnectionString;

            void UpdateCounty(string table)
            {
                using (var connection = new SqliteConnection(s))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE " + table + " SET BirthCountry = $country WHERE Id = $Id;";
                    command.Parameters.Add("$Id", SqliteType.Integer);
                    command.Parameters.Add("$country", SqliteType.Text);
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        command.Transaction = transaction;
                        command.Prepare();
                        foreach (var row in places)
                        {
                            command.Parameters["$Id"].Value = row.Key;
                            command.Parameters["$country"].Value = row.Value;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }

            UpdateCounty("Persons");
            UpdateCounty("TreePersons");
        }

        public void BulkUpdatePersonsCountyAndCountry(Dictionary<long, string> places)
        {
            var s = this.Database.GetDbConnection().ConnectionString;

            void UpdateCounty(string table)
            {
                using (var connection = new SqliteConnection(s))
                {
                    var command = connection.CreateCommand();
                    command.CommandText = "UPDATE " + table + " SET BirthCountry = $country , BirthCounty = $county WHERE Id = $Id;";
                    command.Parameters.Add("$Id", SqliteType.Integer);
                    command.Parameters.Add("$county", SqliteType.Text);
                    command.Parameters.Add("$country", SqliteType.Text);
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        command.Transaction = transaction;
                        command.Prepare();
                        foreach (var row in places)
                        {
                            command.Parameters["$Id"].Value = row.Key;

                            

                            var parts = Regex.Split(row.Value, "||");

                            command.Parameters["$county"].Value = parts[0];
                            command.Parameters["$country"].Value = parts[1];
                            

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }
            }

            UpdateCounty("Persons");
            UpdateCounty("TreePersons");
        }

        private long AddPerson(DbDataReader reader, List<PersonsOfInterest> personsOfInterests, long idx, ref long personIdOut)
        {
            long.TryParse(reader.GetValue(0).ToString(), out long personId);
            string christianName = reader.GetValue(1).ToString();
            string surname = reader.GetValue(2).ToString();
            int.TryParse(reader.GetValue(3).ToString(), out int birthYear);
            string birthPlace = reader.GetValue(4).ToString();
            string birthCounty = reader.GetValue(5).ToString();
            string birthCountry = reader.GetValue(6).ToString();
            string testDisplayName = reader.GetValue(7).ToString();
            string testAdminDisplayName = reader.GetValue(8).ToString();
            string treeURL = reader.GetValue(9).ToString();
            Guid.TryParse(reader.GetValue(10).ToString(), out Guid testGuid);
            double.TryParse(reader.GetValue(11).ToString(), out double confidence);
            double.TryParse(reader.GetValue(12).ToString(), out double sharedCM);

            DateTime createDateTime = DateTime.Today;

            //   DateTime.TryParse(reader.GetValue(13).ToString(), out createDateTime);

            bool.TryParse(reader.GetValue(14).ToString(), out bool rootsEntry);
            string memory = reader.GetValue(15).ToString();

            Guid.TryParse(reader.GetValue(16).ToString(), out Guid kitId);

            string name = reader.GetValue(17).ToString();


            if (idx == 0 || personIdOut != personId)
            {
                personsOfInterests.Add(new PersonsOfInterest()
                {
                    Id = idx,
                    BirthCountry = birthCountry,
                    BirthCounty = birthCounty,
                    BirthPlace = birthPlace,
                    BirthYear = birthYear,
                    ChristianName = christianName,
                    Confidence = confidence,
                    CreatedDate = createDateTime,
                    KitId = kitId,
                    Memory = memory,
                    Name = name,
                    PersonId = personId,
                    RootsEntry = rootsEntry,
                    SharedCentimorgans = sharedCM,
                    Surname = surname,
                    TestAdminDisplayName = testAdminDisplayName,
                    TestDisplayName = testDisplayName,
                    TestGuid = testGuid,
                    TreeURL = treeURL
                });

                idx++;
            }

            personIdOut = personId;

            return idx;
        }


    }
}
