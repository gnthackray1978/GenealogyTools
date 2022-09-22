using FTMContext;
using LoggingLib;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTMContextNet.Data.Repositories
{
    public class InMemoryCacheRepository
    {
        protected InMemoryCacheContext _inMemoryCacheContext;
        protected Ilog _ilog;

        public InMemoryCacheRepository(InMemoryCacheContext inMemoryCacheContext, Ilog iLog)
        {
            this._inMemoryCacheContext = inMemoryCacheContext;
            this._ilog = iLog;
        }

        public void CheckForChangedDates()
        {


            //missing persons
            //changed persons

            List<int> missingPersonIds = new List<int>();
            List<int> updatedPersonIds = new List<int>();

            int peopleCount = this._inMemoryCacheContext.personCache.Count();
     

            _ilog.WriteLine(peopleCount + " Records ");

            foreach (var p in this._inMemoryCacheContext.personCache.Values)
            {
                if (!_inMemoryCacheContext.Type90Facts.Select(s => s.Key).Contains(p.Id))
                {
                    missingPersonIds.Add(p.Id);
                }
                else
                {

                    var processDateReturnType = this._inMemoryCacheContext.GetPersonBirthDateRange(p.Id);
                    var currentOriginDates = processDateReturnType.RangeString.Trim();

                    var existingPair = _inMemoryCacheContext.Type90Facts.FirstOrDefault(f => f.Key == p.Id);
                    var existingDateRange = "";

                    var parts = Regex.Split(existingPair.Value, @"\|\|");

                    if (parts.Length > 0)
                    {
                        existingDateRange = parts[0].Trim();
                    }

                    if (existingDateRange != currentOriginDates)
                    {
                        updatedPersonIds.Add(p.Id);
                    }

                }
            }

        }


        public int GetPersonCacheSize() {
            return _inMemoryCacheContext.personCache.Count;
        }

        public int GetMarriageCacheSize()
        {
            return _inMemoryCacheContext.relationshipDictionary.Values.Count;
        }

        public Dictionary<int,PersonSubset>.ValueCollection GetPersons() {
            return _inMemoryCacheContext.personCache.Values;
        }

        public Dictionary<int, RelationSubSet>.ValueCollection GetMarriages()
        {
            return _inMemoryCacheContext.relationshipDictionary.Values;
        }

        public ProcessDateReturnType GetPersonBirthDateRange(int personId)
        {
            var birthDateRange = _inMemoryCacheContext.GetPersonBirthDateRange(personId);

            return birthDateRange;
        }

        public ProcessLocationReturnType GetAllLocationsForPerson(int personId)
        {
            var associatedLocationData = _inMemoryCacheContext.GetAllLocationsForPerson(personId);

            return associatedLocationData;
        }

        public List<int> GetParentIds(int personId)
        {
            var parents = _inMemoryCacheContext.GetParentIds(personId);

            return parents;
        }

        public string GetOrigin(int personId) {

            _inMemoryCacheContext.originDictionary.TryGetValue(personId, out string origin);

            return origin;
        }

    }
}
