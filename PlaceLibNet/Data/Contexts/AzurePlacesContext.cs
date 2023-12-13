using System;
using ConfigHelper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PlaceLibNet.Domain.Entities.Persistent;

namespace PlaceLibNet.Data.Contexts
{
    public partial class AzurePlacesContext  : DbContext, IPlacesContext
    {
        private IMSGConfigHelper _configObj { get; set; }

        public AzurePlacesContext(IMSGConfigHelper config)
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
       //    var connectionString = this.Database.GetDbConnection().ConnectionString;

            var nextId =  GetNextId("PlaceCache");

            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();
            command.CommandText = "Insert INTO ukp.PlaceCache(Id,AltId,Name,NameFormatted,JSONResult,Country,County,Searched,BadData,Lat,Long,Src,DateCreated) VALUES(@Id,@AltId,@Name,@NameFormatted,@JSONResult,@Country,@County,@Searched,@BadData,@Lat,@Lon, @Src, @DateCreated)";

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
            
            command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = nextId });
            command.Parameters.Add(new SqlParameter { ParameterName = "@AltId", Value = nextId });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Name", Value = name });
            command.Parameters.Add(new SqlParameter { ParameterName = "@NameFormatted", Value = nameFormatted });
            command.Parameters.Add(new SqlParameter { ParameterName = "@JSONResult", Value = jsonResult });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Country", Value = country });
            command.Parameters.Add(new SqlParameter { ParameterName = "@County", Value = county ?? "" });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Searched", Value = searched });
            command.Parameters.Add(new SqlParameter { ParameterName = "@BadData", Value = badData });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Lat", Value = lat ?? "" });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Lon", Value = @long ?? "" });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Src", Value = src ?? "" });
            command.Parameters.Add(new SqlParameter { ParameterName = "@DateCreated", Value = DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss") });
            

            command.ExecuteNonQuery();

            transaction.Commit();
             
        }
         
        
        public void UpdateFormattedName(int id, string name)
        {
            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE ukp.PlaceCache SET NameFormatted = @NameFormatted WHERE Id = @Id;";
            
            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = id });
            command.Parameters.Add(new SqlParameter { ParameterName = "@NameFormatted", Value = name });

            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdatePlaceCacheLatLong(PlaceCache placeCache)
        {
            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE ukp.PlaceCache SET Lat = @Lat, Long = @Lon, BadData = @BadData, County = @County, Country = @Country, Src = @Src WHERE Id = @Id;";
 
            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

            command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = placeCache.Id });
            command.Parameters.Add(new SqlParameter { ParameterName = "@BadData", Value = 0 });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Lat", Value = placeCache.Lat });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Lon", Value = placeCache.Long });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Src", Value = placeCache.Src });
            command.Parameters.Add(new SqlParameter { ParameterName = "@County", Value = placeCache.County });
            command.Parameters.Add(new SqlParameter { ParameterName = "@Country", Value = placeCache.Country });
            
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdateBadData(int id, bool badData)
        {

            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE ukp.PlaceCache SET BadData = @BadData WHERE Id = @Id;";

         
            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();

             
            command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = id});
            command.Parameters.Add(new SqlParameter { ParameterName = "@BadData", Value = badData ? 1 : 0 });


            command.ExecuteNonQuery();

            transaction.Commit();

        }

        public void UpdateJSONCacheResult(int id, string results)
        {

            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();

            command.CommandText = "UPDATE ukp.PlaceCache SET JSONResult = @JSONResult, Country = '', County = '', Searched = 1, BadData = 1 WHERE Id = @Id;";

            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
             
            command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = id });
            command.Parameters.Add(new SqlParameter { ParameterName = "@JSONResult", Value = results  });

            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdateCounty(int id, string county)
        {
            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();
            command.CommandText = "UPDATE ukp.PlaceCache SET County = @County WHERE Id = @Id;";
            
            connection.Open();

            using var transaction = connection.BeginTransaction();

            command.Transaction = transaction;
            command.Prepare();
             
            command.Parameters.Add(new SqlParameter { ParameterName = "@Id", Value = id });
            command.Parameters.Add(new SqlParameter { ParameterName = "@County", Value = county });
            
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public int GetNextId(string tableName)
        {
            using var connection = new SqlConnection(_configObj.MSGGenDB01);

            var command = connection.CreateCommand();
            command.CommandText = "SELECT MAX(Id) from ukp." + tableName + ";";

            connection.Open();

            command.Prepare();


            var r = command.ExecuteScalar();

            int nextId = 0;



            if (r != null && r.GetType().Name != "DBNull")
            {
                nextId = Convert.ToInt32(r);
            }
             
            connection.Close();

            nextId++;

            return nextId;
        }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configObj.MSGGenDB01);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Places>(entity =>
            {
                entity.ToTable("Places", "UKP");

                entity.Property(e => e.Id).ValueGeneratedNever(); 

            });

            modelBuilder.Entity<PlaceCache>(entity =>
            {
                entity.ToTable("PlaceCache", "UKP");

                entity.Property(e => e.Id).ValueGeneratedNever();

            });

        }


    }
}
