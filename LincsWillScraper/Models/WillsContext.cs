using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LincsWillScraper.Models
{
    public partial class WillsContext : DbContext
    {
        public WillsContext()
        {
        }

        public WillsContext(DbContextOptions<WillsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LincsWills> LincsWills { get; set; }

        public virtual DbSet<NorfolkWillsRaw> NorfolkWillsRaw { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-KGS70RI\\SQL2016EX;Initial Catalog=Wills;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-preview3-35497");

            modelBuilder.Entity<LincsWills>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Collection).IsUnicode(false);

                entity.Property(e => e.DateString).HasMaxLength(500);

                entity.Property(e => e.Description).IsUnicode(false);
                entity.Property(e => e.FirstName).IsUnicode(false);
                entity.Property(e => e.Surname).IsUnicode(false);
                entity.Property(e => e.Aliases).IsUnicode(false);
                entity.Property(e => e.Occupation).IsUnicode(false);


                entity.Property(e => e.Place).IsUnicode(false);

                entity.Property(e => e.Reference).IsUnicode(false);

                entity.Property(e => e.Url).IsUnicode(false);
            });

            modelBuilder.Entity<NorfolkWillsRaw>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Link).IsUnicode(false);
                entity.Property(e => e.Title).IsUnicode(false);
                entity.Property(e => e.DateRange).IsUnicode(false);
                entity.Property(e => e.CatalogueRef).IsUnicode(false);             

            });
        }
    }
}
