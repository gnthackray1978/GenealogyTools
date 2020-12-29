using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DNAGedLib.Models;
using GenDBContext.Models;
using Microsoft.EntityFrameworkCore;

namespace PlaceLib
{
    public class LocationSqlServerDataContext : ILocationDataContext
    {         
        public List<(long personId, string place)> GetPeopleWithUnknowns(bool countryUpdated, PlaceCriteria placeCriteria)
        {
            List<(long personId, string place)> placesOfBirth;

            Func<Persons, bool> whereClause = w=>w.Id!=0;
 
            switch (placeCriteria)
            {
                case PlaceCriteria.ForAllUnknownCounties:
                    whereClause = w => (w.BirthCountry == "England" || w.BirthCountry == "Scotland" ||
                                        w.BirthCountry == "Wales" ||
                                        w.BirthCountry == "Unknown") &&
                                       (w.BirthCounty == "Unknown" ||
                                        w.BirthCounty == "") &&
                                       w.BirthPlace != "";
                    break;
                case PlaceCriteria.ForEnglishCounties:
                    whereClause = w => w.BirthCountry == "England" && w.BirthCounty == "Unknown" && w.BirthPlace != "";
                    break;
                case PlaceCriteria.ForMappings:
                    whereClause = w => w.BirthCountry == "Unknown" && !string.IsNullOrEmpty(w.BirthPlace) && w.CountryUpdated == countryUpdated;
                    break;
            }

          
             
            using (var dnagedContext = new DNAGEDContext())
            {
                placesOfBirth = dnagedContext.Persons.Where(whereClause)
                    .AsEnumerable()
                    .Select(c => (personId: c.Id, place: c.BirthPlace))
                    .ToList();
            }

            return placesOfBirth;
        }

        public List<(long? FatherId, long? MotherId)> GetPersonsOfGivenNationality(string origin)
        {
            List<(long? FatherId, long? MotherId)> personsBornInEngland;
            using (var dnagedContext = new DNAGEDContext())
            {
                personsBornInEngland = dnagedContext.Persons.Where(w =>
                        w.BirthCountry == origin && (w.FatherId != 0 || w.MotherId != 0))
                    .AsEnumerable()
                    .Select(c => (FatherId: c.FatherId, MotherId: c.MotherId))
                    .ToList();
            }

            return personsBornInEngland;
        }

        public List<long> GetPersonsOfUnknownOrigins(bool isEnglish)
        {         
            List<long> personsOfUnknownOrigins;

            using (var dnagedContext = new DNAGEDContext())
            {
                if (isEnglish)
                {
                    personsOfUnknownOrigins = dnagedContext.Persons.Where(w =>
                            w.BirthCountry == "Unknown" && w.EnglishParentsChecked == false)
                        .Select(s => s.Id)
                        .ToList();
                }
                else
                {
                    personsOfUnknownOrigins = dnagedContext.Persons.Where(w =>
                            w.BirthCountry == "Unknown" && w.AmericanParentsChecked == false)
                        .Select(s => s.Id)
                        .ToList();
                }

            }

            return personsOfUnknownOrigins;
        }

        public void SaveEnglishParents(List<long> unknownOriginsPersons, 
            HashSet<long> englishParentsPersons, bool isEnglish, IProgress<string> progress)
        {
            using (var dnagedContext = new DNAGEDContext())
            {
                //dnagedContext.Database.ExecuteSqlRaw()
                var matches = dnagedContext.Persons.Where(w => unknownOriginsPersons.Contains(w.Id));

                foreach (var m in matches)
                {
                    if (isEnglish)
                        m.EnglishParentsChecked = true;
                    else
                        m.AmericanParentsChecked = true;
                }

                matches = dnagedContext.Persons.Where(w => englishParentsPersons.Contains(w.Id));

                foreach (var m in matches)
                {
                    if(isEnglish)
                        m.BirthCountry = "England";             
                    else
                        m.BirthCountry = "USA";
                }

                dnagedContext.SaveChanges();
            }
        }

        public void SavePlaces(Dictionary<long,string> places, PlaceCriteria placeCriteria)
        {            
            using (var dnagedContext = new DNAGEDContext())
            {                
                var matches = dnagedContext.Persons.Where(w => places.ContainsKey(w.Id));
                               
                foreach (var m in matches)
                {                   
                    switch (placeCriteria)
                    {
                        case PlaceCriteria.ForAllUnknownCounties:
                            
                            var parts = Regex.Split(places[m.Id], "||");
                            m.BirthCounty = parts[0];
                            m.BirthCountry = parts[1];
                            break;
                        case PlaceCriteria.ForEnglishCounties:
                            m.BirthCounty = places[m.Id];
                            break;
                        case PlaceCriteria.ForMappings:
                            m.BirthCountry = places[m.Id];
                            break;
                    }
                }

                dnagedContext.SaveChanges();
            }
        }

        public void MarkUnknowns()
        {
            using (var dnagedContext = new DNAGEDContext())
            {
                dnagedContext.Database.ExecuteSqlRaw(@"update Persons set BirthCountry = 'Unknown' where BirthCountry is null or BirthCountry = ''");
                dnagedContext.Database.ExecuteSqlRaw(@"update Persons set BirthCounty = 'Unknown' where BirthCounty is null or birthcounty = ''");
            }
        }

    }
}