using System;
using System.Collections.Generic;
using System.Linq;
using DNAGedLib.Models;
using GenDBContext.Models;
using Microsoft.EntityFrameworkCore;

namespace PlaceLib
{
    public class LocationSqlLiteDataContext : ILocationDataContext
    {
        public List<(long personId, string place)> GetPeopleWithUnknowns(bool countryUpdated, PlaceCriteria placeCriteria)
        {
            List<(long personId, string place)> placesOfBirth;

            using (var dnagedContext = new DNAGEDContext())
            {
                placesOfBirth = dnagedContext.FillPersonMapPlacesBySQLite(countryUpdated, placeCriteria);
            }

            return placesOfBirth;
        }

        public List<(long? FatherId, long? MotherId)> GetPersonsOfGivenNationality(string origin)
        {
            List<(long? FatherId, long? MotherId)> personsBornInEngland;

            using (var dnagedContext = new DNAGEDContext())
            {
                personsBornInEngland = dnagedContext.GetPersonsOfGivenNationality(origin);
            }

            return personsBornInEngland;
        }

        public List<long> GetPersonsOfUnknownOrigins(bool isEnglish)
        {
            List<long> personsOfUnknownOrigins;
             
            Func<T, bool> getWhereClause<T>() where T : IPersons
            {
                Func<T, bool> whereClause;
                if (isEnglish)
                {
                    whereClause = w => w.BirthCountry == "Unknown" && w.EnglishParentsChecked == false;
                }
                else
                {
                    whereClause = w => w.BirthCountry == "Unknown" && w.AmericanParentsChecked == false;
                }
                return whereClause;
            }

            using (var dnagedContext = new DNAGEDContext())
            {
              
                personsOfUnknownOrigins = dnagedContext.Persons.Where(getWhereClause<Persons>())
                    .Select(s => s.Id)
                    .ToList();

                personsOfUnknownOrigins.AddRange(dnagedContext.TreePersons.Where(getWhereClause<TreePersons>())
                    .Select(s => s.Id)
                    .ToList());

            }

            return personsOfUnknownOrigins;
        }

        public void SaveEnglishParents(List<long> unknownOriginsPersons, HashSet<long> englishParentsPersons, bool isEnglish)
        {
            using (var dnagedContext = new DNAGEDContext())
            {            
                dnagedContext.BulkUpdatePersonsNationality(unknownOriginsPersons, englishParentsPersons, isEnglish);
            }
        }

        public void SavePlaces(Dictionary<long, string> places, PlaceCriteria placeCriteria)
        {
            using (var dnagedContext = new DNAGEDContext())
            {
                switch (placeCriteria)
                {
                    case PlaceCriteria.ForAllUnknownCounties:
                        dnagedContext.BulkUpdatePersonsCountyAndCountry(places);             
                        break;
                    case PlaceCriteria.ForEnglishCounties:
                        dnagedContext.BulkUpdatePersonsCounty(places);                       
                        break;
                    case PlaceCriteria.ForMappings:
                        dnagedContext.BulkUpdatePersonsCountry(places);                    
                        break;
                }       
            }
        }

        public void MarkUnknowns()
        {
            using (var dnagedContext = new DNAGEDContext())
            {
                dnagedContext.Database.ExecuteSqlRaw(@"update Persons set BirthCountry = 'Unknown' where BirthCountry is null or BirthCountry = ''");
                dnagedContext.Database.ExecuteSqlRaw(@"update Persons set BirthCounty = 'Unknown' where BirthCounty is null or birthcounty = ''");
                dnagedContext.Database.ExecuteSqlRaw(@"update TreePersons set BirthCountry = 'Unknown' where BirthCountry is null or BirthCountry = ''");
                dnagedContext.Database.ExecuteSqlRaw(@"update TreePersons set BirthCounty = 'Unknown' where BirthCounty is null or birthcounty = ''");
            }
        }

    }
}