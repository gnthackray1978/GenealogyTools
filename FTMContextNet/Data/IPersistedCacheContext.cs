using System.Collections.Generic;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data;

public interface IPersistedCacheContext
{
    DbSet<FTMPersonOrigin> PersonOrigins { get; set; }
    DbSet<TreeRecord> TreeRecord { get; set; }
    DbSet<TreeGroups> TreeGroups { get; set; }
    DbSet<TreeRecordMapGroup> TreeRecordMapGroup { get; set; }
    DbSet<DupeEntry> DupeEntries { get; set; }
    DbSet<FTMPersonView> FTMPersonView { get; set; }
    DbSet<Relationships> Relationships { get; set; }
    DbSet<TreeImport> TreeImport { get; set; }
    DbSet<IgnoreList> IgnoreList { get; set; }


    int BulkInsertMarriages(int nextId, int importId,int userId, List<Relationships> marriages);
    int BulkInsertFTMPersonView(int nextId, int importId, int userId, List<FTMPersonView> ftmPersonViews);
    int BulkInsertPersonOrigins(int nextId, int userId, List<FTMPersonOrigin> origins);

    int BulkInsertTreeRecord(List<TreeRecord> treeRecords);
    int InsertGroups(int nextId, string groupName,int importId, int userId);
    int InsertRecordMapGroup(int nextId, string groupName, string treeName,int importId,int userId);
    void DeleteOrigins(int importId);
    void DeleteDupes(int importId);
    void DeleteDupes();
    void DeletePersons(int importId);
    void DeleteTreeRecord(int importId);
    void DeleteMarriages(int importId);
    void DeleteImports(int importId);
    void DeleteTreeGroups(int importId);
    void DeleteRecordMapGroups(int importId);
    void UpdatePersonLocations(int personId, string lng, string lat, string altLng, string altLat);

    int SaveChanges();

    bool ImportExistsInPersons(int importId);
}