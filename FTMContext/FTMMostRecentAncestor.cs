using FTMContext;
using FTMContext;
using FTMContext.Models;
using MyFamily.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTMContext
{
    public class FTMMostRecentAncestor
    {
        public static List<int> AddedPersons = new List<int>();

        public static void MarkMostRecentAncestor()
        {
            var f = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Software MacKiev\Family Tree Maker\",
                FileName = @"DNA Match File.ftm",
                IsEncrypted = true
            });

            var firstPerson = f.Person.First(w => w.Id == 1);

            Debug.WriteLine(firstPerson.BirthDate);


            var rootPeople = f.Person.Where(w => w.FamilyName.StartsWith("_"));

            foreach (var rootPerson in rootPeople)
            {
                //var rootPerson = rootPeople.First();
                Console.WriteLine("Assigning ancestors for : " + rootPerson.FamilyName);

                AddedPersons = new List<int>();

                LookupAncestors(f, rootPerson.Id);

                foreach (var id in AddedPersons)
                {
                    var person = f.Person.First(w => w.Id == id);
                    FTMTools.SaveFact(f, 14, rootPerson.FamilyName, person.Id);
                }

            }

            f.SaveChanges();
            Console.WriteLine("finished: " + AddedPersons.Count);
        }

        private static void LookupDescendants(FTMakerContext f, int testPersonId)
        {

            var parentalRelationship = f.Relationship.Where(w => w.Person1Id == testPersonId
                                           || w.Person2Id == testPersonId).ToList();

            //is this guy part of any parental relationships
            foreach (var pr in parentalRelationship)
            {

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
                        var p = f.Person.FirstOrDefault(w => w.Id == spouseId);
                        AddedPersons.Add(p.Id);
                        Debug.WriteLine(p.Id + " " + p.FullName);
                    }
                }



                var relationshipId = pr.Id;
                var otherChildren = f.ChildRelationship.Where(w => w.RelationshipId == relationshipId).ToList();

                foreach (var child in otherChildren)
                {
                    if (!AddedPersons.Contains(child.PersonId))
                    {
                        AddedPersons.Add(child.PersonId);
                        var cPerson = f.Person.FirstOrDefault(w => w.Id == child.PersonId);

                        Debug.WriteLine(child.PersonId + " " + cPerson.FullName);
                        LookupDescendants(f, child.PersonId);
                    }


                }

            }
        }

        private static void LookupAncestors(FTMakerContext f, int testPersonId)
        {
            var p = f.Person.FirstOrDefault(w => w.Id == testPersonId);

            if (!AddedPersons.Contains(p.Id))
            {
                AddedPersons.Add(p.Id);

                Debug.WriteLine(p.Id + " " + p.FullName);
            }
            //8439 William Wall Junior

            if (p.Id == 8439)
            {
                Debug.WriteLine("");
            }
            //child relationship joins a person to a relationship i.e. their parents relationship
            var crs = f.ChildRelationship.Where(w => w.PersonId == testPersonId).ToList();

            LookupDescendants(f, testPersonId);

            foreach (var c in crs)
            {

                var rels = f.Relationship.Where(w => w.Id == c.RelationshipId).ToList();

                foreach (var r in rels)
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                        LookupAncestors(f, p1.Value);

                    var p2 = r.Person2Id;
                    if (p2 != null)
                        LookupAncestors(f, p2.Value);
                }
            }
        }


    }
}
