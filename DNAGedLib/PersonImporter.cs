using System;
using System.Collections.Generic;
using System.Linq;
using DNAGedLib.Models;
using Microsoft.Data.Sqlite;
using PlaceLib;

namespace DNAGedLib
{
    public class PersonImporter {


        public List<MatchTreeEntry> Trees { get; set; } = new List<MatchTreeEntry>();

        public List<ICW> ICWs { get; set; } = new List<ICW>();

        public List<MatchDetail> MatchDetails { get; set; } = new List<MatchDetail>();

        public List<MatchGroups> MatchGroups { get; set; } = new List<MatchGroups>();


        public void Load() {
       

            void matchTrees(SqliteConnection mDbConnection, List<MatchTreeEntry> matchTreeEntries)
            {
                string sql = "select * from Ancestry_matchTrees";
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

            void Ancestry_MatchGroups(SqliteConnection mDbConnection, List<MatchGroups> matchTreeEntries)
            {
                string sql = "select * from Ancestry_matchGroups";
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

      

            string path = @"C:\Users\george\Documents\DNAGedcom.db";

            var m_dbConnection =
                new SqliteConnection("Data Source="+ path);
            m_dbConnection.Open();

            Console.WriteLine("Loading SQLLite DB");

            matchTrees(m_dbConnection, Trees);
            Ancestry_ICW(m_dbConnection, ICWs);
            Ancestry_MatchDetail(m_dbConnection, MatchDetails);
            Ancestry_MatchGroups(m_dbConnection, MatchGroups);


            Console.WriteLine(Trees.Count + " " + ICWs.Count + " " + MatchDetails.Count + " " + MatchGroups.Count);
        }

        

        public void Export(Guid importTestId, DateTime cutOff) {
       

            DNAGEDContext dnagedContext = new DNAGEDContext();
            

            ImportPeople(dnagedContext, cutOff);

            ImportMatchTrees(dnagedContext);

            ImportICW(dnagedContext);

            ImportMatchDetailsWithGroups(dnagedContext, importTestId);

            // UpdateLocations();

            Console.WriteLine("finished");
        }



        private void ImportICW(DNAGEDContext dnagedContext)
        {
            long counter = 0;
            long percentage = 0;

            int total = this.ICWs.Count;
            counter = 0;
            percentage = 0;

            Console.WriteLine("adding icw: " + total);

            int addedRecords = 0;

            foreach (var i in this.ICWs)
            {
                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;


                if (dnagedContext.MatchIcw.Any(p => p.MatchId == i.MatchId && p.Icwid == i.Icwid)) continue;

                dnagedContext.MatchIcw.Add(new MatchIcw()
                {
                    Id = i.Id,
                    MatchId = i.MatchId,
                    Icwid = i.Icwid
                });
            }


            dnagedContext.SaveChanges();


            Console.WriteLine("saved " + addedRecords + " new records");
        }

        private void ImportMatchTrees(DNAGEDContext dnagedContext)
        {
            long counter = 0;
            long percentage = 0;


            var treesContextHash = dnagedContext.MatchTrees.Select(s => s.PersonId).Distinct().ToHashSet();

            var treesSQLLiteHash = Trees.Select(t => t.PersonId).Distinct().ToHashSet();

            // all ids not in treesContextHash that are in treesSQLLiteHash
            Console.WriteLine(treesSQLLiteHash.Count + " Entries in SQL Lite DB");
            Console.WriteLine(treesContextHash.Count + " Entries in SQL SERVER");


            treesSQLLiteHash.ExceptWith(treesContextHash);

            Console.WriteLine(treesSQLLiteHash.Count - treesContextHash.Count + " Correct Number");

            Console.WriteLine(treesSQLLiteHash.Count + " Entries in TO ADD");


            int intpercentage = 0;

            Console.WriteLine("adding entries ..");

            var guidList = dnagedContext.MatchTrees.Select(s => s.MatchId).ToHashSet();

            long addedRecords = 0;
            var total = treesSQLLiteHash.Count;
            foreach (var s in Trees.Where(w => treesSQLLiteHash.Contains(w.PersonId)))
            {
                intpercentage = (int) ((counter / total) * 100);

                if (counter % 1000 == 0)
                    Console.Write("\r" + intpercentage + " %   ");


                counter++;

                dnagedContext.MatchTrees.Add(new MatchTrees
                {
                    Id = s.Id,
                    PersonId = s.PersonId,
                    RelId = s.RelId,
                    MatchId = s.MatchId,
                    CreatedDate = DateTime.Now
                });

                addedRecords++;

                guidList.Add(s.MatchId);
            }


            dnagedContext.SaveChanges();


            Console.WriteLine("saved " + addedRecords + " new records");
        }

        private DateTime ImportPeople(DNAGEDContext dnagedContext, DateTime cutOff)
        {
            #region adding persons

            


            var personsAdded = dnagedContext.Persons.Select(p => p.Id).ToHashSet();

            // HashSet<long> personHashSet = new HashSet<long>();


            var filteredlist = this.Trees.Where(l => l.CreatedDate > cutOff).ToList();

            //&& !personsAdded.Contains(l.PersonId)


            Console.WriteLine("adding persons: " + filteredlist.Count);

            double total = filteredlist.Count;
            double counter = 0;
            double percentage = 0.0;

            int addedRecords = 0;

            foreach (var s in filteredlist)
            {
                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;

                if (personsAdded.Contains(s.PersonId)) continue;

                dnagedContext.Persons.Add(new Persons
                {
                    Id = s.PersonId,
                    ChristianName = s.GivenName,
                    Surname = s.Surname,
                    DeathPlace = s.DeathPlace,
                    DeathDate = s.DeathString,
                    DeathYear = s.DeathInt,
                    BirthDate = s.BirthString,
                    BirthPlace = s.BirthPlace,
                    BirthYear = s.BirthInt,
                    BirthCountry = "Unknown",
                    DeathCountry = "Unknown",
                    FatherId = s.FatherId,
                    MotherId = s.MotherId,
                });

                addedRecords++;
                personsAdded.Add(s.PersonId);
            }

            #endregion


            dnagedContext.SaveChanges();

            Console.WriteLine("saved " + addedRecords + " new records");
            return cutOff;
        }

        private void ImportMatchDetailsWithGroups(DNAGEDContext dnagedContext, Guid importTestId)
        {
            

            // find all the matches in the existing db for this testid
            var filterGroupList = dnagedContext.MatchGroups.Where(w=>w.TestGuid == importTestId).Select(m => m.MatchGuid).ToList();

            //find all the matches in the sqlite db for this testid
            var missingGroupRecordCount = this.MatchGroups.Where(w => w.TestGuid == importTestId).Count(w => !filterGroupList.Contains(w.MatchGuid));

            var newRecords = this.MatchGroups.Where(w => !filterGroupList.Contains(w.MatchGuid) && w.TestGuid == importTestId).ToList();

            Console.WriteLine("missing match groups: " + missingGroupRecordCount);

            List<Guid> insertedRecords = new List<Guid>();

            int newMatchGroupCounter = 0;
            foreach (var s in newRecords)
            {
                if (insertedRecords.Contains(s.MatchGuid)) continue;

                insertedRecords.Add(s.MatchGuid);

                dnagedContext.MatchGroups.Add(new Models.MatchGroups()
                {
                    Id = s.Id,
                    SharedSegment = s.SharedSegment,
                    TestGuid = s.TestGuid,
                    MatchGuid = s.MatchGuid,
                    Note = s.Note,
                    GroupName = s.GroupName,
                    TreesPrivate = s.TreesPrivate,
                    UserPhoto = s.UserPhoto,
                    TestAdminDisplayName = s.TestAdminDisplayName,
                    TestDisplayName = s.TestDisplayName,
                    TreeId = s.TreeId,
                    TreeNodeCount = s.TreeNodeCount,
                    Confidence = s.Confidence,
                    Starred = s.Starred,
                    SharedCentimorgans = s.SharedCentimorgans,
                    Viewed = s.Viewed,
                    HasHint = s.HasHint
                });

                newMatchGroupCounter++;
            }

            Console.WriteLine("added: " + newMatchGroupCounter + " to the context");




            var existingMatchRecords = dnagedContext.MatchDetail.Where(w => w.TestGuid == importTestId).Select(m => m.MatchGuid).ToList();

            Console.WriteLine("number of match details in the system: " + existingMatchRecords.Count);
            var sqlLiteMatchDetailsToImport = this.MatchDetails.Where(w => w.TestGuid == importTestId).Select(s => s.MatchGuid).ToList();

            var missingRecordCount = sqlLiteMatchDetailsToImport.Count(w => !existingMatchRecords.Contains(w));

            Console.WriteLine("missing match details: " + missingRecordCount);

            Console.WriteLine("adding match details: ");
            dnagedContext.MatchDetail.AddRange(this.MatchDetails.Where(w => !existingMatchRecords.Contains(w.MatchGuid) && w.TestGuid == importTestId).Select(s =>
                new Models.MatchDetail()
                {
                    Id = s.Id,
                    SharedSegment = s.SharedSegment,
                    TestGuid = s.TestGuid,
                    MatchGuid = s.MatchGuid,
                    MatchGu = dnagedContext.MatchGroups.FirstOrDefault(f => f.MatchGuid == s.MatchGuid)
                    
                }));

            Console.WriteLine("added: " + missingRecordCount + " MatchDetails to the context");

            dnagedContext.SaveChanges();
            Console.WriteLine("saving context");

            Console.WriteLine("press key to continue");
        }


        public void UpdateTreeId()
        {

            DNAGEDContext dnagedContext = new DNAGEDContext();


           // double total = dnagedContext.MatchGroups.Count(mg => mg.TreeId.Contains("ancestry.com"));
            double counter = 0;
            double percentage = 0.0;

            var data = dnagedContext.MatchGroups.Where(mg => mg.TreeId.Contains("ancestry.com"));
            //dnagedContext.UpdateRange();
            double total = data.Count();
            
            int saveCounter = 0;

            foreach (var p in data)
            {
                p.TreeId = p.TreeId.Replace(@"ancestry.com", "ancestry.co.uk");

                percentage = ((counter / total) * 100);
                
                Console.Write("\r" + percentage + " %   ");
                
                counter++;

                //if (saveCounter == 1000)
                //{

                //    dnagedContext.SaveChanges();
                //    saveCounter = 0;
                //}

                saveCounter++;

                dnagedContext.MatchGroups.Update(p);
            }

            dnagedContext.SaveChanges();



        }
        

       

        #region update country


        private static void UnknownExperiment(DNAGEDContext dnagedContext)
        {
            var places = PlaceOperations.GetPlaces();

            var countyLookup = dnagedContext.Persons.Where(w => w.BirthCounty.Length > 1 ).Select(s => new PlaceDto()
            {
                Place = s.BirthPlace,
                County = s.BirthCounty,
                Country = s.BirthCountry
            }).ToList();


            var filteredPersonSet = dnagedContext.Persons.Where(w => w.BirthCountry == "anglosphere" && w.BirthPlace!=""
                                                                     && (w.BirthCounty == null || w.BirthCounty == ""));

            double total = filteredPersonSet.Count();
            double counter = 0;
            double percentage = 0.0;

            Console.WriteLine("UnknownExperiment: " + total);

            foreach (var p in filteredPersonSet)
            {
                int isFound = 0;

                p.BirthCounty = "";

                var placeLine = new PlaceLine(p.BirthPlace);

                if (CustomExceptions(p)) continue;

                PlaceCollection matchRecord = new PlaceCollection();

                string record = "";
                p.Memory = "";

                foreach (var place in places)
                {
                    if (place.Place == "ham" ||
                        place.Place == "ton" ||
                        place.Place == "field" ||
                        place.Place == "ford" ||
                        place.Place == "west" ||
                        place.Place == "street")
                        continue;

                    placeLine.LoadIntoCollection(place, matchRecord);

                }

                if (matchRecord.Count > 0)
                {
                    p.Memory = matchRecord.ToString();

                    p.BirthCountry = "anglosphere";

                    if (matchRecord.Count == 1)
                    {
                        p.BirthCounty = matchRecord.First().County;
                        p.BirthCountry = matchRecord.First().Country;
                    }
                    else
                    {
                        //so our persons birthplace has matched against multiple locations
                        
                        foreach (var match in matchRecord)
                        {
                            PlaceCollection matchRecord2 = new PlaceCollection();

                            foreach (var possibleCounty in countyLookup)
                            {
                                //person birth place broken up into components
                                var lookupPlaceLine = new PlaceLine(possibleCounty.Place);
                                //does the match , match any birthplaces with known counties
                                lookupPlaceLine.Check(possibleCounty,match, matchRecord2);
                            }

                            if (matchRecord2.Count > 0)
                            {                                
                                var county = matchRecord2.GroupBy(g => g.County)
                                    .OrderByDescending(o => o.Count()).FirstOrDefault();

                                if (county != null)
                                {
                                    p.BirthCounty = county?.Key;
                                    p.BirthCountry = county.FirstOrDefault()?.Country;

                                    break;
                                }
                               
                            }
                        }

                    }
                    
                     

                    
                }
 
                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }

            Console.WriteLine("saving");
            dnagedContext.SaveChanges();
        }

        private static bool CustomExceptions(Persons p)
        {
            if (p.BirthPlace.ToLower().Contains("victoria"))
            {
                p.BirthCounty = "New South Wales";
                p.BirthCountry = "Australia";
                return true;
            }

            if (p.BirthPlace == "en" || p.BirthPlace == "eng" || p.BirthPlace == "uk")
            {
                p.BirthCounty = "";
                p.BirthCountry = "England";
                return true;
            }

            if (p.BirthPlace == "Godmanchester")
            {
                p.BirthCounty = "Huntingdonshire";
                p.BirthCountry = "England";
                return true;
            }

            if (p.BirthPlace == "Goole")
            {
                p.BirthCounty = "Yorkshire";
                p.BirthCountry = "England";
                return true;
            }
            
            if (p.BirthPlace == "Hartford, Hunts")
            {
                p.BirthCounty = "Huntingdonshire";
                p.BirthCountry = "England";
                return true;
            }
            
            if (p.BirthPlace == "Linconshire")
            {
                p.BirthCounty = "Lincolnshire";
                p.BirthCountry = "England";
                return true;
            }




            return false;
        }



        #endregion


    }






}
