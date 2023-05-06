using FTMContext;
using LoggingLib;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using QuickGed.Types;
using ProcessDateReturnType = FTMContext.ProcessDateReturnType;

namespace FTMContextNet.Data.Repositories
{
    public interface IInMemoryCacheRepository
    {
        int GetPersonCacheSize();
        int GetMarriageCacheSize();
        Dictionary<int,PersonSubset>.ValueCollection GetPersons();
        Dictionary<int, RelationSubSet>.ValueCollection GetMarriages();
        ProcessDateReturnType GetPersonBirthDateRange(int personId);
        ProcessLocationReturnType GetAllLocationsForPerson(int personId);
        List<int> GetParentIds(int personId);
        PersonOrigin GetOrigin(int personId);
    }

    public class InMemoryCacheRepository : IInMemoryCacheRepository
    {
        protected IInMemoryCacheContext _inMemoryCacheContext;
        protected Ilog _ilog;

        public InMemoryCacheRepository(IInMemoryCacheContext inMemoryCacheContext, Ilog iLog)
        {
            this._inMemoryCacheContext = inMemoryCacheContext;
            this._ilog = iLog;
        }
        
        public int GetPersonCacheSize() {
            return _inMemoryCacheContext.PersonCache.Count;
        }

        public int GetMarriageCacheSize()
        {
            return _inMemoryCacheContext.RelationshipDictionary.Values.Count;
        }

        public Dictionary<int,PersonSubset>.ValueCollection GetPersons() {
            return _inMemoryCacheContext.PersonCache.Values;
        }

        public Dictionary<int, RelationSubSet>.ValueCollection GetMarriages()
        {
            return _inMemoryCacheContext.RelationshipDictionary.Values;
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

        public PersonOrigin GetOrigin(int personId) {


            _inMemoryCacheContext.OriginDictionary.TryGetValue(personId, out PersonOrigin origin);

            return origin ?? new PersonOrigin();
        }

    }
}
