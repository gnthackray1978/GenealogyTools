using System.Collections.Generic;
using FTMContext;
using FTMContextNet.Domain.Collections;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories.TreeAnalysis;

public interface IPersistedCacheRepository
{
    List<string> DumpCount();
    void UpdatePersons(int personId, string lat, string lng, string altLat, string altLng);
    void DeleteDupes(int importId);
    void DeleteDupes();
    void DeletePersons(int importId);
    void DeleteTreeRecord(int importId);
    void DeleteMarriages(int importId);
    void DeleteTreeGroups(int importId);
    void DeleteRecordMapGroups(int importId);
    void DeleteOrigins(int importId);
    bool ImportPresent(int importId);

    List<string> MakePlaceRecordCache(int importId);
    void CreatePersonOriginEntries(int importId, int userId);
    DuplicateIgnoreList GetIgnoreList();
    List<PersonIdentifier> GetComparisonPersons(int importId);
    void AddDupeEntrys(List<KeyValuePair<int, string>> dupes, int userId);
    int OriginPersonCount();
    Info GetInfo(int userId);
    List<PersonLocation> GetPersonMapLocations();

    /// <summary>
    /// Updates treerecords table in cache. 
    /// stores number of people in tree.
    /// tree name etc
    /// </summary>
    void PopulateTreeRecordFromCache(int userId, int importId);
    int InsertTreeGroups(int id, string treeGroup, int importId, int userId);
    int InsertTreeRecordMapGroup(string treeGroup, string treeName, int importId, int userId);
    void InsertPersons(int importId, int userId, List<Person> persons);
    void InsertRelationships(int importId, int userId, List<RelationSubSet> marriages);
    Dictionary<string, List<string>> GetGroups(int importId);
    Dictionary<int, string> GetGroupPerson(int importId);
}