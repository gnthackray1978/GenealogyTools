using System;
using System.Linq;
using GenDBContext.Models;
using Microsoft.Data.SqlClient;

namespace DNAGedLib
{
    public class ImportStageKitStats : ImportStage
    {
        public ImportStageKitStats(ImportationContext personImporter)
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
         //   string connectionString = @"Data Source=DESKTOP-KGS70RI\SQL2016EX;Initial Catalog=DNAGED;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            int result = 0;
            using (SqlConnection connection = new SqlConnection(base.personImporter.Destination))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(
                    "SELECT count(*) as PersonNo FROM dbo.MatchTrees INNER JOIN dbo.MatchGroups ON dbo.MatchTrees.MatchId = dbo.MatchGroups.MatchGuid WHERE dbo.MatchGroups.TestGuid LIKE @TestGuid", connection))
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
