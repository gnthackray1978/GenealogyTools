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
  //  DupeEntry CreateNewDupeEntry(int dupeId, FTMPersonView person, string ident);
    void DeleteDupes(int importId);
    void DeletePersons(int importId);
    void DeleteTreeRecords(int importId);
    void DeleteMarriages(int importId);
    void DeleteTreeGroups(int importId);
    void DeleteRecordMapGroups(int importId);
    void DeleteOrigins(int importId);
    List<string> MakePlaceRecordCache(int importId);
    void CreatePersonOriginEntries(int importId, int userId);
    DuplicateIgnoreList GetIgnoreList();
    List<PersonIdentifier> GetComparisonPersons(int importId);
    void AddDupeEntrys(List<KeyValuePair<int, string>> dupes, int userId);
    int OriginPersonCount();
    Info GetInfo(int userId);

    /// <summary>
    /// Updates treerecords table in cache. 
    /// stores number of people in tree.
    /// tree name etc
    /// </summary>
    void PopulateTreeRecordsFromCache(int importId);
    int InsertTreeGroups(int nextId, string treeGroup, int importId, int userId);
    int InsertTreeRecordMapGroup(int nextId, string treeGroup, string treeName,int importId, int userId);
    void InsertPersons(int importId,int userId, List<Person> persons);
    void InsertMarriages(int importId,int userId,  List<RelationSubSet> marriages);
    Dictionary<string, List<string>> GetGroups(int importId); 
    Dictionary<int, string> GetGroupPerson(int importId);
}