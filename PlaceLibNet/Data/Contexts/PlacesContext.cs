using System;
using ConfigHelper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PlaceLibNet.Domain.Entities.Persistent;

namespace PlaceLibNet.Data.Contexts
{
    public interface IPlacesContext
    {
        DbSet<Places> Places { get; set; }
        DbSet<PlaceCache> PlaceCache { get; set; }

        void InsertPlaceCache(int id,
            string name,
            string nameFormatted,
            string jsonResult,
            string country,
            string county,
            bool searched,
            bool badData,
            string lat,
            string @long,
            string src);

        void UpdateFormattedName(int id, string name);
        void UpdatePlaceCacheLatLong(PlaceCache placeCache);
        void UpdateBadData(int id, bool badData);
        void UpdateJSONCacheResult(int id, string results);
        void UpdateCounty(int id, string county);
    }

    public partial class PlacesContext  : DbContext, IPlacesContext
    {
        private IMSGConfigHelper _configObj { get; set; }

        public PlacesContext(IMSGConfigHelper config)
        {
            _configObj = config;
        }

        //public PlacesContext(DbContextOptions<PlacesContext> options)
        //    : base(options)
        //{
        //}

        public virtual DbSet<Places> Places { get; set; }
        public virtual DbSet<PlaceCache> PlaceCache { get; set; }

        public void InsertPlaceCache(int id,
            string name,
            string nameFormatted,
            string jsonResult,
            string country,
            string county,
            bool searched,
            bool badData,
            string lat,
            string @long,
            string src)
        {
            
            var connectionString = Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "Insert INTO PlaceCache(Id,AltId,Name,NameFormatted,JSONResult,Country,County,Searched,BadData,Lat,Long,Src,DateCreated) VALUES($Id,$AltId,$Name,$NameFormatted,$JSONResult,$Country,$County,$Searched,$BadData,$Lat,$Lon, $Src, $DateCreated)";
            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$AltId", SqliteType.Integer);
            command.Parameters.Add("$Name", SqliteType.Text);
            command.Parameters.Add("$NameFormatted", SqliteType.Text);
            command.Parameters.Add("$JSONResult", SqliteType.Text);
            command.Parameters.Add("$Country", SqliteType.Text);
            command.Parameters.Add("$County", SqliteType.Text);
            command.Parameters.Add("$Searched", SqliteType.Text);
            command.Parameters.Add("$BadData", SqliteType.Text);
            command.Parameters.Add("$Lat", SqliteType.Text);
            command.Parameters.Add("$Lon", SqliteType.Text);
            command.Parameters.Add("$Src", SqliteType.Text);
            command.Parameters.Add("$DateCreated", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = id;
            command.Parameters["$AltId"].Value = id;
            command.Parameters["$Name"].Value = name;
            command.Parameters["$NameFormatted"].Value = nameFormatted;
            command.Parameters["$JSONResult"].Value = jsonResult;
            command.Parameters["$Country"].Value = country;
            command.Parameters["$County"].Value = county ?? "";
            command.Parameters["$Searched"].Value = searched;
            command.Parameters["$BadData"].Value = badData;
            command.Parameters["$Lat"].Value = lat ?? "";
            command.Parameters["$Lon"].Value = @long ?? "";
            command.Parameters["$Src"].Value = src ?? "";
            command.Parameters["$DateCreated"].Value = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss");

            command.ExecuteNonQuery();

            transaction.Commit();

        }
        
        public void UpdateFormattedName(int id, string name)
        {
            var connectionString = Database.GetDbConnection().ConnectionString;

            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE PlaceCache SET NameFormatted = $NameFormatted WHERE Id = $Id;";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$NameFormatted", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = id;
            command.Parameters["$NameFormatted"].Value = name;

            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdatePlaceCacheLatLong(PlaceCache placeCache)
        {
            var connectionString = Database.GetDbConnection().ConnectionString;

            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE PlaceCache SET Lat = $Lat, Long = $Lon, BadData = $BadData, County = $County, Country = $Country, Src = $Src WHERE Id = $Id;";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$BadData", SqliteType.Integer);
            command.Parameters.Add("$Lat", SqliteType.Text);
            command.Parameters.Add("$Lon", SqliteType.Text);
            command.Parameters.Add("$Src", SqliteType.Text);
            command.Parameters.Add("$County", SqliteType.Text);
            command.Parameters.Add("$Country", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = placeCache.Id;
            command.Parameters["$Lat"].Value = placeCache.Lat;
            command.Parameters["$Lon"].Value = placeCache.Long;
            command.Parameters["$BadData"].Value = 0;
            command.Parameters["$Src"].Value = placeCache.Src;
            command.Parameters["$County"].Value = placeCache.County;
            command.Parameters["$Country"].Value = placeCache.Country;


            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdateBadData(int id, bool badData)
        {

            var connectionString = Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE PlaceCache SET BadData = $BadData WHERE Id = $Id;";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$BadData", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = id;
            command.Parameters["$BadData"].Value = badData ? 1 : 0;

            command.ExecuteNonQuery();

            transaction.Commit();

        }

        public void UpdateJSONCacheResult(int id, string results)
        {

            var connectionString = Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE PlaceCache SET JSONResult = $JSONResult, Country = '', County = '', Searched = 1, BadData = 1 WHERE Id = $Id;";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$JSONResult", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = id;
            command.Parameters["$JSONResult"].Value = results;

            command.ExecuteNonQuery();

            transaction.Commit();

        }

        public void UpdateCounty(int id, string county)
        {

            var connectionString = Database.GetDbConnection().ConnectionString;


            using var connection = new SqliteConnection(connectionString);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE PlaceCache SET County = $County WHERE Id = $Id;";

            command.Parameters.Add("$Id", SqliteType.Integer);
            command.Parameters.Add("$County", SqliteType.Text);

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters["$Id"].Value = id;
            command.Parameters["$County"].Value = county;

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
