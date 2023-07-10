﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FTMContext;
using FTMContextNet.Domain.Entities.NonPersistent;
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


        public DupeEntry CreateNewDupeEntry(int dupeId, FTMPersonView person, string ident)
        {

            var dupeEntry = new DupeEntry
            {
                Id = dupeId,
                Ident = ident,
                PersonId = person.PersonId,
                BirthYearFrom = person.BirthFrom,
                BirthYearTo = person.BirthTo,
                Origin = person.Origin,
                Location = Location.FormatPlace(person.BirthLocation),
                ChristianName = person.FirstName,
                Surname = person.Surname
            };

            return dupeEntry;
        }

      
        #region deletes
    
        public void DeleteDupes()
        {
            _persistedCacheContext.DeleteDupes();
        }

        public void DeletePersons(int importId)
        {
            _persistedCacheContext.DeletePersons(importId);
        }

        public void DeleteTreeRecords()
        {
            _persistedCacheContext.DeleteTreeRecords();
        }

        public void DeleteMarriages(int importId)
        {
            _persistedCacheContext.DeleteMarriages(importId);
        }



        public void DeleteTreeGroups()
        {
            _persistedCacheContext.DeleteTreeGroups();
        }

        public void DeleteRecordMapGroups()
        {
            _persistedCacheContext.DeleteRecordMapGroups();
        }

        public void DeleteOrigins()
        {
            _persistedCacheContext.DeleteOrigins();
        }

        #endregion

        #region Valid Data
        private static Expression<Func<FTMPersonView, bool>> ValidData()
        {
            return w =>
                    !string.IsNullOrEmpty(w.FirstName)
                    && !w.FirstName.ToLower().Contains("group")
                    && !string.IsNullOrEmpty(w.Surname)
                && !string.IsNullOrEmpty(w.Origin)
                && w.Origin != "Thackray";
        }

        #endregion

        public List<string> MakePlaceRecordCache()
        {
            var comparisonPersons = this._persistedCacheContext
                .FTMPersonView.Where(w=>!w.LocationsCached && w.BirthLocation!=null).Select(s=>s.BirthLocation).ToList();

            comparisonPersons.AddRange(this._persistedCacheContext.FTMPersonView.Where(w => !w.LocationsCached && w.AltLocation != null).Select(s => s.AltLocation).ToList());

          
            return comparisonPersons;
        }

        public void CreatePersonOriginEntries(int importId, int userId)
        {
            DeleteOrigins();

            var recordsToSave = _persistedCacheContext
                .FTMPersonView
                .Where(w=>w.Origin!="")
                .Select(s=> new FTMPersonOrigin()
                {
                    Id  = s.Id,
                    Origin = s.Surname.ToLower().Contains("group") ? s.Surname : s.Origin,
                    DirectAncestor = s.DirectAncestor,
                    ImportId = importId
                }).ToList();
 
            _persistedCacheContext.BulkInsertFTMPersonOrigins(1, importId, userId, recordsToSave.OrderBy(o => o.Origin).ToList());
        
        }

        public List<IgnoreList> GetIgnoreList()
        {
            var ignoreList = this._persistedCacheContext.IgnoreList.ToList();

            return ignoreList;
        }

        public List<PersonDupeSearchSubset> GetComparisonPersons() {

            var comparisonPersons = this._persistedCacheContext.FTMPersonView.Where(ValidData())
             .Select(s => new PersonDupeSearchSubset()
             {
                 Id = s.PersonId,
                 FamilyName = Misc.Misc.MakeKey(s.FirstName),
                 GivenName = Misc.Misc.MakeKey(s.Surname),
                 Fact = PersonDataObj.Create(s.BirthFrom, s.BirthTo, s.Origin, s.LinkedLocations, s.Surname)
             }).ToList();

            return comparisonPersons;
        }

        public List<PersonLocation> GetPersonMapLocations()
        {

            var comparisonPersons = this
                ._persistedCacheContext.FTMPersonView.Where(ValidData())
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
           var dupeId = _persistedCacheContext.DupeEntries.Count() + 1;

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
                    UserId = userId
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
        public void PopulateTreeRecordsFromCache()
        {
            var treeRecords = new List<TreeRecord>();

            var locationsMapOrigin =_persistedCacheContext.FTMPersonView.Select(s => new { s.Origin, s.LinkedLocations }).ToList();

            foreach (var tree in this.GetRootNameDictionary().Values)
            {
                var treeLocations = locationsMapOrigin.Where(c => c.Origin == tree)
                    .Select(s=>s.LinkedLocations).ToArray();
 
                treeRecords.Add(TreeRecord.CreateFromOrigin(tree,
                    string.Join(",", EnglishHistoricCounties.FromPlaceList(treeLocations)),
                    treeLocations.Length, 0));
                 
            }

            _iLog.WriteLine("Created " + _persistedCacheContext.BulkInsertTreeRecords(treeRecords)+ " tree records");
        }
        
        public int InsertTreeGroups(int nextId, string treeGroup, int userId)
        {
            return _persistedCacheContext.InsertGroups(nextId, treeGroup, userId);
        }
        
        public int InsertTreeRecordMapGroup(int nextId, string treeGroup, string treeName, int userId)
        {
            return _persistedCacheContext.InsertRecordMapGroup(nextId, treeGroup, treeName, userId);
        }

        public void InsertPersons(int importId,int userId, List<Person> persons)
        {
            var nextId = _persistedCacheContext.FTMPersonView.Count()+1;
            
            var ftmPersons = persons.Select(person => FTMPersonView.Create(person)).ToList();

            _persistedCacheContext.BulkInsertFTMPersonView(nextId, importId,userId, ftmPersons);
        }

        public void InsertMarriages(int importId,int userId, List<RelationSubSet> marriages)
        {
            var nextId = _persistedCacheContext.FTMMarriages.Count() + 1;
             
            var ftmPersons = marriages.Select(person => FTMMarriage.Create(person)).ToList();

            _persistedCacheContext.BulkInsertMarriages(nextId, importId,userId, ftmPersons);
        }

        public int GetMyId()
        {
            var me = _persistedCacheContext
                .FTMPersonView.FirstOrDefault(w => w.Surname.Trim() == "Thackray" && w.FirstName.Trim() == "George Nicholas");

            int personId = 0;


            if (me != null)
            {
                personId = me.PersonId;
            }

            return personId;
        }
       


        public Dictionary<string, List<string>> GetGroups()
        {
            var results = new Dictionary<string, List<string>>();

            var treeIds = GetTreeIds();

            var tp = GetRelationships();

            var nameDict = GetRootNameDictionary();

            var groupNames = GetGroupNamesDictionary();

            foreach (var treeId in treeIds)
            {
                var groupMembers = tp.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();
                 
                results.Add(nameDict[treeId], (from gm in groupMembers where groupNames.ContainsKey(gm) select groupNames[gm]).ToList());
            }

            return results;
        }

        public Dictionary<int, string> GetRootNameDictionary()
        {
            var nameDict = this._persistedCacheContext
                .FTMPersonView
                .Where(w => w.RootPerson)
                .ToDictionary(i => i.Id, i => (i.FirstName + " " + i.Surname).Trim());
            return nameDict;
        }
        public List<int> GetTreeIds()
        {
            var treeIds = this._persistedCacheContext
                .FTMPersonView
                .Where(w => w.RootPerson)
                .Select(s => s.Id).ToList();
            return treeIds;
        }
        public Dictionary<int, string> GetGroupNamesDictionary()
        {
            var groupNames = this._persistedCacheContext
                .FTMPersonView
                .Where(p => p.LinkNode)
                .ToDictionary(i => i.Id, i => i.FirstName + " " + i.Surname);
            return groupNames;
        }

        public List<RelationSubSet> GetRelationships()
        {
            var tp = this._persistedCacheContext.FTMMarriages
                .Select(s => new RelationSubSet() { Person1Id = s.GroomId, Person2Id = s.BrideId }).ToList();
            return tp;
        }
        
        public Dictionary<int, string> GetGroupPerson()
        {
           
            var gps = this._persistedCacheContext
                .FTMPersonView
                .Where(x=>x.LinkNode)
                .ToDictionary(s => s.PersonId, x => x.FirstName + " " + x.Surname);


            return gps;
        }
    }
}
