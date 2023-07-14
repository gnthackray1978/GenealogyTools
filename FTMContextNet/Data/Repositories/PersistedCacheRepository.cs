using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FTMContext;
using FTMContextNet.Domain.Collections;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using GoogleMapsHelpers;
using LoggingLib;
using Microsoft.EntityFrameworkCore;
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories
{
    public class PersistedCacheRepository : IPersistedCacheRepository
    {        
        private readonly IPersistedCacheContext _persistedCacheContext;
        private readonly Ilog _iLog;
        
        public PersistedCacheRepository(IPersistedCacheContext persistedCacheContext, Ilog iLog)
        {
            _persistedCacheContext = persistedCacheContext;
            _iLog = iLog;
        }
        
        public List<string> DumpCount()
        {

            List<string> results = new List<string>();

            DumpRecordCount(results, _persistedCacheContext.FTMPersonView, "FTMPersonView");
          //  DumpRecordCount(results, _persistedCacheContext.FTMPlaceCache, "FTMPlaceCache");
            DumpRecordCount(results, _persistedCacheContext.DupeEntries, "DupeEntries");

            return results;
        }

        private void DumpRecordCount<TT>(List<string> results, DbSet<TT> set, string name) where TT : class
        {
            string result = "";

            var count = set.Count();

            if (count > 0)
                result = name + " " + set.Count();

            if (result != "")
                results.Add(result);
        }
        
        #region deletes
    
        public void DeleteDupes(int importId)
        {
            _persistedCacheContext.DeleteDupes(importId);
        }

        public void DeletePersons(int importId)
        {
            _persistedCacheContext.DeletePersons(importId);
        }

        public void DeleteTreeRecords(int importId)
        {
            _persistedCacheContext.DeleteTreeRecords(importId);
        }

        public void DeleteMarriages(int importId)
        {
            _persistedCacheContext.DeleteMarriages(importId);
        }
        
        public void DeleteTreeGroups(int importId)
        {
            _persistedCacheContext.DeleteTreeGroups(importId);
        }

        public void DeleteRecordMapGroups(int importId)
        {
            _persistedCacheContext.DeleteRecordMapGroups(importId);
        }

        public void DeleteOrigins(int importId)
        {
            _persistedCacheContext.DeleteOrigins(importId);
        }

        #endregion

        #region Valid Data
        private static Expression<Func<FTMPersonView, bool>> ValidData(int importId)
        {
            if (importId != 0)
            {
                return w =>
                    !string.IsNullOrEmpty(w.FirstName)
                    && !w.FirstName.ToLower().Contains("group")
                    && !string.IsNullOrEmpty(w.Surname)
                    && !string.IsNullOrEmpty(w.Origin)
                    && w.Origin != "Thackray"
                    && w.ImportId == importId;
            }

            return w =>
                !string.IsNullOrEmpty(w.FirstName)
                && !w.FirstName.ToLower().Contains("group")
                && !string.IsNullOrEmpty(w.Surname)
                && !string.IsNullOrEmpty(w.Origin)
                && w.Origin != "Thackray" ;
        }

        #endregion

        public List<string> MakePlaceRecordCache(int importId)
        {
            var comparisonPersons = this._persistedCacheContext
                .FTMPersonView.Where(w=>!w.LocationsCached && w.BirthLocation!=null && w.ImportId == importId).Select(s=>s.BirthLocation).ToList();

            comparisonPersons.AddRange(this._persistedCacheContext.FTMPersonView.Where(w => !w.LocationsCached 
                && w.AltLocation != null && w.ImportId == importId).Select(s => s.AltLocation).ToList());

          
            return comparisonPersons;
        }

        public void CreatePersonOriginEntries(int importId, int userId)
        {
            DeleteOrigins(userId);

            var recordsToSave = _persistedCacheContext
                .FTMPersonView
                .Where(w=>w.Origin!="" && w.UserId == userId)
                .Select(s=> new FTMPersonOrigin()
                {
                    Id  = s.Id,
                    Origin = s.Surname.ToLower().Contains("group") ? s.Surname : s.Origin,
                    DirectAncestor = s.DirectAncestor,
                    ImportId = s.ImportId
                }).ToList();
 
            _persistedCacheContext.BulkInsertFTMPersonOrigins(1,  userId, recordsToSave.OrderBy(o => o.Origin).ToList());
        
        }

        public DuplicateIgnoreList GetIgnoreList()
        {
            return new DuplicateIgnoreList(this._persistedCacheContext.IgnoreList.ToList());
        }

        public List<PersonIdentifier> GetComparisonPersons(int importId) {

            var comparisonPersons = this._persistedCacheContext.FTMPersonView
                .Where(ValidData(importId))
             .Select(s => PersonIdentifier.Create(s.PersonId,
                 s.BirthFrom, s.BirthTo, s.Origin, s.LinkedLocations, s.Surname, s.FirstName)).ToList();

            return comparisonPersons;
        }

        public List<PersonLocation> GetPersonMapLocations()
        {

            var comparisonPersons = this
                ._persistedCacheContext.FTMPersonView.Where(ValidData(0))
                .Select(s => new PersonLocation()
                {
                    Id = s.PersonId,
                    Location = s.BirthLocation,
                    Lat = s.BirthLat.ToString(),
                    Lng = s.BirthLong.ToString(),
                    AltLocation = s.AltLocation,
                    AltLat = s.AltLat.ToString(),
                    AltLng = s.AltLong.ToString()
                }).ToList();

            return comparisonPersons;
        }
        
        public void AddDupeEntrys(List<KeyValuePair<int, string>> dupes, int userId)
        {
           var dupeId = _persistedCacheContext.DupeEntries.Max(m=>m.Id) + 1;

           foreach (var pair in dupes)
           {

                var personId = pair.Key;
                var ident = pair.Value;
                var fpvPerson = _persistedCacheContext.FTMPersonView.First(f => f.PersonId == personId);

                var dupeEntry = new DupeEntry
                {
                    Id = dupeId,
                    Ident = ident,
                    PersonId = fpvPerson.PersonId,
                    BirthYearFrom = fpvPerson.BirthFrom,
                    BirthYearTo = fpvPerson.BirthTo,
                    Origin = fpvPerson.Origin,
                    Location = Location.FormatPlace(fpvPerson.BirthLocation),
                    ChristianName = fpvPerson.FirstName,
                    Surname = fpvPerson.Surname,
                    UserId = userId,
                    ImportId = fpvPerson.ImportId
                };

                _persistedCacheContext.DupeEntries.Add(dupeEntry);

                dupeId++;
           }

           _persistedCacheContext.SaveChanges();

        }

        public int OriginPersonCount() 
        {
            return _persistedCacheContext.FTMPersonOrigins.Count() + 1;
        }

        public void UpdatePersons(int personId, string lat, string lng, string altLat, string altLng)
        {
            _persistedCacheContext.UpdatePersonLocations(personId,lng,lat,altLng,altLat);
        }
        
        #region debug data

        public Info GetInfo(int userId) {
            
            var pCount = _persistedCacheContext.FTMPersonView.Count();
            var mcount = _persistedCacheContext.FTMMarriages.Count();
            var originsCount =  _persistedCacheContext.FTMPersonOrigins.Count();
            
            return new Info() { 
                MarriagesCount = mcount,
                PersonViewCount = pCount,
                OriginMappingCount = originsCount,
                DupeEntryCount = _persistedCacheContext.DupeEntries.Count(),
                TreeRecordCount = _persistedCacheContext.TreeRecords.Count()
            };
        }

        #endregion
        
        /// <summary>
        /// Updates treerecords table in cache. 
        /// stores number of people in tree.
        /// tree name etc
        /// </summary>
        public void PopulateTreeRecordsFromCache(int importId)
        {
            var treeRecords = new List<TreeRecord>();

            var locationsMapOrigin =_persistedCacheContext.FTMPersonView.Select(s => new { s.Origin, s.LinkedLocations }).ToList();

            foreach (var tree in this.GetRootNameDictionary(importId).Values)
            {
                var treeLocations = locationsMapOrigin.Where(c => c.Origin == tree)
                    .Select(s=>s.LinkedLocations).ToArray();
 
                treeRecords.Add(TreeRecord.CreateFromOrigin(tree,
                    string.Join(",", EnglishHistoricCounties.FromPlaceList(treeLocations)),
                    treeLocations.Length, 0));
                 
            }

            _iLog.WriteLine("Created " + _persistedCacheContext.BulkInsertTreeRecords(treeRecords)+ " tree records");
        }
        
        public int InsertTreeGroups(int nextId, string treeGroup,int importId, int userId)
        {
            return _persistedCacheContext.InsertGroups(nextId, treeGroup, importId, userId);
        }
        
        public int InsertTreeRecordMapGroup(int nextId, string treeGroup, string treeName,int importId, int userId)
        {
            return _persistedCacheContext.InsertRecordMapGroup(nextId, treeGroup, treeName,importId, userId);
        }

        public void InsertPersons(int importId,int userId, List<Person> persons)
        {
            var nextId = _persistedCacheContext.FTMPersonView.Max(m=>m.Id)+1;
            
            var ftmPersons = persons.Select(person => FTMPersonView.Create(person)).ToList();

            _persistedCacheContext.BulkInsertFTMPersonView(nextId, importId,userId, ftmPersons);
        }

        public void InsertMarriages(int importId,int userId, List<RelationSubSet> marriages)
        {
            var nextId = _persistedCacheContext.FTMMarriages.Max(m => m.Id) + 1;
             
            var ftmPersons = marriages.Select(person => FTMMarriage.Create(person)).ToList();

            _persistedCacheContext.BulkInsertMarriages(nextId, importId,userId, ftmPersons);
        }
         
        public Dictionary<string, List<string>> GetGroups(int importId)
        {
            var results = new Dictionary<string, List<string>>();

            var treeIds = GetTreeIds(importId);

            var tp = GetRelationships(importId);

            var nameDict = GetRootNameDictionary(importId);

            var groupNames = GetGroupNamesDictionary(importId);

            foreach (var treeId in treeIds)
            {
                var groupMembers = tp.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();
                 
                results.Add(nameDict[treeId], (from gm in groupMembers where groupNames.ContainsKey(gm) select groupNames[gm]).ToList());
            }

            return results;
        }
        public Dictionary<int, string> GetGroupPerson(int importId)
        {

            var gps = this._persistedCacheContext
                .FTMPersonView
                .Where(x => x.LinkNode && x.ImportId == importId)
                .ToDictionary(s => s.PersonId, x => x.FirstName + " " + x.Surname);


            return gps;
        }

        private Dictionary<int, string> GetRootNameDictionary(int importId)
        {
            var nameDict = this._persistedCacheContext
                .FTMPersonView
                .Where(w => w.RootPerson && w.ImportId == importId)
                .ToDictionary(i => i.Id, i => (i.FirstName + " " + i.Surname).Trim());
            return nameDict;
        }
        private List<int> GetTreeIds(int importId)
        {
            var treeIds = this._persistedCacheContext
                .FTMPersonView
                .Where(w => w.RootPerson && w.ImportId == importId)
                .Select(s => s.Id).ToList();
            return treeIds;
        }
        private Dictionary<int, string> GetGroupNamesDictionary(int importId)
        {
            var groupNames = this._persistedCacheContext
                .FTMPersonView
                .Where(p => p.LinkNode && p.ImportId== importId)
                .ToDictionary(i => i.Id, i => i.FirstName + " " + i.Surname);
            return groupNames;
        }
        private List<RelationSubSet> GetRelationships(int importId)
        {
            var tp = this._persistedCacheContext.FTMMarriages.Where(w=>w.ImportId == importId)
                .Select(s => new RelationSubSet() { Person1Id = s.GroomId, Person2Id = s.BrideId }).ToList();
            return tp;
        }
        

    }
}
