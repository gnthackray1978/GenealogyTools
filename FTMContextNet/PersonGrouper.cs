using FTMContext;
using FTMContext.lib;
using FTMContext.Models;
using nullpointer.Metaphone;
using System;
using System.Diagnostics;
using System.Linq;
using LoggingLib;

namespace FTMContext
{
    public class PersonGrouper {      
        private FTMakerContext _sourceContext;
        private FTMakerCacheContext _cacheContext;
        private Ilog _ilog;

        public MatchGroups MatchGroups { get; set; } = new MatchGroups();

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
                             Ilog ilog) {
         
            _sourceContext = sourceContext;
            _cacheContext = cacheContext;
            _ilog = ilog;
        }





        /// <summary>
        /// Fills out dupes table in destination context.
        /// Searchs source context for dupes. Needs source context because
        /// this will have all facts(origin person,location,birth range etc) filled out. 
        /// </summary>      
        public void PopulateDupeEntries() {



            //  var facts =  _sourceContext.Fact.Where(w => w.FactTypeId == 14 || w.FactTypeId == 90).ToList();
            //GetFactFromViewEntry

              var comparisonPersons = _cacheContext.FTMPersonView.Where(w=> 
                                                                      !string.IsNullOrEmpty(w.FirstName)
                                                                      && !w.FirstName.ToLower().Contains("group")
                                                                      && !string.IsNullOrEmpty(w.Surname)
                                                                   && !string.IsNullOrEmpty(w.Origin)
                                                                   && w.Origin != "Thackray")
                        .Select(s => new PersonDupeSearchSubset() {
                        Id = s.PersonId,
                        FamilyName = MakeKey(s.FirstName),
                        GivenName = MakeKey(s.Surname),
                        Fact = PersonDataObj.Create(s.BirthFrom,s.BirthTo,s.Origin,s.LinkedLocations,s.Surname)
                        }).ToList();


            _ilog.WriteLine("Records to search: " + comparisonPersons.Count());
            int idx = 0;

            foreach (var cp in comparisonPersons) {
                //12707
                //13124

                if (cp.Id == 8260 || cp.Id == 27621)
                {
                    Debug.WriteLine("stop");
                }

                if (idx % 1000 == 0)
                    _ilog.WriteCounter(idx + " of " + comparisonPersons.Count());

                // if this person is in a existing group
                // get that group
                var mg = MatchGroups.FindByPersonId(cp.Id) ?? MatchGroups.CreateGroup(cp.Id, cp.Fact.Origin, cp.Fact.BirthYearFrom);

                foreach (var p in comparisonPersons
                    .Where(w=>w.FamilyName == cp.FamilyName 
                              && w.GivenName == cp.GivenName 
                              && cp.Fact!=null
                              && w.Fact!=null 
                              && !mg.Contains(w.Id)))
                {
                    
                    var yearMatch = cp.Fact.MatchBirthYear(p.Fact);

                    var locationMatch = cp.Fact.MatchLocations(p.Fact);

                    var originMatch = cp.Fact.Origin == p.Fact.Origin;

                    var surnameCheck = cp.Fact.DoubleCheckSurname(p.Fact);

                    if (surnameCheck && yearMatch && locationMatch && !originMatch) {

                        mg.AddPerson(p.Id, p.Fact.Origin, p.Fact.BirthYearFrom);
                        
                    }
                    
                }

                MatchGroups.SaveGroup(mg);

                 
                idx++;
            }

            _ilog.WriteLine("Found: " + MatchGroups.Groups.Count());

            MatchGroups.SetAggregates();


             var dupeId = _cacheContext.DupeEntries.Count() +1;

            foreach (var group in MatchGroups.Groups.GroupBy(g => g.IncludedTrees)) {
                _ilog.WriteCounter(group.Key);

                var p = group.OrderByDescending(o => o.LatestTree).First();
                
                foreach (var person in p.Persons)
                {

                    var fpvPerson = _cacheContext.FTMPersonView.First(f => f.PersonId == person.PersonId);

                    var dupeEntry = _cacheContext.CreateNewDupeEntry(dupeId, fpvPerson, group.Key);

                    _cacheContext.DupeEntries.Add(dupeEntry);
                    dupeId++;
                }
                //Debug.WriteLine(DumpString(f, facts, person));
            }

            _cacheContext.SaveChanges();
           


          
        }
    }
}
