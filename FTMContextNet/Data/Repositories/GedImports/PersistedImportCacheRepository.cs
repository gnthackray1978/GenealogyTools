using System;
using System.Collections.Generic;
using System.Linq;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;

namespace FTMContextNet.Data.Repositories.GedImports;

public class PersistedImportCacheRepository : IPersistedImportCacheRepository
{
    private readonly IPersistedCacheContext _persistedCacheContext;
    private readonly Ilog _iLog;

    public PersistedImportCacheRepository(IPersistedCacheContext persistedCacheContext, Ilog iLog)
    {
        _persistedCacheContext = persistedCacheContext;
        _iLog = iLog;
    }

    public void DeleteImport(int importId)
    {
        _persistedCacheContext.DeleteImports(importId);
    }
    
    public bool ImportExists(string fileName, double fileSize, int userId)
    {
        return _persistedCacheContext.FTMImport.Any(a => a.FileSize == fileSize && a.FileName == fileName && a.UserId == userId);
    }

    public bool ImportExists(int importId)
    {
        return _persistedCacheContext.FTMImport.Any(a => a.Id == importId);
    }

    public int SelectImport(int importId, int userId)
    {
        foreach (var imp in _persistedCacheContext.FTMImport.Where(w => w.UserId == userId))
        {
            imp.Selected = false;
        }

        _persistedCacheContext.FTMImport.First(f => f.Id == importId).Selected = true;

        _persistedCacheContext.SaveChanges();
        
        return 0;
    }
     
    public List<FTMImport> GetImportData()
    {
        return _persistedCacheContext.FTMImport.ToList();
    }

    public ImportData AddImportRecord(string fileName, double fileSize, bool selected, int userId)
    {
        // if there has been a previous import with this filename 
        // we want to overwrite it. 

        var importData = new ImportData() { CurrentId = new List<int>() };

        importData.CurrentId = _persistedCacheContext.FTMImport.Where(w => w.FileName == fileName).Select(s => s.Id).ToList();


        var newId = _persistedCacheContext.FTMImport.Max(m => m.Id) + 1;

        var import = new FTMImport()
        {
            Id = newId,
            FileName = fileName,
            FileSize = fileSize,
            DateImported = DateTime.Today.ToShortDateString() + " " + DateTime.Today.ToShortTimeString(),
            Selected = selected,
            UserId = userId
        };

        _persistedCacheContext.FTMImport.Add(import);

        _persistedCacheContext.SaveChanges();

        importData.NextId = newId;

        return importData;
    }

    public int GetCurrentImportId()
    {
        return _persistedCacheContext.FTMImport.FirstOrDefault(f => f.Selected)?.Id ?? -1;
    }
}