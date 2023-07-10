using System.Collections.Generic;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.Persistent.Cache;

namespace FTMContextNet.Data.Repositories.GedImports;

public interface IPersistedImportCacheRepository
{
    int GetCurrentImportId();

    ImportData AddImportRecord(string fileName, double fileSize, bool selected, int userId);

    int SelectImport(int importId, int userId);

    bool ImportExists(string fileName, double fileSize, int userId);

    bool ImportExists(int importId);

    void DeleteImport(int importId);

    List<FTMImport> GetImportData();
}