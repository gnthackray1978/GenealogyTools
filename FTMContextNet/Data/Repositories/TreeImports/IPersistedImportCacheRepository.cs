using System.Collections.Generic;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.Persistent.Cache;

namespace FTMContextNet.Data.Repositories.GedImports;

public interface IPersistedImportCacheRepository
{
    int GetCurrentImportId();

    ImportData AddImportRecord(string fileName, long fileSize, bool selected, int userId);

    string SelectImport(int importId, int userId);

    bool ImportExists(string fileName, long fileSize, int userId);

    bool ImportExists(int importId);

    void DeleteImport(int importId);

    List<TreeImport> GetImportData();

    string GedFileName();
}