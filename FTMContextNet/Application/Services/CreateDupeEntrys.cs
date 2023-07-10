using FTMContext;
using FTMContextNet.Data.Repositories;
using LoggingLib;
using System.Collections.Generic;
using System.Linq;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent.Matches;
using FTMContextNet.Domain.Entities.Persistent.Cache;

namespace FTMContextNet.Application.Services
{
    public class CreateDupeEntrys
    {
        private readonly IPersistedCacheRepository _persistedCacheRepository;
        private readonly Ilog _ilog;
        private readonly IAuth _auth;
        
        public CreateDupeEntrys(IPersistedCacheRepository persistedCacheRepository, IAuth auth,
            Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ilog = outputHandler;
            _auth = auth;
        }

        public static bool ContainsPair(List<string> testItem, List<IgnoreList> ignoreList)
        {
            foreach (var ignoreItem in ignoreList)
            {
                if (ignoreItem.Person1.ToLower().Trim() == testItem[0].ToLower().Trim() && ignoreItem.Person2.ToLower().Trim() == testItem[1].ToLower().Trim())
                {
                    return true;
                }
            }

            testItem.Reverse();

            foreach (var ignoreItem in ignoreList)
            {
                if (ignoreItem.Person1.ToLower().Trim() == testItem[0].ToLower().Trim() && ignoreItem.Person2.ToLower().Trim() == testItem[1].ToLower().Trim())
                {
                    return true;
                }
            }

            return false;
        }

        public void Execute()
        {
            _persistedCacheRepository.DeleteDupes();

            _ilog.WriteLine("Executing Create Dupe Entries");

            var groupCollection = new GroupCollection();

            var comparisonPersons = _persistedCacheRepository.GetComparisonPersons();

            var ignoreList = _persistedCacheRepository.GetIgnoreList();

            

            int idx = 0;
            int comparisonTotal = comparisonPersons.Count();

            _ilog.WriteLine(comparisonTotal + " records");

            foreach (var cp in comparisonPersons)
            {
                //12707
                //13124

                //if (cp.Fact.Surname== "Bates" || cp.Fact.Surname == "Briggs")
                //{
                //    Debug.WriteLine("stop");
                //}

                if (idx % 1000 == 0)
                    _ilog.ProgressUpdate(idx, comparisonTotal," dupes");

                // if this person is in a existing group
                // get that group
                var mg = groupCollection.FindByPersonId(cp.Id) ?? groupCollection.CreateGroup(cp.Id, cp.Fact.Origin, cp.Fact.BirthYearFrom);

                AddIfMatch(comparisonPersons, cp, mg, ignoreList);

                groupCollection.SaveGroup(mg);


                idx++;
            }

            _ilog.WriteLine("Found: " + groupCollection.Groups.Count());

            groupCollection.SetAggregates();


           
            var tp = new List<KeyValuePair<int, string>>();

            foreach (var group in groupCollection.Groups.GroupBy(g => g.IncludedTrees))
            {
                _ilog.WriteCounter(group.Key);

                var p = group.OrderByDescending(o => o.LatestTree).First();

                foreach (var person in p.Items)
                {                    
                    tp.Add(new KeyValuePair<int, string>(person.PersonId, group.Key));
                }
               
            }

            _persistedCacheRepository.AddDupeEntrys(tp,_auth.GetUser());
        }

        private static void AddIfMatch(List<PersonDupeSearchSubset> comparisonPersons,
            PersonDupeSearchSubset cp, Group mg, List<IgnoreList> ignoreList)
        {
            foreach (var p in comparisonPersons
                                .Where(w => w.FamilyName == cp.FamilyName
                                          && w.GivenName == cp.GivenName
                                          && cp.Fact != null
                                          && w.Fact != null
                                          && !mg.Contains(w.Id)))
            {

                var yearMatch = cp.Fact.MatchBirthYear(p.Fact);
                    
                var locationMatch = cp.Fact.MatchLocations(p.Fact);

                var originMatch = cp.Fact.Origin == p.Fact.Origin;

                //p.FamilyName
                //cp.FamilyName

                var matchPair = new List<string>
                {
                    p.Fact.Surname,
                    cp.Fact.Surname
                };

                var surnameCheck = ContainsPair(matchPair, ignoreList);

                // var surnameCheck = cp.Fact.DoubleCheckSurname(p.Fact);

                if (yearMatch && locationMatch && !originMatch && !surnameCheck)
                {

                    mg.AddPerson(p.Id, p.Fact.Origin, p.Fact.BirthYearFrom);

                }

            }
        }



    }
}
