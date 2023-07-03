using System.Collections.Generic;
using FTMContext;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories;

public interface IPersistedCacheRepository
{

    List<string> DumpCount();
    DupeEntry CreateNewDupeEntry(int dupeId, FTMPersonView person, string ident);
    void DeleteDupes();
    void DeletePersons(int importId);
    void DeleteTreeRecords();
    void DeleteMarriages(int importId);

    void DeleteTreeGroups();
    void DeleteRecordMapGroups();
    void DeleteOrigins();
    List<string> MakePlaceRecordCache();
    void CreatePersonOriginEntries(int importId);
    List<IgnoreList> GetIgnoreList();
    List<PersonDupeSearchSubset> GetComparisonPersons();


    void AddDupeEntrys(List<KeyValuePair<int, string>> dupes);
    int OriginPersonCount();
    Info GetInfo();

    /// <summary>
    /// Updates treerecords table in cache. 
    /// stores number of people in tree.
    /// tree name etc
    /// </summary>
    void PopulateTreeRecordsFromCache();

    int SaveTreeGroups(int nextId, string treeGroup);
    int SaveTreeRecordMapGroup(int nextId, string treeGroup, string treeName);
    void SavePersons(int importId, List<Person> persons);
    void SaveMarriages(int importId, List<RelationSubSet> marriages);
    int GetMyId();
    Dictionary<string, List<string>> GetGroups();
    Dictionary<int, string> GetRootNameDictionary();
    List<int> GetTreeIds();
    Dictionary<int, string> GetGroupNamesDictionary();
    List<RelationSubSet> GetRelationships();
    Dictionary<int, string> GetGroupPerson();
}