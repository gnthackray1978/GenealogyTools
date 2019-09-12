using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using DNAGedLib.Models;
using Microsoft.EntityFrameworkCore;

namespace DNAGedLib
{
    public class PersonContainer 
    {
        public PersonsOfInterest PersonsOfInterest { get; set; }
        public bool IsDupe { get; set; }
    }

    public class Tools {
        
        /// <summary>
        /// 
        /// </summary>
        public static void CreateGroup()
        {            
            int idx = 0;

           // List<UtilityPersonGroup> pgroups = new List<UtilityPersonGroup>();

            DNAGEDContext dnagedContext = new DNAGEDContext();

            int recCount = dnagedContext.PersonsOfInterest.Count();
            List<PersonContainer> records = dnagedContext.PersonsOfInterest.Select(s=> new PersonContainer{PersonsOfInterest = s}).ToList();
            
            Console.WriteLine(recCount);

            List<List<PersonGroupContainer>> listPersonGroups = new List<List<PersonGroupContainer>>();

            int groupId = 0;

            while (idx < recCount)
            {
                if (records[idx].PersonsOfInterest.BirthYear < 1700)
                {
                    idx++;
                    continue;

                }

                //if (records[idx].BirthYear == 1842 && records[idx].ChristianName == "John George" && records[idx].Surname== "Wright")
                //{
                //    Console.WriteLine("found");
                //}

                groupId++;

                if (!records[idx].IsDupe) //.PersonsOfInterest.Memory != "ADDED")
                {
                    List<PersonGroupContainer> personGroups = new List<PersonGroupContainer>();

                    var pg = new PersonGroups
                    {
                        CreatedDate = DateTime.Now,
                        Description = records[idx].PersonsOfInterest.BirthYear + records[idx].PersonsOfInterest.ChristianName + records[idx].PersonsOfInterest.Surname,
                        GroupingKey = "",
                        PersonGroupId = groupId,
                        PersonId = records[idx].PersonsOfInterest.PersonId,
                        PersonGroupCount = 0,
                        PersonGroupIndex = 0           
                    };

                    personGroups.Add(new PersonGroupContainer{PersonGroup = pg,
                        TestAdminDisplayName = records[idx].PersonsOfInterest.TestAdminDisplayName } );

                    SearchForDupes(personGroups, records, idx, groupId);

                    if (personGroups.Count > 1)
                        listPersonGroups.Add(personGroups);
                }

              

                //search for dupes 

                idx++;
            }


            Console.WriteLine(listPersonGroups.Count);


            List<PersonGroups> pgroups = new List<PersonGroups>();

            foreach (var lst in listPersonGroups)
            {
                foreach (var pg in lst)
                {
                    pg.PersonGroup.PersonGroupCount = lst.Count;
                    pgroups.Add(pg.PersonGroup);
                }
            }

            AddPersonGroups(pgroups);

        }


        private static void SearchForDupes(List<PersonGroupContainer> personGroups,
            List<PersonContainer> records,
            int location,
            int groupId)
        {
            if (location > records.Count) return;

            var comparisonPerson = records[location];

            int limit = location + 10;
            int idx = location + 1;
            while (idx < limit && idx < records.Count)
            {

                if (records[idx].PersonsOfInterest.BirthYear == comparisonPerson.PersonsOfInterest.BirthYear &&
                    records[idx].PersonsOfInterest.ChristianName == comparisonPerson.PersonsOfInterest.ChristianName &&
                    records[idx].PersonsOfInterest.Surname == comparisonPerson.PersonsOfInterest.Surname)
                {
                    if (!PersonGroupContainsPersonId(personGroups, records[idx].PersonsOfInterest.PersonId))
                    {
                        var pg = new PersonGroups
                        {
                            CreatedDate = DateTime.Now,
                            Description = records[idx].PersonsOfInterest.BirthYear + records[idx].PersonsOfInterest.ChristianName + records[idx].PersonsOfInterest.Surname,
                            GroupingKey = "",
                            PersonGroupId = groupId,
                            PersonId = records[idx].PersonsOfInterest.PersonId,
                            PersonGroupCount = 0,
                            PersonGroupIndex = personGroups.Count,
                        };

                        personGroups.Add(new PersonGroupContainer
                        {
                            PersonGroup = pg,
                            TestAdminDisplayName = records[idx].PersonsOfInterest.TestAdminDisplayName
                        });

                    }


                    records[idx].IsDupe = true;
                }
                idx++;
            }
        }

        private static void AddPersonGroups(List<PersonGroups> personGroups)
        {
            DNAGEDContext dnagedContext = new DNAGEDContext();

            var tp = dnagedContext.PersonGroups;

            int seedId = 0;

            if (tp.Any())
                seedId = dnagedContext.PersonGroups.Max(m => m.Id) + 1;

            foreach (var pg in personGroups)
            {
                pg.Id = seedId;

                dnagedContext.PersonGroups.Add(pg);

                seedId++;
            }

            dnagedContext.SaveChanges();

            Console.WriteLine();
            Console.WriteLine("count of recs : " + dnagedContext.PersonGroups.Count());
        }


        private static bool PersonGroupContainsTestAdmin(List<PersonGroupContainer> personGroups, string testAdmin)
        {
            if (personGroups.Count == 0) return false;

            foreach (var pg in personGroups)
            {
                if (pg.TestAdminDisplayName == testAdmin)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool PersonGroupContainsPersonId(List<PersonGroupContainer> personGroups, long personId)
        {
            if (personGroups.Count == 0) return false;

            foreach (var pg in personGroups)
            {
                if (pg.PersonGroup.PersonId == personId)
                {
                    return true;
                }
            }

            return false;
        }


        public static void FindGroups() {
            var ignoreList = new List<Guid>();



            DNAGEDContext dnagedContext = new DNAGEDContext();


            ignoreList = dnagedContext.MatchGroups.Where(w => w.TestGuid == Guid.Parse("8FB3636A-03A2-4289-A7CB-C1F8C5AB25C4")).Select(s => s.MatchGuid).ToList();


            ignoreList.Remove(Guid.Parse("4F827E38-9E05-4BD3-A058-56CE94EC06E4"));//me via dad

            ignoreList.Add(Guid.Parse("44796820-3BA6-4E9F-BF38-71EB1BB3EC7B"));//francis smith

            var personsToIgnore = dnagedContext.MatchTrees.Where(w => ignoreList.Contains(w.MatchId)).Select(s => s.PersonId).ToList();

            var temp = dnagedContext.Persons.Where(w =>

                w.BirthCountry == "Unknown"
                && w.BirthPlace.ToLower().Contains("norfolk")
                && w.DeathCountry == "Unknown"
                && w.BirthYear >= 1700
                && w.Surname != ""
                && w.Surname.Length > 3
                && w.BirthPlace != ""
                && !personsToIgnore.Contains(w.Id)
            ).ToList();

            foreach (var t in temp)
            {
                if (t.Id == 2018077130)
                {
                    Debug.WriteLine("mum");
                }

                if (t.Id == 2018076843)
                {
                    Debug.WriteLine("my tree");
                }

                if (t.Id == 77003242552)
                {
                    Debug.WriteLine("francis smith");
                }

                if (t.Id == 370050135191)
                {
                    Debug.WriteLine("david cornish");
                }

                t.Memory = t.Surname.Substring(0, 3);
            }




            var test = temp.GroupBy(x => new { x.Surname }, (key, group) => new
            {
                //  BirthYear = key.BirthYear,
                Surname = key.Surname,
                //Surname = key.Surname,
                Result = group.ToList()
            });


            var groups = new List<Persons>();


            //disabled because i changed primary keyof matchgroups and that broke efcore update...


            //foreach (var t in test.Where(p => p.Result.Count() > 1).OrderBy(p => p.Result.Count()))
            //{
            //    var matchDescription = "";

            //    var groupCount = t.Result.Count();


            //    List<string> matchRecord = new List<string>();

            //    foreach (var person in t.Result)
            //    {
            //        var m = dnagedContext.MatchTrees.Include(i => i.Match).FirstOrDefault(w => w.PersonId == person.Id);

            //        if (!matchRecord.Contains(m.Match.TestAdminDisplayName))
            //        {
            //            matchDescription += " " + m.Match.TestAdminDisplayName;
            //            matchRecord.Add(m.Match.TestAdminDisplayName);
            //            groupCount--;
            //        }
            //    }



            //    if (groupCount == 0)
            //    {

            //        var newAddition = t.Result.First();

            //        newAddition.Memory = matchDescription;

            //        groups.Add(newAddition);
            //    }
            //}

            //foreach (var gp in groups.GroupBy(g => g.Memory))
            //{


            //    Console.WriteLine(gp.First().BirthYear + " " + gp.First().ChristianName + " " + gp.First().Surname + " " + gp.First().BirthPlace + " " + gp.Key);
            //}

        }

        public static void GroupByCounty()
        {
            // var gedcomdb = new GedcomDB();


            //  DNAGEDContext dnagedContext = new DNAGEDContext();

            //  List<Guid> matchIds = new List<Guid>();
            //  List<EFCoreReaderConsole.Models.MatchGroups> matchs = new List<EFCoreReaderConsole.Models.MatchGroups>();

            //  var norfolkPeople = dnagedContext.Persons.Include(p=>p.MatchTrees).Where(u => u.BirthCountry == "Unknown" 
            //  && u.BirthPlace.ToLower().Contains("norfolk")).ToList();


            //  var mgroups = dnagedContext.MatchGroups.ToList();



            //  var ignoreList = dnagedContext.MatchGroups.Where(w => w.TestGuid == Guid.Parse("8FB3636A-03A2-4289-A7CB-C1F8C5AB25C4")).Select(s => s.MatchGuid).ToList();


            ////  ignoreList.Remove(Guid.Parse("4F827E38-9E05-4BD3-A058-56CE94EC06E4"));//me via dad

            //  ignoreList.Add(Guid.Parse("44796820-3BA6-4E9F-BF38-71EB1BB3EC7B"));//francis smith


            //  foreach (var rec in norfolkPeople) {
            //      foreach (var m in rec.MatchTrees) {
            //          if (!matchIds.Contains(m.MatchId) && !ignoreList.Contains(m.MatchId))
            //          {
            //              matchs.Add(mgroups.FirstOrDefault(f => f.MatchGuid == m.MatchId));


            //              matchIds.Add(m.MatchId);
            //          }
            //      }
            //  }

            //  Console.WriteLine("match count: " + matchs.Count);

            //  //
            //  foreach (var m in matchs.OrderByDescending(a => a.SharedCentimorgans)) {
            //      Console.WriteLine(m.TestAdminDisplayName);
            //      Debug.WriteLine(m.TestAdminDisplayName + " , "+ m.TreeId);
            //  }
        }
    }
}