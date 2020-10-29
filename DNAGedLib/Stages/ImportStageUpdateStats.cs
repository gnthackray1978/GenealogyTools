using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GenDBContext.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DNAGedLib
{
    public class CreationItem
    {
        public DateTime CreatedDate { get; set; }
        public Guid TestGuid { get; set; }
    }

    public class ImportStageUpdateStats : ImportStage
    {
        public ImportStageUpdateStats(ImportationContext personImporter)
        {
            base.personImporter = personImporter;
        }

   


        public override Response Import()
        {
            UpdateKitSqlite();


           // UpdateSqlServer();

            return new Response()
            {
                State = ReturnState.Success,
                Message = "Updated"
            };
        }

        private void UpdateSqlServer()
        {
            using (var conn = new SqlConnection(base.personImporter.Destination))
            using (var command = new SqlCommand("UpdateKits", conn)
            {
                CommandType = CommandType.StoredProcedure
            })
            {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateKitSqlite()
        {
            DNAGEDContext dnagedContext = new DNAGEDContext();

            string sqlString =
                @"SELECT distinct MatchTrees.CreatedDate, MatchGroups.TestGuid FROM MatchTrees INNER JOIN  MatchGroups ON MatchTrees.MatchId = MatchGroups.MatchGuid";

            var createdItems = new List<CreationItem>();

            using (var connection = new SqliteConnection(dnagedContext.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();

                using (var command = new SqliteCommand(sqlString, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        long idx = 0;
                        while (reader.Read())
                        {
                            DateTime createDateTime = DateTime.Today;

                            DateTime.TryParse(reader.GetValue(13).ToString(), out createDateTime);

                            Guid.TryParse(reader.GetValue(1).ToString(), out Guid testGuid);

                            createdItems.Add(new CreationItem() {CreatedDate = createDateTime, TestGuid = testGuid});
                        }
                    }
                }
            }

            foreach (var kit in dnagedContext.MatchKitName)
            {
                var createdDate = createdItems.Where(w => w.TestGuid == kit.Id).OrderByDescending(o => o.CreatedDate)
                    .FirstOrDefault();
                if (createdDate != null)
                {
                    kit.LastUpdated = createdDate.CreatedDate;
                }

                var count = createdItems.Count(c => c.TestGuid == kit.Id);

                kit.PersonCount = count;
            }


            dnagedContext.SaveChanges();
        }
    }






}
