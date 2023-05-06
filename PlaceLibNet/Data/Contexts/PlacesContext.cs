using System;
using System.Linq;
using System.Text.RegularExpressions;
using ConfigHelper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PlaceLib.Model;

namespace PlaceLibNet.Data.Contexts
{
    public partial class PlacesContext : DbContext
    {
        private IMSGConfigHelper _configObj { get; set; }

        public PlacesContext(IMSGConfigHelper config)
        {
            _configObj = config;
        }

        public PlacesContext(DbContextOptions<PlacesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<FtmPlaceCache> FTMPlaceCache { get; set; }

        public void InsertFTMPlaceCache(int id, int placeId,
            string FTMOrginalName,
            string FTMOrginalNameFormatted,
            string JSONResult,
            string Country,
            string County,
            bool Searched,
            bool BadData,
            string Lat,
            string Long,
            string Src)
        {



            var connectionString = Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "Insert INTO FTMPlaceCache(Id,FTMPlaceId,FTMOrginalName,FTMOrginalNameFormatted,JSONResult,Country,County,Searched,BadData,Lat,Long,Src) VALUES($Id,$FTMPlaceId,$FTMOrginalName,$FTMOrginalNameFormatted,$JSONResult,$Country,$County,$Searched,$BadData,$Lat,$Lon, $Src)";
            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$FTMPlaceId", SqliteType.Integer);
            command.Parameters.Add("$FTMOrginalName", SqliteType.Text);
            command.Parameters.Add("$FTMOrginalNameFormatted", SqliteType.Text);
            command.Parameters.Add("$JSONResult", SqliteType.Text);
            command.Parameters.Add("$Country", SqliteType.Text);
            command.Parameters.Add("$County", SqliteType.Text);
            command.Parameters.Add("$Searched", SqliteType.Text);
            command.Parameters.Add("$BadData", SqliteType.Text);
            command.Parameters.Add("$Lat", SqliteType.Text);
            command.Parameters.Add("$Lon", SqliteType.Text);
            command.Parameters.Add("$Src", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = id;
            command.Parameters["$FTMPlaceId"].Value = placeId;
            command.Parameters["$FTMOrginalName"].Value = FTMOrginalName;
            command.Parameters["$FTMOrginalNameFormatted"].Value = FTMOrginalNameFormatted;
            command.Parameters["$JSONResult"].Value = JSONResult;
            command.Parameters["$Country"].Value = Country;
            command.Parameters["$County"].Value = County ?? "";
            command.Parameters["$Searched"].Value = Searched;
            command.Parameters["$BadData"].Value = BadData;
            command.Parameters["$Lat"].Value = Lat ?? "";
            command.Parameters["$Lon"].Value = Long ?? "";
            command.Parameters["$Src"].Value = Src ?? "";

            command.ExecuteNonQuery();

            transaction.Commit();

        }

        public void UpdateFTMPlaceCacheLatLong(int placeId, string lat, string lon)
        {

            var connectionString = Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE FTMPlaceCache SET Lat = $Lat, Long = $Lon WHERE FTMPlaceId = $FTMPlaceId;";

            command.Parameters.Add("$FTMPlaceId", SqliteType.Integer);
            command.Parameters.Add("$Lat", SqliteType.Text);
            command.Parameters.Add("$Lon", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$FTMPlaceId"].Value = placeId;
            command.Parameters["$Lat"].Value = lat;
            command.Parameters["$Lon"].Value = lon;

            command.ExecuteNonQuery();

            transaction.Commit();

        }
        public void UpdateFTMPlaceCache(int placeId, string results)
        {

            var connectionString = Database.GetDbConnection().ConnectionString;


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



        private string GetCon()
        {
            return _configObj.PlaceConString;
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
