using System;
using System.Collections.Generic;
using GenDBContextNET.Contexts;
using GenDBContextNET.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GenDBContextNET.Data.Repositories
{
    public class LocationSqlLiteDataContext : ILocationDataContext
    {
        public List<(long personId, string place)> GetPeopleWithUnknowns(bool countryUpdated, PlaceCriteria placeCriteria)
        {
            using var dnagedContext = new DNAGEDContext();

            var placesOfBirth = dnagedContext.FillPersonMapPlacesBySQLite(countryUpdated, placeCriteria);

            return placesOfBirth;
        }

        public List<(long? FatherId, long? MotherId)> GetPersonsOfGivenNationality(string origin)
        {
            using var dnagedContext = new DNAGEDContext();

            var personsBornInEngland = dnagedContext.GetPersonsOfGivenNationality(origin);

            return personsBornInEngland;
        }

        public List<long> GetPersonsOfUnknownOrigins(bool isEnglish)
        {
            List<long> personsOfUnknownOrigins;

            using var dnagedContext = new DNAGEDContext();

            personsOfUnknownOrigins = dnagedContext.GetPersonsOfUnknownOrigins(isEnglish);

            return personsOfUnknownOrigins;
        }

        public void SaveEnglishParents(List<long> unknownOriginsPersons, HashSet<long> englishParentsPersons, bool isEnglish, IProgress<string> progress)
        {
            using var dnagedContext = new DNAGEDContext();

            dnagedContext.BulkUpdatePersonsNationality(unknownOriginsPersons, englishParentsPersons, isEnglish, progress);
        }

        public void SavePlaces(Dictionary<long, string> places, PlaceCriteria placeCriteria)
        {
            using var dnagedContext = new DNAGEDContext();

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

        public void MarkUnknowns()
        {
            using var dnagedContext = new DNAGEDContext();
            dnagedContext.Database.ExecuteSqlRaw(@"update Persons set BirthCountry = 'Unknown' where BirthCountry is null or BirthCountry = ''");
            dnagedContext.Database.ExecuteSqlRaw(@"update Persons set BirthCounty = 'Unknown' where BirthCounty is null or birthcounty = ''");
            dnagedContext.Database.ExecuteSqlRaw(@"update TreePersons set BirthCountry = 'Unknown' where BirthCountry is null or BirthCountry = ''");
            dnagedContext.Database.ExecuteSqlRaw(@"update TreePersons set BirthCounty = 'Unknown' where BirthCounty is null or birthcounty = ''");
        }

    }
}