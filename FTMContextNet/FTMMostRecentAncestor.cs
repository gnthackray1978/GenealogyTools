using ConsoleTools;
using FTMContext.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FTMContext
{
    public class FTMMostRecentAncestor
    {
        private FTMakerContext _context;
        private FTMakerCacheContext _cacheContext;
        private IConsoleWrapper _consoleWrapper;

        public List<int> AddedPersons = new List<int>();
        public List<int> SpouseList = new List<int>();

        public List<int> RelList = new List<int>();

        public List<int> ChildList = new List<int>();

        public FTMMostRecentAncestor(FTMakerContext context,
            FTMakerCacheContext cacheContext,
            IConsoleWrapper consoleWrapper) {
            _consoleWrapper = consoleWrapper;
            _context = context;
            _cacheContext = cacheContext;
        }

        

        public void MarkMostRecentAncestor()
        {           
            var firstPerson = _context.Person.First(w => w.Id == 1);

            _consoleWrapper.WriteLine(firstPerson.BirthDate);

            var me = _context.Person.FirstOrDefault(w => w.FamilyName.Trim() == "Thackray" && w.GivenName.Trim() == "George Nicholas");

            int personId = 0;

            if (me != null)
            {
                personId = me.Id;
            }


            var rootPeople = _context.Person.Where(w => w.FamilyName.StartsWith("_") || w.Id == personId);
            
            int idx = _cacheContext.FTMPersonOrigins.Count()+1;

            foreach (var rootPerson in rootPeople)
            {
                //var rootPerson = rootPeople.First();
              //  _consoleWrapper.WriteLine("Assigning ancestors for : " + rootPerson.FamilyName);

                AddedPersons = new List<int>();
                SpouseList = new List<int>();
                RelList = new List<int>();
                ChildList = new List<int>();


                //if (rootPerson.FamilyName != "_28_lyn!lawrence")
                //    continue;
              
                LookupAncestors(rootPerson.Id);

                _consoleWrapper.WriteCounter("Saving Facts for "+ rootPerson.FamilyName + " " + AddedPersons.Count() + " ancestors");

                //FTMPERSONORIGIN shoule be empty!

                Debug.WriteLine(rootPerson.FamilyName + " , " + AddedPersons.Count + " , " + SpouseList.Count);

                if (SpouseList.Count > 0)
                {
                    var spouseListIdx = 0;

                    while (spouseListIdx < SpouseList.Count)
                    {
                        LookupAncestors(SpouseList[spouseListIdx]);

                        spouseListIdx++;
                    }
                }

                foreach (var id in AddedPersons)
                {
                    var person = _context.Person.First(w => w.Id == id);
                    //  FTMTools.SaveFact(_context, 14, rootPerson.FamilyName, person.Id);

                    //if (rootPerson.FamilyName == "_28_lyn!lawrence" )
                    //{
                    //    Debug.WriteLine(person.Id + "," + person.FullName);
                    //}

                    _cacheContext.FTMPersonOrigins.Add(new FTMPersonOrigin()
                    {
                        Id = idx,
                        PersonId = person.Id,
                        Origin =  rootPerson.FamilyName
                    });

                    idx++;
                }


                _cacheContext.SaveChanges();

            }
           
            _context.SaveChanges();
            _consoleWrapper.WriteLine("Finished ");
        }

        private void LookupDescendants(int testPersonId)
        {
           

            var parentalRelationship = _context.Relationship.Where(w => w.Person1Id == testPersonId
                                                                        || w.Person2Id == testPersonId).ToList();

           
            //is this guy part of any parental relationships
            foreach (var pr in parentalRelationship)
            {
                if (RelList.Contains(pr.Id) || pr.Id == 12619)
                    continue;

                RelList.Add(pr.Id);

                var spouseId = pr.Person1Id;
                if (pr.Person1Id == testPersonId)
                {
                    spouseId = pr.Person2Id;
                }

                if (spouseId != null)
                {
                    //ADD SPOUSE
                    if (!AddedPersons.Contains(spouseId.Value))
                    {
                        var p = _context.Person.FirstOrDefault(w => w.Id == spouseId);

                        if (p != null && !p.FullName.ToLower().Contains("group")
                                      && !p.FullName.ToLower().Contains("_"))
                        {
                            if (!SpouseList.Contains(p.Id))
                            {
                                //if (p.FullName == "Hannah Wilson")
                                //{
                                //    Debug.WriteLine("xx");
                                //}
                                //Debug.WriteLine("added: " + p.FullName + " " + p.BirthDate);
                                SpouseList.Add(p.Id);
                            }
                           
                            AddedPersons.Add(p.Id);
                        }

                        //    LookupAncestors(p.Id);

                        
                       // _consoleWrapper.WriteLine(p.Id + " " + p.FullName);
                    }
                }



                var relationshipId = pr.Id;
                var otherChildren = _context.ChildRelationship.Where(w => w.RelationshipId == relationshipId).ToList();

                foreach (var child in otherChildren)
                {
                    if (ChildList.Contains(child.Id))
                        continue;

                    ChildList.Add(child.Id);

                    if (!AddedPersons.Contains(child.PersonId))
                    {
                        AddedPersons.Add(child.PersonId);
                        var cPerson = _context.Person.FirstOrDefault(w => w.Id == child.PersonId);

                        if (cPerson != null 
                            && !cPerson.FullName.ToLower().Contains("group")
                            && !cPerson.FullName.ToLower().Contains("_"))
                        {
                            LookupDescendants(child.PersonId);
                        } 
                        
                    }


                }

            }
        }

        private void LookupAncestors(int testPersonId)
        {

            //function called recursively.

            //if (testPersonId == 27828)
            //{
            //    Debug.WriteLine("xx");

            //}

            var p = _context.Person.FirstOrDefault(w => w.Id == testPersonId);

            //if (p.FullName.ToLower().Contains("blakeway"))
            //{
            //    Debug.WriteLine(p.FullName);
            //}

            if (!AddedPersons.Contains(p.Id))
            {
                AddedPersons.Add(p.Id);
                

            }
            else
            {
              //  Debug.WriteLine(p.Id + " " + p.FullName + " already added");

            }
          
            //child relationship joins a person to a relationship i.e. their parents relationship
            var crs = _context.ChildRelationship.Where(w => w.PersonId == testPersonId).ToList();

            LookupDescendants(testPersonId);

            foreach (var c in crs)
            {
                if (ChildList.Contains(c.Id))
                    continue;

                ChildList.Add(c.Id);


                var rels = _context.Relationship.Where(w => w.Id == c.RelationshipId).ToList();

                foreach (var r in rels)
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                        LookupAncestors(p1.Value);

                    var p2 = r.Person2Id;
                    if (p2 != null)
                        LookupAncestors(p2.Value);
                }
            }
        }


    }
}
