using System;
using System.Collections.Generic;
using DNAGedLib.Models;
using Microsoft.Data.Sqlite;

namespace DNAGedLib
{
    public class ImportStageICW : ImportStage
    {        
        public ImportStageICW(ImportDataStore personImporter) {
            base.personImporter = personImporter;
        }
        public override Response Import()
        {
            void Ancestry_ICW(SqliteConnection mDbConnection, List<ICW> matchTreeEntries)
            {

                string sql = "select * from Ancestry_ICW";
                var command = new SqliteCommand(sql, mDbConnection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    matchTreeEntries.Add(new ICW()
                    {
                        Id = Convert.ToInt32(reader["Id"]),

                        MatchId = MatchTreeHelpers.ExtractGuid(reader["matchid"]),

                        MatchName = reader["matchname"].ToString(),

                        MatchAdmin = reader["matchadmin"].ToString(),

                        Icwid = MatchTreeHelpers.ExtractGuid(reader["icwid"]),

                        Icwname = reader["icwname"].ToString(),

                        Icwadmin = reader["icwadmin"].ToString(),

                        Source = reader["source"].ToString(),
                    });
                }
            }

            var m_dbConnection =
                new SqliteConnection("Data Source=" + personImporter.Path);
            m_dbConnection.Open();

            Console.WriteLine("Loading SQLLite DB");


            Ancestry_ICW(m_dbConnection, personImporter.ICWs);

            long counter = 0;
            long percentage = 0;

            int total = personImporter.ICWs.Count;
            counter = 0;
            percentage = 0;

            Console.WriteLine("adding icw: " + total);

            int addedRecords = 0;

            foreach (var i in personImporter.ICWs)
            {
                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;


                //              if (dnagedContext.MatchIcw.Any(p => p.MatchId == i.MatchId && p.Icwid == i.Icwid)) continue;

                personImporter.DNAGedContext.MatchIcw.Add(new MatchIcw()
                {
                    Id = i.Id,
                    MatchId = i.MatchId,
                    Icwid = i.Icwid
                });
            }


            personImporter.DNAGedContext.SaveChanges();


           // Console.WriteLine("saved " + addedRecords + " new records");

            return new Response()
            {
                State =  ReturnState.Success,
                Message = "saved " + addedRecords + " new records"
            };
        }

    }






}
