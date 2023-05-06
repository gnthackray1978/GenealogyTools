using FTMContextNet.Domain.Entities.Source;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FTMContext;
using PlaceLibNet.Model;
using QuickGed;
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories
{
    public class FTMMakerRepository : IFTMMakerRepository
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
        public List<IPerson> GetTreeRootPeople()
        {           
            

            var rootPeople = GetTreeRootPersons();


            return new List<IPerson>();
        }

        /// <summary>
        /// Gets list of tree root person. each root person will have a name like
        /// _12_fred!smith
        /// </summary>
        /// <returns></returns>
        public HashSet<int> GetListOfTreeIds()
        {
            var lst = GetTreeRootPersons().Select(s => s.Id);

            var set = new HashSet<int>();

            foreach (var i in lst)
            {
                set.Add(i);
            }

            return set;
        }

        public Dictionary<int,string> GetTreeRootNameDictionary()
        {
            var nameDictionary = new Dictionary<int, string>(); 

            var lst = GetTreeRootPersons();
            
            foreach (var i in lst)
            {
                nameDictionary.Add(i.Id,i.FullName);
            }
            
            return nameDictionary;
        }

        public Dictionary<int, string> GetTreeGroupNameDictionary()
        {
            var nameDictionary = new Dictionary<int, string>();

          
            var gps = GetGroupPerson();

            foreach (var i in gps)
            {
                nameDictionary.Add(i.Id, i.FullName);
            }

            return nameDictionary;
        }

        private List<Person> GetTreeRootPersons()
        {
            int personId = GetMyId();

            Regex r = new Regex(@"_[1-9]\d*(\.\d+)?_");


            var result = this._ftMakerContext.Person.Where(p =>
                p != null && (!p.FullName.ToLower().Contains("group") 
                    && p.FullName.ToLower().Contains("_") || p.Id == personId));

            List<Person> persons = new List<Person>();

            //this should only be a small number of records so the performance hit
            //ought to not be noticeable
            foreach (var person in result)
            {
                if (r.IsMatch(person.FullName))
                {
                    persons.Add(person);
                }
            }

            return persons;
        }

        //get a list of trees 
        //look them up in the relationship table
        //make a list of groups.

        public List<IPerson> GetGroupPerson()
        {
            var groups = this._ftMakerContext.Person.Where(p =>  p != null && p.FullName.ToLower().Contains("group"));

            return groups.Cast<IPerson>().ToList();
        }



        //
        public Dictionary<string, List<string>> GetGroups()
        {
            
            var results = new Dictionary<string, List<string>>();

            var treeIds = GetListOfTreeIds();


           var tp = _ftMakerContext.Relationship.Select(s => new RelationSubSet() { Person1Id = s.Person1Id, Person2Id = s.Person2Id }).ToList();

           var nameDict = GetTreeRootNameDictionary();

           var groupNames = GetTreeGroupNameDictionary();

           foreach (var treeId in treeIds)
           {
           
               var groupMembers = tp.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();

               var names = IdsToNames(groupMembers, groupNames);

                results.Add(nameDict[treeId], names);
           }

           return results;
        }

        private static List<string> IdsToNames(List<int> groupMembers, Dictionary<int, string> nameDict)
        {
            List<string> names = new List<string>();

            foreach (var gm in groupMembers)
            {
                if (nameDict.ContainsKey(gm))
                {
                    names.Add(nameDict[gm]);
                }
            }

            return names;
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

        public List<IRelationship> GetRelationshipsById(int id)
        {
            var rels = this._ftMakerContext.Relationship.Where(w => w.Id == id).Cast<IRelationship>().ToList();

            return rels;
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

        public List<PersonSubset> GetPersons()
        {
            throw new System.NotImplementedException();
        }

    }
}
