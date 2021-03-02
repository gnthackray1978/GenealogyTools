using ConsoleTools;
using FTMContext;
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
        private FTMakerContext _sourceContext;
        private FTMakerCacheContext _cacheContext;
        private IConsoleWrapper _consoleWrapper;

        public List<MatchGroup> MatchGroups { get; set; } = new List<MatchGroup>();

        //public static DupeEntry DumpString(FTMakerContext f, List<Fact> facts, 
        //    int personId, string ident) {

        //    var dupeEntry = new DupeEntry();

        //    var cpFact = FTMTools.GetFact(facts, personId);

        //    var person = f.Person.FirstOrDefault(f1 => f1.Id == personId);

        //    var birthString = "";

        //    if (cpFact.BirthYearFrom == cpFact.BirthYearTo)
        //        birthString = cpFact.BirthYearFrom.ToString();
        //    else
        //        birthString = cpFact.BirthYearFrom.ToString() + " - " + cpFact.BirthYearTo.ToString();

        //    string result = cpFact.Origin + " , " + birthString + " , " + GoogleGeoCodingHelpers.FormatPlace(person.BirthPlace) + " , " + person.GivenName + " , " + person.FamilyName;

        //    dupeEntry.Ident = ident;
        //    dupeEntry.PersonId = personId;
        //    dupeEntry.BirthYearFrom = cpFact.BirthYearFrom;
        //    dupeEntry.BirthYearTo = cpFact.BirthYearTo;
        //    dupeEntry.Origin = cpFact.Origin;
        //    dupeEntry.Location = GoogleGeoCodingHelpers.FormatPlace(person.BirthPlace);
        //    dupeEntry.ChristianName = person.GivenName;
        //    dupeEntry.Surname = person.FamilyName;

        //    return dupeEntry;
        //}

        public static string MakeKey(string name1) {
            DoubleMetaphone mphone = new DoubleMetaphone();

            mphone.computeKeys(name1);

            var pkey1 = mphone.PrimaryKey;
            var akey1 = mphone.AlternateKey;

            return pkey1 + akey1;
        } 

        public static bool CompareNames(string name1, string name2) {


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

        public PersonGrouper(FTMakerContext sourceContext,
                             FTMakerCacheContext cacheContext,
                             IConsoleWrapper consoleWrapper) {
         
            _sourceContext = sourceContext;
            _cacheContext = cacheContext;
            _consoleWrapper = consoleWrapper;
        }


        private MatchGroup MatchGroupExists(List<MatchGroup> matchGroups, int personId) {
          

            foreach (var matchGroup in matchGroups) {
                if (matchGroup.Contains(personId)) {
                    return matchGroup;
                }
            }

            return null;
        }


        /// <summary>
        /// Fills out dupes table in destination context.
        /// Searchs source context for dupes. Needs source context because
        /// this will have all facts(origin person,location,birth range etc) filled out. 
        /// </summary>      
        public void PopulateDupeEntries() {



            //  var facts =  _sourceContext.Fact.Where(w => w.FactTypeId == 14 || w.FactTypeId == 90).ToList();
            //GetFactFromViewEntry

              var comparisonPersons = _cacheContext.FTMPersonView.Where(w=> !string.IsNullOrEmpty(w.FirstName)
                                                                   && !string.IsNullOrEmpty(w.Surname))
                        .Select(s => new PersonDupeSearchSubset() {
                        Id = s.PersonId,
                        FamilyName = MakeKey(s.FirstName),
                        GivenName = MakeKey(s.Surname),
                        Fact = FTMTools.GetFactFromViewEntry(s.BirthFrom,s.BirthTo,s.Origin,s.LinkedLocations)
                        }).ToList();


            _consoleWrapper.WriteLine("Records to search: " + comparisonPersons.Count());
            int idx = 0;

            foreach (var cp in comparisonPersons) {

                if (idx % 1000 == 0)
                    _consoleWrapper.WriteCounter(idx + " of " + comparisonPersons.Count());

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

            _consoleWrapper.WriteLine("Found: " + MatchGroups.Count());

            foreach (var mg in MatchGroups)
            {
                string identString = "";
               // var birthString = "";
                int latestTree = 0;
                List<string> origins = new List<string>();

                foreach (var p in mg.Persons)
                {
                    var person = comparisonPersons.First(x => x.Id == p);

                    var cpFact = person.Fact;

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

            var dupeId = _cacheContext.DupeEntries.Count() +1;

            foreach (var group in MatchGroups.GroupBy(g => g.IncludedTrees)) {
                _consoleWrapper.WriteCounter(group.Key);

                var p = group.OrderByDescending(o => o.LatestTree).First();
                
                foreach (var person in p.Persons) {

                    var dupeEntry = _cacheContext.CreateNewDupeEntry(dupeId,
                        _cacheContext.FTMPersonView.First(f => f.PersonId == person), person, group.Key);

                    _cacheContext.DupeEntries.Add(dupeEntry);
                    dupeId++;
                }
                //    Debug.WriteLine(DumpString(f, facts, person));
            }

            _cacheContext.SaveChanges();
           


          
        }
    }
}
