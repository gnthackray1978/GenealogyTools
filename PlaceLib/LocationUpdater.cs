using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTools;
using GenDBContext.Models;

namespace PlaceLib
{   
    public class LocationFixer
    {
        private readonly IConsoleWrapper _consoleWrapper;
        private readonly ILocationDataContext _locationDataContext;

        public LocationFixer(ILocationDataContext locationDataContext, IConsoleWrapper consoleWrapper)
        {
            _locationDataContext = locationDataContext;
            _consoleWrapper = consoleWrapper;
        }

        public void UpdateBritishParents()
        {
            List<(long? FatherId,  long? MotherId)> personsBornInEngland = new List<(long? FatherId, long? MotherId)>();


            personsBornInEngland = _locationDataContext.GetPersonsOfGivenNationality("England");
            
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

                _consoleWrapper.ProgressSearch(counter, total, "English Parents", "possible records");

                counter++;
            }
            
            var personsOfUnknownOrigins = _locationDataContext.GetPersonsOfUnknownOrigins(true);

            if (!personsOfUnknownOrigins.Any())
            {
                _consoleWrapper.StatusReport("No English Parents",true);
                return;
            }

            var progress = new Progress<string>();

            _locationDataContext.SaveEnglishParents(personsOfUnknownOrigins, englishParents,true, progress);

            progress.ProgressChanged += (sender, e) =>
            {
                _consoleWrapper.StatusReport(e,true);
            };

            _consoleWrapper.StatusReport("Set country as England based on parents",true);
        }

        public void UpdateAmericanParents()
        {
            List<(long? FatherId, long? MotherId)> personsBornInEngland = new List<(long? FatherId, long? MotherId)>();


            personsBornInEngland = _locationDataContext.GetPersonsOfGivenNationality("USA");

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

                _consoleWrapper.ProgressSearch(counter, total, "American Parents", "possible records");

                counter++;
            }

            var personsOfUnknownOrigins = _locationDataContext.GetPersonsOfUnknownOrigins(false);

            if (!personsOfUnknownOrigins.Any())
            {
                _consoleWrapper.StatusReport("No American Parents",true);
                return;
            }
            var progress = new Progress<string>();

            _locationDataContext.SaveEnglishParents(personsOfUnknownOrigins, englishParents,false, progress);

            progress.ProgressChanged += (sender, e) =>
            {
                _consoleWrapper.StatusReport(e,true);
            };

            _consoleWrapper.StatusReport("Set country as America based on parents",true);

        }

        public void UpdateCountry(bool countryUpdated)
        {
            var countryDictionary = new Dictionary<long, string>();
           
            var places = _locationDataContext.GetPeopleWithUnknowns(countryUpdated,PlaceCriteria.ForMappings);
 
            if (!places.Any())
            {
                Console.WriteLine("No Unknown Countries");
                return;
            }
 
            foreach (var p in places)
            {
                if(!countryDictionary.ContainsKey(p.personId))
                    countryDictionary.Add(p.personId, PlaceOperations.FindCountry(p.place));
                 
            }

            _locationDataContext.SavePlaces(countryDictionary, PlaceCriteria.ForMappings);

            _consoleWrapper.StatusReport("Set known countries",false);
        }

        public void MapPlaceListToCounties(List<PlaceDto> knowns)
        {
            var personsWithUnknownCounties = _locationDataContext.GetPeopleWithUnknowns(false,PlaceCriteria.ForEnglishCounties);
            var countryDictionary = new Dictionary<long, string>();
            double total;
            double counter;
           
            total = personsWithUnknownCounties.Count();
            Console.WriteLine("");
            Console.WriteLine(total + " unknown counties");

            counter = 0;

            if (!personsWithUnknownCounties.Any())
            {
                Console.WriteLine("No Unknown Counties");
                return;
            }

            foreach (var p in personsWithUnknownCounties.ToList())
            {                    
                var birthPlace = p.place.ToLower();

                foreach (var pair in knowns)
                {
                    if (pair.County == "") continue;

                    var parts = birthPlace.Contains(',') ? birthPlace.Split(',').ToList() : birthPlace.Split(' ').ToList();

                    

                    foreach (var part in parts)
                    {
                        if(part.Trim() == "England") continue;

                        if (part.ToLower().Trim() == pair.Place.Trim().ToLower()) continue;

                        if (!countryDictionary.ContainsKey(p.personId))
                            countryDictionary.Add(p.personId, pair.County);
                    }
                  

                    
                }

                _consoleWrapper.ProgressUpdate(counter, total, "Birth County", "Unset counties");

                counter++;

            }

            _locationDataContext.SavePlaces(countryDictionary, PlaceCriteria.ForEnglishCounties);
        }

        public void SetBirthCounty(List<CountyDto> counties)
        {
            var filteredPersonSet2 = _locationDataContext.GetPeopleWithUnknowns(false, PlaceCriteria.ForAllUnknownCounties);
            var countryDictionary = new Dictionary<long, string>();
            double total;
            double counter;
  
            total = filteredPersonSet2.Count();

            counter = 0;

            if (!filteredPersonSet2.Any())
            {
                Console.WriteLine("No Unknown Counties");
                return;
            }

            foreach (var p in filteredPersonSet2.ToList())
            {               
                foreach (var county in counties)
                {
                    if (!p.place.ToLower().Contains(county.County)) continue;

                    if (!countryDictionary.ContainsKey(p.personId))
                        countryDictionary.Add(p.personId, county.County + "||" + county.Country);
                }

                _consoleWrapper.ProgressUpdate(counter, total, "Birth County", "Unset counties");

                counter++;
                 
            }
            _locationDataContext.SavePlaces(countryDictionary,PlaceCriteria.ForAllUnknownCounties);



        }

        public void EnsureUnknowns()
        {
            _locationDataContext.MarkUnknowns();

            _consoleWrapper.StatusReport("Marked unknown birth places as Unknown",false,false);
        }

        public static void UpdateLocations()
        {
            var consoleWrapper = new ConsoleWrapper();

            var locationFixer = new LocationFixer(new LocationSqlLiteDataContext(), consoleWrapper);

            locationFixer.EnsureUnknowns();

            locationFixer.UpdateCountry(false);

            locationFixer.UpdateBritishParents();

            locationFixer.UpdateAmericanParents();

            locationFixer.SetBirthCounty(PlaceOperations.GetCounties());

            consoleWrapper.StatusReport("Finished press any key to exit",false, true);
        }

        public static void MapPlaceListToCounties()
        {
            var consoleWrapper = new ConsoleWrapper();

            var locationFixer = new LocationFixer(new LocationSqlLiteDataContext(), consoleWrapper);

            locationFixer.MapPlaceListToCounties(PlaceMapCounty.GetMappings());

            consoleWrapper.StatusReport("Finished press any key to exit", true);
        }
    }
     
}