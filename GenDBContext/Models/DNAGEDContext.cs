using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using DNAGedLib.Models;
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

    public partial class DNAGEDContext : DbContext
    {


        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddProvider(new DebugLoggerProvider()); });

        public virtual DbSet<MatchDetail> MatchDetail { get; set; }
        public virtual DbSet<MatchGroups> MatchGroups { get; set; }
        public virtual DbSet<MatchIcw> MatchIcw { get; set; }
        public virtual DbSet<MatchTrees> MatchTrees { get; set; }
        public virtual DbSet<Persons> Persons { get; set; }
        public virtual DbSet<MyPersons> MyPersons { get; set; }
        public virtual DbSet<PersonsOfInterest> PersonsOfInterest { get; set; }
        public virtual DbSet<PersonGroups> PersonGroups { get; set; }
        public virtual DbSet<MatchKitName> MatchKitName { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  optionsBuilder.UseLoggerFactory(MyLoggerFactory);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-KGS70RI\SQL2016EX;Initial Catalog=DNAGED;Integrated Security=SSPI;");
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

            modelBuilder.Entity<MyPersons>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

            });

            modelBuilder.Entity<PersonsOfInterest>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

            });
        }
    }
}
