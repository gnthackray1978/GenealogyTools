using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using DNAGedLib;
using DNAGedLib.Models;
using GenDBContext.Models;
using Microsoft.EntityFrameworkCore;
using RootsLib.Model;

namespace RootsLib
{
    public class RootParser
    {
        public List<FamilyTable> Families { get; set; }
        public List<ChildTable> Children { get; set; }
        public List<EventTable> Events { get; set; }
        public List<PlaceTable> Places { get; set; }
        public List<NameTable> Names { get; set; }

        public List<PersonWithAncestor> ChildList { get; set; } = new List<PersonWithAncestor>();
        public PersonWithAncestor CurrentTree { get; set; }

        public void Init()
        {


            using (var context = new DNAGEDContext())
            {
             
                var oldPersons = context.Persons.Where(pid => pid.RootsEntry);

                Console.WriteLine("Removing  " + oldPersons.Count() + " old records");

                context.RemoveRange(oldPersons);

                context.SaveChanges();

            }




            Roots roots = new Roots();


            DNAGEDContext dnagedContext = new DNAGEDContext();
            var rootpeople = roots.NameTable.Where(w => w.Surname.Contains("_"));

            Families = roots.FamilyTable.ToList();
            Children = roots.ChildTable.ToList();
            Events = roots.EventTable.ToList();
            Places = roots.PlaceTable.ToList();
            Names = roots.NameTable.ToList();

            Console.WriteLine("Root people count: " + rootpeople.Count());
            CurrentTree = new PersonWithAncestor();

            foreach (var rp in rootpeople)
            {
                var name = Names.FirstOrDefault(f => f.OwnerId == rp.OwnerId);

                CurrentTree.AncestorId = rp.OwnerId.GetValueOrDefault();

                if (name != null)
                    CurrentTree.AncestorDescription = name.Given + " " + name.Surname;

                CurrentTree.AncestorDescription = CurrentTree.AncestorDescription.Trim();

                Console.WriteLine("Searching for: " + rp.Given + " " + rp.Surname);

                var searchId = rp.OwnerId;

                //getChildren(searchId);

                getParents(searchId);
            }



            double total = ChildList.Count;
            double counter = 0;
            double percentage = 0.0;

            int addedRecords = 0;

            var personsAdded = dnagedContext.Persons.Select(p => p.Id).ToList();
            List<int> dupes = new List<int>();

            foreach (var s in ChildList)
            {
                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;



                var name = Names.FirstOrDefault(f => f.OwnerId == s.PersonId);

                List<EventDetail> birthEvents = new List<EventDetail>();
                birthEvents.Add(GetDetail(1, s.PersonId));
                birthEvents.Add(GetDetail(7, s.PersonId));

                List<EventDetail> deathEvents = new List<EventDetail>();
                deathEvents.Add(GetDetail(2, s.PersonId));
                deathEvents.Add(GetDetail(4, s.PersonId));

                if (personsAdded.Any(p => p == s.PersonId))
                {
                    //Console.Write("dupe not added " + s.PersonId);
                    dupes.Add(s.PersonId);
                    continue;
                }

                var parents = GetParents(Families, Children, s.PersonId);


                dnagedContext.Persons.Add(new Persons
                {
                    Id = s.PersonId,
                    ChristianName = name?.Given ?? "",
                    Surname = name?.Surname ?? "",

                    DeathPlace = deathEvents.OrderByDescending(o => o.PercentageComplete).FirstOrDefault()?.DeathPlace,
                    DeathDate = deathEvents.OrderByDescending(o => o.PercentageComplete).FirstOrDefault()?.DeathDateString,
                    DeathYear = deathEvents.OrderByDescending(o => o.PercentageComplete).FirstOrDefault()?.DeathYear,

                    BirthPlace = birthEvents.OrderByDescending(o => o.PercentageComplete).FirstOrDefault()?.DeathPlace,
                    BirthDate = birthEvents.OrderByDescending(o => o.PercentageComplete).FirstOrDefault()?.DeathDateString,
                    BirthYear = birthEvents.OrderByDescending(o => o.PercentageComplete).FirstOrDefault()?.DeathYear,
                    Memory = s.AncestorDescription,
                    FatherId = parents.Item1,
                    MotherId = parents.Item2,
                    RootsEntry = true
                });
          
                addedRecords++;
                personsAdded.Add(s.PersonId);
            }

            dnagedContext.SaveChanges();

            Console.WriteLine(dupes.Count + " dupes");
        }


        private Tuple<int,int> GetParents(List<FamilyTable> families, List<ChildTable> childrens, int personId)
        {
            //search through children

            var childFamilyLinks = childrens.Where(w => w.ChildId == personId);

            if (childFamilyLinks.Any())
            {
                var childLink = childFamilyLinks.FirstOrDefault();

                if (childLink != null)
                {
                    var family = families.FirstOrDefault(f => f.FamilyId == childLink.FamilyId);

                    if(family!=null)
                        return new Tuple<int, int>(family.FatherId.GetValueOrDefault(), family.MotherId.GetValueOrDefault());
                }
            }

            return new Tuple<int, int>(0,0);
        }

        private string Decode(string date)
        {            
            //string date = "D.+18410000.A+00000000..";
            string output = "";
            if (date.Contains("D"))
            {
                Regex reg = new Regex(@"\d\d\d\d\d\d\d\d");

                var m = reg.Match(date);

                if (m.Success)
                {
      //              Console.WriteLine(m.Captures[0].Value);
                    string numericPart = m.Captures[0].Value;
                    string yearPart = numericPart.Substring(0, 4);
                    string monthPart = numericPart.Substring(4, 2);
                    string dayPart = numericPart.Substring(6, 2);

                    int year = Int32.Parse(yearPart);
                    int month = Int32.Parse(monthPart);
                    int day = Int32.Parse(dayPart);

                    if (year != 0)
                    {

                        if (month != 0 && day != 0)
                        {
                            DateTime dt = new DateTime(year, month, day);
                            output = dt.ToString("d MMM yyyy");
                        }
                        else
                        {
                            output = yearPart;
                        }


                        if (date.Contains("A"))
                        {
                            output = "Abt. " + output;
                        }
                    }

                    //Console.WriteLine(output);
                }

            }

            return output;
        }

        private EventDetail GetDetail(int eventTypeId, int personId)
        {
            EventDetail eventDetail = new EventDetail();

            var evt = Events.FirstOrDefault(w => w.OwnerId == personId && w.EventType == eventTypeId);
            
            eventDetail.PercentageComplete = 0;
        
            if (evt != null)
            {
                var place = Places.FirstOrDefault(f => f.PlaceId == evt.PlaceId);

                eventDetail.DeathYear = MatchTreeHelpers.ExtractYear(evt.Date);

                if(eventDetail.DeathYear >0) eventDetail.PercentageComplete ++;

                eventDetail.DeathDateString = Decode(evt.Date);



                //D.+18981214..+00000000..
                //D.+18410000.A+00000000..


                if (place != null)
                {
                    eventDetail.PercentageComplete++;

                    if(place.Name!="") eventDetail.PercentageComplete++;

                    if (place.Normalized != "")
                    {
                        eventDetail.DeathPlace = place.Normalized;
                        eventDetail.PercentageComplete++;
                    }
                    else
                    {
                        eventDetail.DeathPlace = place.Name;
                    }
                       
                }

            }



            return eventDetail;
        }

        private void getParents(int? searchId)
        {
            if (searchId == 0) return;

            var personFamily = Children.Where(c => c.ChildId == searchId).ToList();

            //    Console.WriteLine(personFamily.Count + " family count");


            ChildList.Add(new PersonWithAncestor()
            {
                AncestorId = CurrentTree.AncestorId,
                AncestorDescription = CurrentTree.AncestorDescription,
                PersonId = searchId.GetValueOrDefault()
            });

            if (personFamily.Count > 0)
            {
                foreach (var family in personFamily)
                {
                    var fams = Families.Where(w => w.FamilyId == family.FamilyId).ToList();

                    foreach (var f in fams)
                    {
                        if (f.FatherId.HasValue)
                        {
                            getParents(f.FatherId);
                        }

                        if (f.MotherId.HasValue)
                        {
                            getParents(f.MotherId);
                        }
                    }

                }

            }

        }

        //private void getChildren(int? searchId)
        //{
        //    var fams = Families.Where(w => w.FatherId == searchId || w.MotherId == searchId).ToList();

        //    Console.WriteLine(fams.Count +  " families");


        //    foreach (var f in fams)
        //    {
        //        var currentChildren = Children.Where(c => c.FamilyId == f.FamilyId).ToList();

        //        Console.WriteLine(currentChildren.Count + " children");

        //        foreach (var child in currentChildren)
        //        {
        //            if (child.ChildId.HasValue)
        //            {
        //                getChildren(child.ChildId.Value);

        //                //Console.WriteLine(rp.Given + " " + rp.Surname);

        //                ChildList.Add(new PersonWithAncestor()
        //                {
        //                    AncestorId = CurrentTree.AncestorId,
        //                    PersonId = child.ChildId.Value
        //                }  );
        //            }
        //        }
        //    }
        //}
    }
}