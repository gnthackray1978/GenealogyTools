﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using DNAGedLib.Models;
using GenDBContext.Models;
using Microsoft.Data.SqlClient;

namespace PlaceLib
{
    public class MissingCounties
    {
        public List<string> unknowns { get; set; }

        public MissingCounties()
        {

        }


        public void FindCounty()
        {


        }
    }

    public class LookupPair
    {
        public string SearchString { get; set; }
        public string County { get; set; }
    }

    public class ConsoleWrapper
    {
        public static void ProgressSearch(double counter, double total, string message, string tailMessage = "")
        {
            double percentage = 0.0;

            percentage = counter / total * 100;
            Console.Write("\r SEARCHING " + message.Trim() + " " + percentage + " %   of " + total + " " + tailMessage.Trim());

        }

        public static void ProgressUpdate(double counter, double total, string message, string tailMessage = "")
        {
            double percentage = 0.0;

            percentage = counter / total * 100;
            Console.Write("\r UPDATING " + message.Trim() + " " + percentage + " %   of " + total + " " + tailMessage.Trim());

        }

        public static void HandleSave(DNAGEDContext dnagedContext, string message)
        {
            Console.WriteLine();

            Console.WriteLine("SAVING " + message);

            dnagedContext.SaveChanges();

            Console.Write(" SAVED ");
        }

        public static void HandleBulkSave<T>(DNAGEDContext dnagedContext, IEnumerable<T> entities, string message) where T : class
        {
            Console.WriteLine();

            Console.WriteLine("SAVING " + message);

            dnagedContext.BulkUpdate(entities);

            Console.Write(" SAVED ");
        }

    }

    public class LocationUpdater
    {
        /// <summary>
        /// If Parents are born in England then set childs birth country as England.
        /// </summary>
        private static void UpdateBritishParents()
        {
            using (var dnagedContext = new DNAGEDContext())
            {

                var personsBornInEngland = dnagedContext.Persons.Where(w => w.BirthCountry == "England"
                                                                            && (w.FatherId != 0 || w.MotherId != 0));


                double total = personsBornInEngland.Count();
                double counter = 0;


                HashSet<long> englishParents = new HashSet<long>();

                foreach (var p in personsBornInEngland)
                {
                    if (p.FatherId != null && p.FatherId != 0)
                    {
                        if (!englishParents.Contains(p.FatherId.Value))
                            englishParents.Add(p.FatherId.Value);
                    }

                    if (p.MotherId != null && p.MotherId != 0)
                    {
                        if (!englishParents.Contains(p.MotherId.Value))
                            englishParents.Add(p.MotherId.Value);
                    }

                    ConsoleWrapper.ProgressSearch(counter, total, "English Parents", "possible records");

                    counter++;
                }

                var personsOfUnknownOrigins = dnagedContext.Persons.Where(w => w.BirthCountry == "Unknown" && w.EnglishParentsChecked == false);

                Console.WriteLine();

                total = personsOfUnknownOrigins.Count();
                counter = 0;

                if (!personsOfUnknownOrigins.Any())
                {
                    Console.WriteLine("No English Parents");
                    return;
                }
                List<Persons> persons = new List<Persons>();

                foreach (var p in personsOfUnknownOrigins)
                {
                    if (englishParents.Contains(p.Id))
                    {
                        p.BirthCountry = "England";
                        p.Memory = "calc from child";

                    }

                    p.EnglishParentsChecked = true;

                    persons.Add(p);

                    ConsoleWrapper.ProgressUpdate(counter, total, "Via English Parents", "records");

                    counter++;
                }

                ConsoleWrapper.HandleBulkSave(dnagedContext, persons, "UK Birth Country");
            }
        }

        private static void UpdateViaUSDescendants()
        {

            using (var dnagedContext = new DNAGEDContext())
            {


                var filteredPersonSet = dnagedContext.Persons.Where(w => w.BirthCountry == "Unknown"
                                                                         && w.BirthCounty == ""
                                                                         && w.BirthPlace == ""
                                                                         && w.AmericanParentsChecked == false);

                var americans = dnagedContext.Persons.Where(w => w.BirthCountry == "USA").Select(s => s.Id).ToList();

                double total = filteredPersonSet.Count();

                if (!filteredPersonSet.Any())
                {
                    Console.WriteLine("No Unknown Americans");
                    return;
                }



                double counter = 0;
                foreach (var p in filteredPersonSet)
                {
                    if (p.FatherId != 0)
                    {
                        if (americans.Contains(p.FatherId.Value))
                        {
                            p.BirthCountry = "USA";
                        }
                    }

                    if (p.MotherId != 0)
                    {
                        if (americans.Contains(p.MotherId.Value))
                        {
                            p.BirthCountry = "USA";
                        }
                    }

                    p.AmericanParentsChecked = true;

                    ConsoleWrapper.ProgressUpdate(counter, total, "Via US Parents", "records");

                    counter++;
                }

                ConsoleWrapper.HandleSave(dnagedContext, "US Birth Country");

            }
        }

        /// <summary>
        /// In Persons table. Where Country is unknown and there is a place name attempt to guess the country.
        /// </summary>
        /// <param name="countryUpdated">If False only set previously unset countries.</param>
        private static void UpdateCountry(bool countryUpdated)
        {
            using (var dnagedContext = new DNAGEDContext())
            {
                var filteredPersonSet = dnagedContext.Persons.Where(w => w.BirthCountry == "Unknown"
                                                                         && w.BirthPlace != "" && w.CountryUpdated == countryUpdated);

                double total = filteredPersonSet.Count();
                double counter = 0;


                if (!filteredPersonSet.Any())
                {
                    Console.WriteLine("No Unknown Countries");
                    return;
                }

                List<Persons> persons = new List<Persons>();

                foreach (var p in filteredPersonSet)
                {
                    if (p.BirthCountry == "Unknown")
                    {
                        p.BirthCountry = PlaceOperations.FindCountry(p.BirthPlace);

                    }

                    p.CountryUpdated = true;
                    persons.Add(p);

                    ConsoleWrapper.ProgressUpdate(counter, total, "Birth Country", "records");

                    counter++;
                }

                ConsoleWrapper.HandleBulkSave(dnagedContext, persons, "Birth Place");

            }
        }

        /// <summary>
        /// Takes list of places and maps them to a county.
        /// In Person Table.
        /// </summary>
        public static void MapPlaceListToCounties(List<PlaceDto> knowns)
        {            

            using (var dnagedContext = new DNAGEDContext())
            {

                double total;
                double counter;
                List<string> unknowns = new List<string>();
                var personsWithUnknownCounties = dnagedContext.Persons.Where(w => w.BirthCountry == "England" &&
                                                                          w.BirthCounty == "Unknown" &&
                                                                          w.BirthPlace != "");


                // filteredPersonSet2 = filteredPersonSet2.Where(p => p.Id == 342033618430);

                total = personsWithUnknownCounties.Count();
                Console.WriteLine("");
                Console.WriteLine(total + " unknown counties");

                counter = 0;

                if (!personsWithUnknownCounties.Any())
                {
                    Console.WriteLine("No Unknown Counties");
                    return;
                }


                List<Persons> persons = new List<Persons>();

                foreach (var p in personsWithUnknownCounties.ToList())
                {

                    if (!string.IsNullOrEmpty(p.BirthPlace))
                    {
                        var birthPlace = p.BirthPlace.ToLower();//.Replace(" ", ",");

                        //var parts = birthPlace.Split(',');

                        //foreach (var part in parts)
                        //{


                        foreach (var pair in knowns)
                        {
                            if (birthPlace.Contains(pair.Place)) p.BirthCounty = pair.County;
                        }
                        //  }       


                    }

                    if (persons.All(a => a.Id != p.Id))
                        persons.Add(p);


                    ConsoleWrapper.ProgressUpdate(counter, total, "Birth County", "Unset counties");

                    counter++;

                }

                ConsoleWrapper.HandleBulkSave(dnagedContext, persons, "Birth County");

            }
        }


        private static void SetBirthCounty(List<CountyDto> counties)
        {
            //if (p.BirthPlace.ToLower().Contains("london")) p.BirthCounty = "Middlesex";
            //  if (p.BirthPlace.ToLower().Contains("huntingdon")) p.BirthCounty = "Huntingdonshire";




            using (var dnagedContext = new DNAGEDContext())
            {

                double total;
                double counter;

                var filteredPersonSet2 = dnagedContext.Persons.Where(w => (w.BirthCountry == "England" ||
                                                                           w.BirthCountry == "Scotland" ||
                                                                           w.BirthCountry == "Wales" ||
                                                                           w.BirthCountry == "Unknown") &&
                                                                          (w.BirthCounty == "Unknown" ||
                                                                           w.BirthCounty == "") &&
                                                                          w.BirthPlace != "");
                //&& w.CountyUpdated ==false);


                // filteredPersonSet2 = filteredPersonSet2.Where(p => p.Id == 342033618430);

                total = filteredPersonSet2.Count();

                counter = 0;

                if (!filteredPersonSet2.Any())
                {
                    Console.WriteLine("No Unknown Counties");
                    return;
                }

                int idx = 0;

                List<Persons> persons = new List<Persons>();

                foreach (var p in filteredPersonSet2.ToList())
                {
                    p.CountyUpdated = true;

                    if (!string.IsNullOrEmpty(p.BirthPlace))
                    {
                        foreach (var county in counties)
                        {
                            if (p.BirthPlace.ToLower().Contains(county.County))
                            {
                                p.BirthCounty = county.County;
                                p.BirthCountry = county.Country;
                            }
                        }

                    }

                    if (!persons.Any(a => a.Id == p.Id))
                        persons.Add(p);


                    ConsoleWrapper.ProgressUpdate(counter, total, "Birth County", "Unset counties");

                    counter++;

                    //if (idx % 50000 == 0)
                    //{
                    //    Console.Write(" Saving");
                    //    dnagedContext.SaveChanges();
                    //}

                    idx++;
                }

                ConsoleWrapper.HandleBulkSave(dnagedContext, persons, "Birth County");

            }
        }

        private static void SetDeathCounty(DNAGEDContext dnagedContext, List<string> counties)
        {
            double total;
            double counter;
            double percentage;
            var filteredPersonSet2 = dnagedContext.Persons.Where(w => (w.DeathCountry == "England" ||
                                                                       w.DeathCountry == "Scotland" ||
                                                                       w.DeathCountry == "Wales" ||
                                                                       w.DeathCountry == "Unknown") &&
                                                                      w.DeathCounty == "" &&
                                                                      w.DeathPlace != "");

            total = filteredPersonSet2.Count();

            counter = 0;
            percentage = 0.0;

            foreach (var p in filteredPersonSet2)
            {
                foreach (var county in counties)
                {
                    if (!string.IsNullOrEmpty(p.DeathPlace))
                    {
                        if (p.DeathPlace.ToLower().Contains(county) && p.DeathPlace != "")
                        {
                            p.DeathCounty = county;
                        }
                    }
                }

                percentage = counter / total * 100;
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }
        }
       
        /// <summary>
        /// If BirthCountry or BirthCounty is set to null or it's empty. Set it to 'Unknown'
        /// For EVERY person in the Persons table.
        /// </summary>
        public static void EnsureUnknowns()
        {

            using (var context = new DNAGEDContext())
            {
                var persons = context.Persons.Where(w => w.BirthCountry == null
                                                         || w.BirthCountry == ""
                                                         || w.BirthCounty == null
                                                         || w.BirthCounty == "").ToList();

                ConsoleWrapper.ProgressUpdate(0, persons.Count, "Birth Country and County To Unknown", "records");

                foreach (var p in persons.Where(w => string.IsNullOrEmpty(w.BirthCountry)))
                {
                    p.BirthCountry = "Unknown";
                }

                foreach (var p in persons.Where(w => string.IsNullOrEmpty(w.BirthCounty)))
                {
                    p.BirthCounty = "Unknown";
                }

                ConsoleWrapper.ProgressUpdate(persons.Count, persons.Count, "Birth Country and County To Unknown", "records");

                ConsoleWrapper.HandleBulkSave(context, persons, "Birth Country and County");

            }

            Console.WriteLine("Updating Birth Country and County");
        }

        /// <summary>
        /// Set County and Country in Persons table.
        /// </summary>
        public static void UpdateLocations()
        {


            EnsureUnknowns();

            var counties = PlaceOperations.GetCounties();

            UpdateCountry(false);

            UpdateBritishParents();

            SetBirthCounty(counties);

            UpdateViaUSDescendants();

            Console.WriteLine("Finished press any key to exit");
        }


    }
}