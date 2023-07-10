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

        public static bool ContainsPair(string nameA,string nameB, List<IgnoreList> ignoreList)
        {
            var matchPair = new List<string>
            {
                nameA,
                nameB
            };

            foreach (var ignoreItem in ignoreList)
            {
                if (ignoreItem.Person1.ToLower().Trim() == matchPair[0].ToLower().Trim() && ignoreItem.Person2.ToLower().Trim() == matchPair[1].ToLower().Trim())
                {
                    return true;
                }
            }

            matchPair.Reverse();

            foreach (var ignoreItem in ignoreList)
            {
                if (ignoreItem.Person1.ToLower().Trim() == matchPair[0].ToLower().Trim() && ignoreItem.Person2.ToLower().Trim() == matchPair[1].ToLower().Trim())
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



                var mg = groupCollection.FindById(cp.Id) ?? groupCollection.CreateGroup(new Item
                {
                    PersonId = cp.Id, 
                    Origin = cp.Fact.Origin, 
                    YearFrom = cp.Fact.BirthYearFrom
                });
                
                mg.AddRange(comparisonPersons
                    .Where(w => w.Equals(cp)
                                && !ContainsPair(w.Fact.Surname, cp.Fact.Surname, ignoreList)
                                && !mg.Contains(w.Id)).Select(s =>
                        new Item
                        {
                            Origin = s.Fact.Origin,
                            PersonId = s.Id,
                            YearFrom = s.Fact.BirthYearFrom
                        }));

                groupCollection.WriteGroup(mg);


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
        
    }
}
