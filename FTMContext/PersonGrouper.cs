﻿using FTMContext;
using FTMContext.lib;
using FTMContext.Models;
using nullpointer.Metaphone;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FTMContext
{
   

    public class PersonGrouper {

        public static DupeEntry DumpString(FTMakerContext f, List<Fact> facts, int personId, string ident) {

            var dupeEntry = new DupeEntry();

            var cpFact = FTMTools.GetFact(facts, personId);

            var person = f.Person.FirstOrDefault(f1 => f1.Id == personId);

            var birthString = "";

            if (cpFact.BirthYearFrom == cpFact.BirthYearTo)
                birthString = cpFact.BirthYearFrom.ToString();
            else
                birthString = cpFact.BirthYearFrom.ToString() + " - " + cpFact.BirthYearTo.ToString();

            string result = cpFact.Origin + " , " + birthString + " , " + GoogleGeoCodingHelpers.FormatPlace(person.BirthPlace) + " , " + person.GivenName + " , " + person.FamilyName;

            dupeEntry.Ident = ident;
            dupeEntry.PersonId = personId;
            dupeEntry.BirthYearFrom = cpFact.BirthYearFrom;
            dupeEntry.BirthYearTo = cpFact.BirthYearTo;
            dupeEntry.Origin = cpFact.Origin;
            dupeEntry.Location = GoogleGeoCodingHelpers.FormatPlace(person.BirthPlace);
            dupeEntry.ChristianName = person.GivenName;
            dupeEntry.Surname = person.FamilyName;

            return dupeEntry;
        }

        public static string MakeKey(string name1) {
            DoubleMetaphone mphone = new DoubleMetaphone();

            mphone.computeKeys(name1);

            var pkey1 = mphone.PrimaryKey;
            var akey1 = mphone.AlternateKey;

            return pkey1 + akey1;
        } 

        public bool CompareNames(string name1, string name2) {


            DoubleMetaphone mphone = new DoubleMetaphone();

            mphone.computeKeys(name1);

            var pkey1 = mphone.PrimaryKey;
            var akey1 = mphone.AlternateKey;


            mphone.computeKeys(name2);

            var pkey2 = mphone.PrimaryKey;
            var akey2 = mphone.AlternateKey;


            if(pkey1 == pkey2 && akey1 == akey2)
            {
                return true;
            }

            return false;
        }

        public List<MatchGroup> MatchGroups { get; set; } = new List<MatchGroup>();


        public MatchGroup MatchGroupExists(List<MatchGroup> matchGroups, int personId) {
          

            foreach (var matchGroup in matchGroups) {
                if (matchGroup.Contains(personId)) {
                    return matchGroup;
                }
            }

            return null;
        }



        public void Group() {
            var a = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });

            var f = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Software MacKiev\Family Tree Maker\",
                FileName = @"DNA Match File.ftm",
                IsEncrypted = true
            });

            var facts =  f.Fact.Where(w => w.FactTypeId == 14 || w.FactTypeId == 90).ToList();

          
            var comparisonPersons = f.Person.Where(w=> !string.IsNullOrEmpty(w.GivenName)
                    && !string.IsNullOrEmpty(w.FamilyName))
                        .Select(s => new PersonDupeSearchSubset() {
                        Id = s.Id,
                        FamilyName = MakeKey(s.FamilyName),
                        GivenName = MakeKey(s.GivenName),
                        Fact = FTMTools.GetFact(facts, s.Id)
                }).ToList();

        
            Console.WriteLine("Records to search: " + comparisonPersons.Count());
            int idx = 0;

            foreach (var cp in comparisonPersons) {

                if (idx % 1000 == 0)
                    Console.WriteLine(idx);

               // var cpFact = FTMTools.GetFact(f, cp.Id);

              //  if (cpFact == null) continue;

                var newGroup = false;
                // if this person is in a existing group
                // get that group
                var mg = MatchGroupExists(this.MatchGroups, cp.Id);

                if (mg == null)
                {
                    mg = new MatchGroup();
                    mg.ID = this.MatchGroups.Count() + 1;
                    mg.Persons.Add(cp.Id);
                    if(cp.Fact!=null)
                    mg.Origins.Add(cp.Fact.Origin);
                    newGroup = true;
                }
             
                foreach (var p in comparisonPersons
                    .Where(w=>w.FamilyName == cp.FamilyName && w.GivenName == cp.GivenName && cp.Fact!=null))
                {
                    if (p.Fact == null) continue;

                    if (mg.Persons.Contains(p.Id))
                        continue;
                                        
                    var yearMatch = cp.Fact.MatchBirthYear(p.Fact);

                    var locationMatch = cp.Fact.MatchLocations(p.Fact);

                    var originMatch = cp.Fact.Origin == p.Fact.Origin;

                    
                    if (yearMatch && locationMatch && !originMatch) {

                        if (p.Fact.Origin == "_24_mountain" || p.Fact.Origin == "_20.9_SCross")
                        {
                            Debug.WriteLine("");
                        }

                        if (!mg.Origins.Contains(p.Fact.Origin))
                        {
                            mg.Persons.Add(p.Id);
                            mg.Origins.Add(p.Fact.Origin);
                        }
                    }


                    //comparisonPerson.GivenName
                    //comparisonPerson.FamilyName


                    //match name
                    //match birth range
                    //match counties
                    //ensure the dupe doesn't have the same source

                }

                //if it's a new group and we found dupes
                if (mg.Persons.Count() > 1 && newGroup) {
                    MatchGroups.Add(mg);
                }

                idx++;
            }

            Console.WriteLine("Found: " + MatchGroups.Count());

            foreach (var mg in MatchGroups)
            {
                string identString = "";
               // var birthString = "";
                int latestTree = 0;
                List<string> origins = new List<string>();

                foreach (var p in mg.Persons)
                {
                    var cpFact = FTMTools.GetFact(facts, p);

                    if (cpFact.BirthYearFrom > latestTree)
                        latestTree = cpFact.BirthYearFrom;

                    //if (cpFact.BirthYearFrom == cpFact.BirthYearTo)
                    //    birthString = cpFact.BirthYearFrom.ToString();
                    //else
                    //    birthString = cpFact.BirthYearFrom.ToString() + " - " + cpFact.BirthYearTo.ToString();

                    //  Debug.WriteLine(DumpString(f,facts, p));
                    origins.Add(cpFact.Origin);
                    
                }

                foreach (var o in origins.OrderBy(o => o)) {
                    identString += o;
                }

                //identString += birthString;
                mg.LatestTree = latestTree;
                mg.IncludedTrees = identString;
            }

            var dupeId = a.DupeEntries.Count() +1;

            foreach (var group in MatchGroups.GroupBy(g => g.IncludedTrees)) {
                Debug.WriteLine(group.Key);

                var p = group.OrderByDescending(o => o.LatestTree).First();



                foreach (var person in p.Persons) {
                    var dupeEntry = DumpString(f, facts, person, group.Key);
                    dupeEntry.Id = dupeId;

                    a.DupeEntries.Add(dupeEntry);
                    dupeId++;
                }
                //    Debug.WriteLine(DumpString(f, facts, person));
            }

            a.SaveChanges();
            Console.ReadKey();


          
        }
    }
}
