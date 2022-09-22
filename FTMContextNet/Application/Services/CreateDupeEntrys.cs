using FTMContext;
using FTMContextNet.Data.Repositories;
using LoggingLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FTMContextNet.Application.Services
{
    public class CreateDupeEntrys
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly Ilog _ilog;


        public CreateDupeEntrys(PersistedCacheRepository persistedCacheRepository, Ilog outputHandler)
        {
            _persistedCacheRepository = persistedCacheRepository;
            _ilog = outputHandler;

        }

        public void Execute()
        {

            _ilog.WriteLine("Executing Create Dupe Entries");

            var matchGroups = new MatchGroups();

            var comparisonPersons = _persistedCacheRepository.GetComparisonPersons();

            _ilog.WriteLine("Records to search: " + comparisonPersons.Count());
            int idx = 0;

            foreach (var cp in comparisonPersons)
            {
                //12707
                //13124

                if (cp.Id == 8260 || cp.Id == 27621)
                {
                    Debug.WriteLine("stop");
                }

                if (idx % 1000 == 0)
                    _ilog.WriteCounter(idx + " of " + comparisonPersons.Count());

                // if this person is in a existing group
                // get that group
                var mg = matchGroups.FindByPersonId(cp.Id) ?? matchGroups.CreateGroup(cp.Id, cp.Fact.Origin, cp.Fact.BirthYearFrom);

                AddIfMatch(comparisonPersons, cp, mg);

                matchGroups.SaveGroup(mg);


                idx++;
            }

            _ilog.WriteLine("Found: " + matchGroups.Groups.Count());

            matchGroups.SetAggregates();


           
            var tp = new List<KeyValuePair<int, string>>();

            foreach (var group in matchGroups.Groups.GroupBy(g => g.IncludedTrees))
            {
                _ilog.WriteCounter(group.Key);

                var p = group.OrderByDescending(o => o.LatestTree).First();

                foreach (var person in p.Persons)
                {                    
                    tp.Add(new KeyValuePair<int, string>(person.PersonId, group.Key));
                }
               
            }

            _persistedCacheRepository.AddDupeEntrys(tp);
        }

        private static void AddIfMatch(List<PersonDupeSearchSubset> comparisonPersons, PersonDupeSearchSubset cp, MatchGroup mg)
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

                var surnameCheck = cp.Fact.DoubleCheckSurname(p.Fact);

                if (surnameCheck && yearMatch && locationMatch && !originMatch)
                {

                    mg.AddPerson(p.Id, p.Fact.Origin, p.Fact.BirthYearFrom);

                }

            }
        }



    }
}
