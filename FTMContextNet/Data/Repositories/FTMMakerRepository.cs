using FTMContextNet.Domain.Entities.Source;
using System.Collections.Generic;
using System.Linq;
using FTMContext;

namespace FTMContextNet.Data.Repositories
{
    public class FTMMakerRepository
    {
        protected FTMakerContext _ftMakerContext;

        public FTMMakerRepository(FTMakerContext ftMakerContext)
        {
            this._ftMakerContext = ftMakerContext;
        }

        public List<Place> GetPlaces() {
           // var sourceDB = FTMakerContext.CreateSourceDB(iMSGConfigHelper);

            var sourcePlaces = this._ftMakerContext.Place.ToList();

            return sourcePlaces;
        }


        public int GetMyId()
        {
            var me = this._ftMakerContext.Person.FirstOrDefault(w => w.FamilyName.Trim() == "Thackray" && w.GivenName.Trim() == "George Nicholas");

            int personId = 0;


            if (me != null)
            {
                personId = me.Id;
            }

            return personId;
        }

        /// <summary>
        /// Returns persons whose family name starts with _. And person who matches my id.
        /// </summary>
        /// <returns></returns>
        public List<Person> GetRootPeople()
        {           
            int personId = GetMyId();

            var rootPeople = this._ftMakerContext.Person.Where(w => w.FamilyName.StartsWith("_") || w.Id == personId);


            return rootPeople.ToList();
        }


       

        public Dictionary<int, int[]> GetRelationships()
        {
            var result = new Dictionary<int, int[]>();
             
            foreach (var relationship in _ftMakerContext.Relationship)
            {
                int[] tp1 = {
                    relationship.Id,
                    relationship.Person2Id.GetValueOrDefault()
                };

                if(!result.ContainsKey(relationship.Person1Id.GetValueOrDefault()))
                    result.Add(relationship.Person1Id.GetValueOrDefault(), tp1);
            }

            foreach (var relationship in _ftMakerContext.Relationship)
            {
                int[] tp2 = {
                    relationship.Id,
                    relationship.Person1Id.GetValueOrDefault()
                };

                if (!result.ContainsKey(relationship.Person2Id.GetValueOrDefault()))
                    result.Add(relationship.Person2Id.GetValueOrDefault(), tp2);
            }

            return result;
        }

        public List<Relationship> GetRelationshipsById(int id)
        {
            var rels = this._ftMakerContext.Relationship.Where(w => w.Id == id).ToList();

            return rels;
        }

        public HashSet<int> GetListOfGroupIds()
        {
            var lst = this._ftMakerContext.Person.Where(p =>
                p != null && !p.FullName.ToLower().Contains("group") && !p.FullName.ToLower().Contains("_")).Select(s=>s.Id);

            var set = new HashSet<int>();

            foreach (var i in lst)
            {
                set.Add(i);
            }

            return set;
        }
        
        public Dictionary<int, HashSet<int>> GetChildrenWithRelationship()
        {
            var result = new Dictionary<int, HashSet<int>>();

            foreach (var cr in this._ftMakerContext.ChildRelationship)
            {
                if (!result.ContainsKey(cr.RelationshipId))
                {
                    result.Add(cr.RelationshipId, new HashSet<int> { cr.PersonId });
                }
                else
                {
                    result[cr.RelationshipId].Add(cr.PersonId);
                }
            }

            return result;
        }
        
        public List<ChildRelationshipSubset> GetChildrenByPersonId()
        {
            var result = new List<ChildRelationshipSubset>();

            result = new List<ChildRelationshipSubset>(_ftMakerContext.ChildRelationship.Select(s => new ChildRelationshipSubset()
            {
                Id = s.Id,
                PersonId = s.PersonId,
                RelationshipId = s.RelationshipId
            }));

            return result;
        }
    }
}
