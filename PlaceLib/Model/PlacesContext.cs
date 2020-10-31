using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PlaceLib.Model
{
    public partial class PlacesContext : DbContext
    {
        public PlacesContext()
        {
        }

        public PlacesContext(DbContextOptions<PlacesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Places> Places { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //                optionsBuilder.UseSqlServer("Server=DESKTOP-KGS70RI\\SQL2016EX;Database=UKPlaces;Trusted_Connection=True;");

                optionsBuilder.UseSqlite(@"Data Source=C:\Users\george\Documents\placeDB.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "3.0.0-preview.18572.1");

            modelBuilder.Entity<Places>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Bua11cd).IsUnicode(false);

                entity.Property(e => e.Ctry15nm).IsUnicode(false);

                entity.Property(e => e.Cty15cd).IsUnicode(false);

                entity.Property(e => e.Cty15nm).IsUnicode(false);

                entity.Property(e => e.Ctyhistnm).IsUnicode(false);

                entity.Property(e => e.Ctyltnm).IsUnicode(false);

                entity.Property(e => e.Descnm).IsUnicode(false);

                entity.Property(e => e.Eer15cd).IsUnicode(false);

                entity.Property(e => e.Eer15nm).IsUnicode(false);

                entity.Property(e => e.Fid).IsUnicode(false);

                entity.Property(e => e.Grid1km).IsUnicode(false);

                entity.Property(e => e.Gridgb1e).IsUnicode(false);

                entity.Property(e => e.Gridgb1m).IsUnicode(false);

                entity.Property(e => e.Gridgb1n).IsUnicode(false);

                entity.Property(e => e.Hlth12cd).IsUnicode(false);

                entity.Property(e => e.Hlth12nm).IsUnicode(false);

                entity.Property(e => e.Lad15cd).IsUnicode(false);

                entity.Property(e => e.Lad15nm).IsUnicode(false);

                entity.Property(e => e.Laddescnm).IsUnicode(false);

                entity.Property(e => e.Lat).IsUnicode(false);

                entity.Property(e => e.Long).IsUnicode(false);

                entity.Property(e => e.Npark15cd).IsUnicode(false);

                entity.Property(e => e.Npark15nm).IsUnicode(false);

                entity.Property(e => e.Par15cd).IsUnicode(false);

                entity.Property(e => e.Pcon15cd).IsUnicode(false);

                entity.Property(e => e.Pcon15nm).IsUnicode(false);

                entity.Property(e => e.Pfa15cd).IsUnicode(false);

                entity.Property(e => e.Pfa15nm).IsUnicode(false);

                entity.Property(e => e.Place15cd).IsUnicode(false);

                entity.Property(e => e.Place15nm).IsUnicode(false);

                entity.Property(e => e.Placeid).ValueGeneratedOnAdd();

                entity.Property(e => e.Placesort).IsUnicode(false);

                entity.Property(e => e.Popcnt).IsUnicode(false);

                entity.Property(e => e.Regd15cd).IsUnicode(false);

                entity.Property(e => e.Regd15nm).IsUnicode(false);

                entity.Property(e => e.Rgn15cd).IsUnicode(false);

                entity.Property(e => e.Rgn15nm).IsUnicode(false);

                entity.Property(e => e.Splitind).IsUnicode(false);

                entity.Property(e => e.Wd15cd).IsUnicode(false);
            });
        }
    }
}
