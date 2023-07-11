using System.Collections.Generic;
using FTMContext;
using FTMContextNet.Domain.Collections;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
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
    void CreatePersonOriginEntries(int importId, int userId);
    DuplicateIgnoreList GetIgnoreList();
    List<PersonIdentifier> GetComparisonPersons();


    void AddDupeEntrys(List<KeyValuePair<int, string>> dupes, int userId);
    int OriginPersonCount();
    Info GetInfo(int userId);

    /// <summary>
    /// Updates treerecords table in cache. 
    /// stores number of people in tree.
    /// tree name etc
    /// </summary>
    void PopulateTreeRecordsFromCache();

    int InsertTreeGroups(int nextId, string treeGroup, int userId);
    int InsertTreeRecordMapGroup(int nextId, string treeGroup, string treeName, int userId);
    void InsertPersons(int importId,int userId, List<Person> persons);
    void InsertMarriages(int importId,int userId,  List<RelationSubSet> marriages);
    int GetMyId();
    Dictionary<string, List<string>> GetGroups();
    Dictionary<int, string> GetRootNameDictionary();
    List<int> GetTreeIds();
    Dictionary<int, string> GetGroupNamesDictionary();
    List<RelationSubSet> GetRelationships();
    Dictionary<int, string> GetGroupPerson();
}