using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace DNAGedLib
{
    public class ReadDNAGedComSQLLiteDB : ImportStage
    {
        public ReadDNAGedComSQLLiteDB(ImportationContext personImporter)
        {
            base.personImporter = personImporter;
        }
        public override Response Import()
        {
            string path = base.personImporter.Path;// @"C:\Users\george\Documents\DNAGedcom.db";

            void matchTrees(SqliteConnection mDbConnection, List<MatchTreeEntry> matchTreeEntries)
            {
                string sql = @"select * from Ancestry_matchTrees where matchid in (select matchguid from ancestry_matchgroups where testguid like '" + base.personImporter.ImportTestId + "')";

                Debug.WriteLine(sql);
                var command = new SqliteCommand(sql, mDbConnection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    matchTreeEntries.Add(new MatchTreeEntry()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MatchId = MatchTreeHelpers.ExtractGuid(reader["matchid"]),
                        GivenName = reader["given"].ToString(),
                        Surname = reader["surname"].ToString(),
                        BirthString = reader["birthdate"].ToString(),
                        BirthInt = MatchTreeHelpers.ExtractYear(reader["birthdate"]),
                        BirthPlace = reader["birthplace"].ToString(),
                        DeathString = reader["deathdate"].ToString(),
                        DeathInt = MatchTreeHelpers.ExtractYear(reader["deathdate"]),
                        DeathPlace = reader["deathplace"].ToString(),
                        RelId = MatchTreeHelpers.ExtractInt(reader["deathplace"]),
                        PersonId = MatchTreeHelpers.ExtractLong(reader["personId"]),
                        FatherId = MatchTreeHelpers.ExtractLong(reader["fatherId"]),
                        MotherId = MatchTreeHelpers.ExtractLong(reader["motherId"]),
                        CreatedDate = MatchTreeHelpers.ExtractDate(reader["created_date"]),
                        Source = reader["deathplace"].ToString(),
                    });
                }
            }

            void Ancestry_ICW(SqliteConnection mDbConnection, List<ICW> matchTreeEntries)
            {
                string subQuery = "select matchGuid from Ancestry_matchGroups where testGuid like '" + base.personImporter.ImportTestId + "'";
                string sql = "select * from Ancestry_ICW where matchid in (" + subQuery + ")";
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

            void Ancestry_MatchGroups(SqliteConnection mDbConnection, List<MatchGroups> matchTreeEntries)
            {
                string sql = "select * from Ancestry_matchGroups where testGuid like '" + base.personImporter.ImportTestId + "'";
                var command = new SqliteCommand(sql, mDbConnection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    matchTreeEntries.Add(new MatchGroups()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MatchGuid = MatchTreeHelpers.ExtractGuid(reader["matchGuid"]),
                        TestGuid = MatchTreeHelpers.ExtractGuid(reader["testGuid"]),
                        SharedSegment = MatchTreeHelpers.ExtractInt(reader["sharedSegment"]),
                        Confidence = MatchTreeHelpers.ExtractDouble(reader["confidence"]),
                        GroupName = reader["groupName"].ToString(),

                        HasHint = MatchTreeHelpers.ExtractBool(reader["hasHint"]),
                        Note = reader["note"].ToString(),

                        SharedCentimorgans = MatchTreeHelpers.ExtractDouble(reader["sharedCentimorgans"]),
                        Starred = MatchTreeHelpers.ExtractBool(reader["starred"]),
                        TestAdminDisplayName = reader["matchTestAdminDisplayName"].ToString(),
                        TestDisplayName = reader["matchTestDisplayName"].ToString(),
                        TreeId = reader["matchTreeId"].ToString(),
                        TreeNodeCount = MatchTreeHelpers.ExtractInt(reader["matchTreeNodeCount"]),
                        TreesPrivate = MatchTreeHelpers.ExtractBool(reader["matchTreeIsPrivate"]),
                        UserPhoto = MatchTreeHelpers.ExtractBool(reader["userPhoto"]),
                        Viewed = MatchTreeHelpers.ExtractBool(reader["viewed"])
                    });
                }
            }

            void Ancestry_MatchDetail(SqliteConnection mDbConnection, List<MatchDetail> matchTreeEntries)
            {
                string sql = "select * from Ancestry_matchDetail";
                var command = new SqliteCommand(sql, mDbConnection);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    matchTreeEntries.Add(new MatchDetail()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        MatchGuid = MatchTreeHelpers.ExtractGuid(reader["matchGuid"]),
                        TestGuid = MatchTreeHelpers.ExtractGuid(reader["testGuid"]),
                        SharedSegment = MatchTreeHelpers.ExtractInt(reader["sharedSegments"])
                    });
                }
            }

            var m_dbConnection =
                new SqliteConnection("Data Source=" + path);
            m_dbConnection.Open();

            Console.WriteLine("Loading SQLLite DB");

            matchTrees(m_dbConnection, base.personImporter.SQLliteTrees);
            Ancestry_ICW(m_dbConnection, base.personImporter.ICWs);
            Ancestry_MatchDetail(m_dbConnection, base.personImporter.MatchDetails);
            Ancestry_MatchGroups(m_dbConnection, base.personImporter.MatchGroups);


            var m = base.personImporter.SQLliteTrees.Count 
                + " " + base.personImporter.ICWs.Count + " " 
                + base.personImporter.MatchDetails.Count + " " + base.personImporter.MatchGroups.Count;


            return new Response()
            {
                State = ReturnState.Success,
                Message = m
            };
        }

    }






}
