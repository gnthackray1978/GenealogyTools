using System.Collections.Generic;
using FTMContext;
using FTMContextNet.Domain.Entities.Source;
using PlaceLibNet.Model;
using QuickGed;
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories;

public interface IFTMMakerRepository
{
    List<Place> GetPlaces();
    int GetMyId();

    /// <summary>
    /// Returns persons whose family name starts with _. And person who matches my id.
    /// </summary>
    /// <returns></returns>
    List<IPerson> GetTreeRootPeople();

    /// <summary>
    /// Gets list of tree root person. each root person will have a name like
    /// _12_fred!smith
    /// </summary>
    /// <returns></returns>
    HashSet<int> GetListOfTreeIds();

    Dictionary<int,string> GetTreeRootNameDictionary();
    Dictionary<int, string> GetTreeGroupNameDictionary();
    List<IPerson> GetGroupPerson();
    Dictionary<string, List<string>> GetGroups();
    Dictionary<int, int[]> GetRelationships();
    List<IRelationship> GetRelationshipsById(int id);
    Dictionary<int, HashSet<int>> GetChildrenWithRelationship();
    List<ChildRelationshipSubset> GetChildrenByPersonId();

    List<PersonSubset> GetPersons();
}