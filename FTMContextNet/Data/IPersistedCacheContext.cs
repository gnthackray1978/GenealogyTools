using System.Collections.Generic;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data;

public interface IPersistedCacheContext
{
    DbSet<FTMPersonOrigin> FTMPersonOrigins { get; set; }
    DbSet<TreeRecord> TreeRecords { get; set; }
    DbSet<TreeGroups> TreeGroups { get; set; }
    DbSet<TreeRecordMapGroup> TreeRecordMapGroup { get; set; }
    DbSet<DupeEntry> DupeEntries { get; set; }
    DbSet<FTMPersonView> FTMPersonView { get; set; }
    DbSet<FTMMarriage> FTMMarriages { get; set; }
    DbSet<FTMImport> FTMImport { get; set; }
    DbSet<IgnoreList> IgnoreList { get; set; }


    int BulkInsertMarriages(int nextId, int importId,int userId, List<FTMMarriage> marriages);
    int BulkInsertFTMPersonView(int nextId, int importId, int userId, List<FTMPersonView> ftmPersonViews);
    int BulkInsertFTMPersonOrigins(int nextId, int userId, List<FTMPersonOrigin> origins);

    int BulkInsertTreeRecords(List<TreeRecord> treeRecords);
    int InsertGroups(int nextId, string groupName,int importId, int userId);
    int InsertRecordMapGroup(int nextId, string groupName, string treeName,int importId,int userId);
    void DeleteOrigins(int importId);
    void DeleteDupes(int importId);
    void DeletePersons(int importId);
    void DeleteTreeRecords(int importId);
    void DeleteMarriages(int importId);
    void DeleteImports(int importId);
    void DeleteTreeGroups(int importId);
    void DeleteRecordMapGroups(int importId);
    void UpdatePersonLocations(int personId, string lng, string lat, string altLng, string altLat);

    int SaveChanges();
}