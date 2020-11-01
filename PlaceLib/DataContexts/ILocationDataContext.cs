using System;
using System.Collections.Generic;
using GenDBContext.Models;

namespace PlaceLib
{
    public interface ILocationDataContext
    {
        List<(long? FatherId, long? MotherId)> GetPersonsOfGivenNationality(string origin);
        List<long> GetPersonsOfUnknownOrigins(bool isEnglish);
        List<(long personId, string place)> GetPeopleWithUnknowns(bool countryUpdated, PlaceCriteria placeCriteria);

        void SaveEnglishParents(List<long> unknownOriginsPersons, HashSet<long> englishParentsPersons, bool isEnglish, IProgress<string> progress);    
        void SavePlaces(Dictionary<long, string> places, PlaceCriteria placeCriteria);

        void MarkUnknowns();
    }
}