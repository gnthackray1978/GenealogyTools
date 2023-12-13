using System.Collections.Generic;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using Microsoft.EntityFrameworkCore;
using PlaceLibNet.Domain.Entities;

namespace FTMContextNet.Data;

public interface IPersistedCacheContext
{
    DbSet<PersonOrigins> PersonOrigins { get; set; }
    DbSet<TreeRecord> TreeRecord { get; set; }
    DbSet<TreeGroups> TreeGroups { get; set; }
    DbSet<TreeRecordMapGroup> TreeRecordMapGroup { get; set; }
    DbSet<DupeEntry> DupeEntries { get; set; }
    DbSet<FTMPersonView> FTMPersonView { get; set; }
    DbSet<Relationships> Relationships { get; set; }
    DbSet<TreeImport> TreeImport { get; set; }
    DbSet<IgnoreList> IgnoreList { get; set; }


    int BulkInsertMarriages( int importId,int userId, List<Relationships> marriages);
    int BulkInsertFTMPersonView( int importId, int userId, List<FTMPersonView> ftmPersonViews);
    int BulkInsertPersonOrigins(int userId, List<PersonOrigins> origins);

    int BulkInsertTreeRecord(int userId, List<TreeRecord> treeRecords);
    int InsertGroups(int id, string groupName,int importId, int userId);
    int InsertRecordMapGroup(string groupName, string treeName,int importId,int userId);
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

    void BulkUpdatePersonLocations(List<PlaceLocationDto> dataset);

    int SaveChanges();

    bool ImportExistsInPersons(int importId);
}