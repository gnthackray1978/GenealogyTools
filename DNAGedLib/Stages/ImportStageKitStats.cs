using System;
using System.Linq;
using GenDBContextNET.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DNAGedLib
{
    public class ImportStageKitStats : ImportStage
    {
        public ImportStageKitStats(ImportDataStore personImporter)
        {
            base.personImporter = personImporter;
            
        }
        public override Response Import()
        {
            int records = GetTestKitRecords();

            Console.WriteLine("Updating Kit Stats");

            using (var dnagedContext = new DNAGEDContext())
            {
                var matchKit = dnagedContext.MatchKitName
                    .FirstOrDefault(f => f.Id == personImporter.ImportTestId);

                if (matchKit != null)
                {
                    matchKit.LastUpdated = DateTime.Now;
                    matchKit.PersonCount = records;
                }

                dnagedContext.SaveChanges();

                Console.WriteLine("Saved Kit Stats");
            }

            return new Response()
            {
                State = ReturnState.Success,
                Message = "Saved Kit Stats"
            };


        }


        private int GetTestKitRecords()
        {     
            int result = 0;

            //result = GetBySqlServer(result);
            result = GetBySqlite(result);

            return result;
        }

        private int GetBySqlite(int result)
        {
            var dnagedContext = new DNAGEDContext();

            using (var connection = new SqliteConnection(dnagedContext.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand(
                    @"SELECT count(*) as PersonNo FROM MatchTrees INNER JOIN MatchGroups ON MatchTrees.MatchId = MatchGroups.MatchGuid WHERE MatchGroups.TestGuid LIKE @TestGuid",                    
                    connection))
                {
                    command.Parameters.Add(new SqlParameter("TestGuid", base.personImporter.ImportTestId));

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            return result;
        }

        private int GetBySqlServer(int result)
        {
            using (SqlConnection connection = new SqlConnection(base.personImporter.Destination))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT count(*) as PersonNo FROM dbo.MatchTrees INNER JOIN dbo.MatchGroups ON dbo.MatchTrees.MatchId = dbo.MatchGroups.MatchGuid WHERE dbo.MatchGroups.TestGuid LIKE @TestGuid",
                    connection))
                {
                    command.Parameters.Add(new SqlParameter("TestGuid", base.personImporter.ImportTestId));

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
            }

            return result;
        }
    }






}
