using System;
using System.Collections.Generic;
using System.Linq;
using DNAGedLib.Models;
using PlaceLib;

namespace DNAGedLib
{
    public class LocationUpdater
    {

        private static void UpdateBritishParents(DNAGEDContext dnagedContext)
        {

            Console.WriteLine("Updating - Via English Parents");


            var personsBornInEngland = dnagedContext.Persons.Where(w => w.BirthCountry == "England"
                                                                        && (w.FatherId != 0 || w.MotherId != 0));


            double total = personsBornInEngland.Count();
            double counter = 0;
            double percentage = 0.0;

            Console.WriteLine("finding english parents: " + total);

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

                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }

            var personsOfUnknownOrigins = dnagedContext.Persons.Where(w => w.BirthCountry == "Unknown");

            Console.WriteLine();

            total = personsOfUnknownOrigins.Count();
            counter = 0;
            percentage = 0.0;

            Console.WriteLine("Info - " + total + " Records to update");


            foreach (var p in personsOfUnknownOrigins)
            {
                if (englishParents.Contains(p.Id))
                {
                    p.BirthCountry = "England";
                    p.Memory = "calc from child";
                }

                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }

            Console.WriteLine();

            Console.WriteLine("Saving - Birth Country");

            dnagedContext.SaveChanges();

            Console.WriteLine("Saved - Birth Country");
        }

        private static void UpdateViaUSDescendants(DNAGEDContext dnagedContext)
        {
            Console.WriteLine("Updating - Via US Descendants");

            var filteredPersonSet = dnagedContext.Persons.Where(w => w.BirthCountry == "Unknown"
                                                                     && w.BirthCounty == ""
                                                                     && w.BirthPlace == "");

            var americans = dnagedContext.Persons.Where(w => w.BirthCountry == "USA").Select(s => s.Id).ToList();

            double total = filteredPersonSet.Count();
            double counter = 0;
            double percentage = 0.0;

            Console.WriteLine("Info - " + total + " Records to update");

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



                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }

            Console.WriteLine();

            Console.WriteLine("Saving - Birth Country");

            dnagedContext.SaveChanges();
            Console.WriteLine("Saved - Birth Country");
        }

        private static void UpdateCountry(DNAGEDContext dnagedContext)
        {
            Console.WriteLine("Updating - Birth Country");

            var filteredPersonSet = dnagedContext.Persons.Where(w => (w.BirthCountry == "Unknown")
                                                                     && w.BirthPlace != "");

            double total = filteredPersonSet.Count();
            double counter = 0;
            double percentage = 0.0;

            foreach (var p in filteredPersonSet)
            {
                if (p.BirthCountry == "Unknown")
                    p.BirthCountry = PlaceOperations.FindCountry(p.BirthPlace);

                //corrections
                if (p.BirthPlace.ToLower().Contains("Unknown"))
                    p.BirthPlace = "";


                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }

            Console.WriteLine();

            Console.WriteLine("Saving - Birth Country");
            dnagedContext.SaveChanges();
            Console.WriteLine("Saved - Birth Country");
        }

        private static void SetBirthCounty(DNAGEDContext dnagedContext, List<CountyDto> counties)
        {
            double total;
            double counter;
            double percentage;
            var filteredPersonSet2 = dnagedContext.Persons.Where(w => (w.BirthCountry == "England" ||
                                                                       w.BirthCountry == "Scotland" ||
                                                                       w.BirthCountry == "Wales" ||
                                                                       w.BirthCountry == "Unknown") &&
                                                                      (w.BirthCounty == "Unknown" || w.BirthCounty == "") &&
                                                                      w.BirthPlace != "");


            // filteredPersonSet2 = filteredPersonSet2.Where(p => p.Id == 342033618430);

            total = filteredPersonSet2.Count();



            Console.WriteLine("Updating - Birth County");

            Console.WriteLine("Info - " + total + " Unset counties");

            counter = 0;
            percentage = 0.0;

            foreach (var p in filteredPersonSet2)
            {
                foreach (var county in counties.Where(w => w.County.Trim() != ""))
                {
                    if (!String.IsNullOrEmpty(p.BirthPlace))
                    {
                        if (p.BirthPlace.ToLower().Contains(county.County))
                        {
                            p.BirthCounty = county.County;

                            p.BirthCountry = county.Country;
                        }
                    }
                }

                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }

            Console.WriteLine();
            Console.WriteLine("Saving - Birth County");
            dnagedContext.SaveChanges();
            Console.WriteLine("Saved - Birth County");
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
                    if (!String.IsNullOrEmpty(p.DeathPlace))
                    {
                        if (p.DeathPlace.ToLower().Contains(county) && p.DeathPlace != "")
                        {
                            p.DeathCounty = county;
                        }
                    }
                }

                percentage = ((counter / total) * 100);
                Console.Write("\r" + percentage + " %   ");
                counter++;
            }
        }


        public static void UpdateLocations()
        {

            DNAGEDContext dnagedContext = new DNAGEDContext();

            //   UpdateCountry(dnagedContext);

            //  UpdateBritishParents(dnagedContext);

            var counties = PlaceOperations.GetCounties();

            UpdateCountry(dnagedContext);

            UpdateBritishParents(dnagedContext);

            SetBirthCounty(dnagedContext, counties);

            UpdateViaUSDescendants(dnagedContext);

            Console.WriteLine("Finished press any key to exit");
        }


    }
}