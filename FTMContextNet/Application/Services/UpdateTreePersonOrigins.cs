using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FTMContext;
using LoggingLib;
using FTMContextNet.Data.Repositories;

namespace FTMContextNet.Application.Services
{
    public class UpdateTreePersonOrigins
    {
        // private FTMakerContext _context;
        //   private PersistedCacheContext _cacheContext;
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private FTMMakerRepository _fTMMakerRepository;
        private Ilog _ilog;

        public Dictionary<int,bool> AddedPersons = new Dictionary<int,bool>();
        public List<int> SpouseList = new List<int>();

        public List<int> RelList = new List<int>();

        public List<int> ChildList = new List<int>();

        public List<ChildRelationshipSubset> ChildRelationshipSubsets = new List<ChildRelationshipSubset>();

        public HashSet<int> GroupPersonIds = new HashSet<int>();

        public Dictionary<int, int[]> RelationsDictionary = new Dictionary<int, int[]>();

        public Dictionary<int, HashSet<int>> ChildWithRelationship = new Dictionary<int, HashSet<int>>();
        private HashSet<int> _exceptions;

        public UpdateTreePersonOrigins(FTMMakerRepository context,
            PersistedCacheRepository persistedCacheRepository,
            Ilog ilog)
        {
            _ilog = ilog;
            _fTMMakerRepository = context;
            _persistedCacheRepository = persistedCacheRepository;
        }

        public static UpdateTreePersonOrigins Create(FTMMakerRepository fTMMakerRepository,
            PersistedCacheRepository persistedCacheRepository,
            Ilog ilog)
        {

            return new UpdateTreePersonOrigins(fTMMakerRepository, persistedCacheRepository, ilog);
        }


        public void Execute()
        {
            ChildRelationshipSubsets = _fTMMakerRepository.GetChildrenByPersonId();
            
          //  GroupPersonIds = _fTMMakerRepository.GetListOfTreeIds();

            RelationsDictionary = _fTMMakerRepository.GetRelationships();

            ChildWithRelationship = _fTMMakerRepository.GetChildrenWithRelationship();

            _ilog.WriteLine("Executing UpdateTreePersonOrigins");

            _persistedCacheRepository.DeleteOrigins();



            var rootPeople = _fTMMakerRepository.GetTreeRootPeople();
            var groups = _fTMMakerRepository.GetGroupPerson();
            _exceptions = groups.Select(s => s.Id).ToHashSet();
            if(!_exceptions.Contains(0))
                _exceptions.Add(0);

            int nextId = _persistedCacheRepository.OriginPersonCount();

            int total = rootPeople.Count;
            int counter = 1;

            foreach (var rootPerson in rootPeople)
            {
               

                AddedPersons = new Dictionary<int, bool>();
                SpouseList = new List<int>();
                RelList = new List<int>();
                ChildList = new List<int>();


                //if (rootPerson.Surname != "_28_lyn!lawrence")
                //    continue;

                LookupAncestors(rootPerson.Id);

                _ilog.WriteCounter(counter + " of " + total + " " + rootPerson.FamilyName + " " + AddedPersons.Count());

                //FTMPERSONORIGIN shoule be empty!

                Debug.WriteLine(rootPerson.FamilyName + " , " + AddedPersons.Count + " , " + SpouseList.Count);

                if (SpouseList.Count > 0)
                {
                    var spouseListIdx = 0;

                    while (spouseListIdx < SpouseList.Count)
                    {
                        LookupAncestors(SpouseList[spouseListIdx],false);

                        spouseListIdx++;
                    }
                }

                foreach (var exception in _exceptions)
                {
                    AddedPersons.Remove(exception);
                }
                
                if(AddedPersons.Count > 0)
                    nextId = _persistedCacheRepository.SaveFtmPersonOrigins(nextId, AddedPersons, rootPerson.FamilyName, rootPerson.FullName);
               
                counter ++;

            }


            nextId = groups.Aggregate(nextId, (current, group) => _persistedCacheRepository.SaveFtmPersonOrigins(current, new Dictionary<int, bool> { { group.Id, false } }, group.FamilyName, group.FullName));


            _ilog.WriteLine("Finished ");
        }

        private void LookupDescendants(int personId)
        {
            //var parentalRelationship =  _fTMMakerRepository.GetRelationships(personId);

            //is this guy part of any parental relationships

            if (!RelationsDictionary.ContainsKey(personId))
                return;

            var relationship = RelationsDictionary[personId];

            var relId = relationship[0];

            var spouseId = relationship[1];

            if (RelList.Contains(relId) || relId == 12619)
                return;

            RelList.Add(relId);

            //ADD SPOUSE
            if (!AddedPersons.ContainsKey(spouseId) && !_exceptions.Contains(spouseId))
            {
                SpouseList.Add(spouseId);

                AddedPersons.Add(spouseId,false);
            }
            

            if (!ChildWithRelationship.ContainsKey(relId))
                return;

            foreach (var child in ChildWithRelationship[relId])
            {
                if (!AddedPersons.ContainsKey(child))
                {
                    AddedPersons.Add(child,false);

                    if (!GroupPersonIds.Contains(child))
                    {
                        LookupDescendants(child);
                    }

                }
            }
             
        }


        private void LookupAncestors(int personId, bool directAncestor =true)
        {
            if (personId == 0)
                return;

            if (!AddedPersons.ContainsKey(personId))
            {
                AddedPersons.Add(personId,directAncestor);
            }
            //child relationship joins a person to a relationship i.e. their parents relationship

            var crs = ChildRelationshipSubsets.Where(w => w.PersonId == personId);

            LookupDescendants(personId);

            foreach (var c in crs)
            {
                if (ChildList.Contains(c.Id))
                    continue;

                ChildList.Add(c.Id);

                var rels = _fTMMakerRepository.GetRelationshipsById(c.RelationshipId);

                foreach (var r in rels)
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                        LookupAncestors(p1.Value, directAncestor);

                    var p2 = r.Person2Id;
                    if (p2 != null)
                        LookupAncestors(p2.Value, directAncestor);
                }
            }
        }


    }
}
