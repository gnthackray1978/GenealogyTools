﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FTMContext;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using GoogleMapsHelpers;
using LoggingLib;
using Microsoft.EntityFrameworkCore;
using PlaceLibNet.Domain;
using QuickGed;
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories
{
    public class PersistedCacheRepository
    {        
        private readonly PersistedCacheContext _persistedCacheContext;
        private readonly Ilog _iLog;
        
        public PersistedCacheRepository(PersistedCacheContext persistedCacheContext, Ilog iLog)
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

        public void DeleteImport(int importId)
        {
            _persistedCacheContext.DeleteImports(importId);
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

        public PlaceRecordCollection GetPersonLocations()
        {
            var comparisonPersons = this._persistedCacheContext
                .FTMPersonView.Where(w=>!w.LocationsCached && w.BirthLocation!=null).Select(s=>s.BirthLocation).ToList();

            comparisonPersons.AddRange(this._persistedCacheContext
                .FTMPersonView.Where(w => !w.LocationsCached && w.AltLocation != null).Select(s => s.AltLocation).ToList());

          
            return new PlaceRecordCollection(comparisonPersons);
        }

        public void CreatePersonOriginEntries(int importId)
        {
            DeleteOrigins();

            var recordsToSave = _persistedCacheContext
                .FTMPersonView
                .Select(s=> new FTMPersonOrigin()
                {
                    Id  = s.Id,
                    Origin = s.Surname.ToLower().Contains("group") ? s.Surname : s.Origin,
                    DirectAncestor = s.DirectAncestor,
                    ImportId = importId
                }).ToList();
 
            _persistedCacheContext.BulkInsertFTMPersonOrigins(1,1, recordsToSave.OrderBy(o => o.Origin).ToList());
        
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
        public ImportData AddImportRecord(string fileName, long fileSize)
        {
            // if there has been a previous import with this filename 
            // we want to overwrite it. 

            var importData = new ImportData(){CurrentId = new List<int>()};

            importData.CurrentId = _persistedCacheContext.FTMImport.Where(w => w.FileName == fileName).Select(s=>s.Id).ToList();

           
            var newId = _persistedCacheContext.FTMImport.Max(m=>m.Id) + 1;

            var import = new FTMImport()
            {
                Id = newId, 
                FileName = fileName,
                FileSize = fileSize,
                DateImported = DateTime.Today.ToShortDateString() + " " + DateTime.Today.ToShortTimeString()
            };

            _persistedCacheContext.FTMImport.Add(import);
            
            _persistedCacheContext.SaveChanges();

            importData.NextId = newId;

            return importData;
        }

        public void AddDupeEntrys(List<KeyValuePair<int, string>> dupes)
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
                    Surname = fpvPerson.Surname
                };

                _persistedCacheContext.DupeEntries.Add(dupeEntry);

                dupeId++;
            }

            _persistedCacheContext.SaveChanges();

        }

        public int OriginPersonCount() {
            
            int idx = _persistedCacheContext.FTMPersonOrigins.Count() + 1;


            return idx;
        }


        
        #region debug data




        public Info GetInfo() {
            
            var pCount = _persistedCacheContext.FTMPersonView.Count();
            var mcount = _persistedCacheContext.FTMMarriages.Count();
           // var placeCount = _persistedCacheContext.FTMPlaceCache.Count();
            var originsCount =  _persistedCacheContext.FTMPersonOrigins.Count();

            //badlocations count
        //    var badLocationCount = _persistedCacheContext.FTMPlaceCache.Count(w => w.BadData == true);

            //not found
          //  var notFoundCount = _persistedCacheContext.FTMPlaceCache.Count(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]") && w.Searched == true);

          //  var unsearchedCount = _persistedCacheContext.FTMPlaceCache.Count(w => w.Searched == false);




            return new Info() { 
            //    BadLocationsCount = badLocationCount,
                MarriagesCount = mcount,
              //  NotFound = notFoundCount,
              //  PlacesCount = placeCount,
                PersonViewCount = pCount,
               // Unsearched = unsearchedCount,
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

            

            int idx = _persistedCacheContext.TreeRecords.Count() + 1;

            // var rootPeople = _context.Person.Where(w => w.Surname.StartsWith("_"));

            List<TreeRecord> newRecords = new List<TreeRecord>();

            foreach (var family in _persistedCacheContext.FTMPersonView.ToList().GroupBy(g => g.Origin))
            {

                string familyName = family.First().Origin ?? "Unknown";

                _iLog.WriteCounter("Adding Tree " + familyName + " " + family.Count() + " ancestors");

                List<string> locationList = new List<string>();

                foreach (var child in family)
                {
                    var parts = child.LinkedLocations.Split(',');

                    foreach (var part in parts)
                    {
                        if (!locationList.Contains(part))
                        {
                            if (EnglishHistoricCountyList.Get.Contains(part))
                                locationList.Add(part);
                        }
                    }

                }
                
                string originString = string.Join(",", locationList);
                
                Regex re = new Regex(@"\d+");

                Match m = re.Match(familyName);

                int cmVal = 0;

                if (m.Success)
                {
                    int.TryParse(m.Value, out cmVal);
                }

                re = new Regex(@"[fF]\d+");

                m = re.Match(familyName);

                newRecords.Add(new TreeRecord()
                {
                    Id = idx,
                    PersonCount = family.Count(),
                    Name = familyName,
                    Origin = originString,
                    CM = cmVal,
                    Located = m.Success
                });

                idx++;
            }

            if(newRecords.Count > 0)
                _persistedCacheContext.TreeRecords.AddRange(newRecords);


            _persistedCacheContext.SaveChanges();
        }
        
        public int SaveTreeGroups(int nextId, string treeGroup)
        {
            return _persistedCacheContext.InsertGroups(nextId, treeGroup);
        }

        public int SaveTreeRecordMapGroup(int nextId, string treeGroup, string treeName)
        {
            return _persistedCacheContext.InsertRecordMapGroup(nextId, treeGroup, treeName);
        }

        public void SavePersons(int importId, List<Person> persons)
        {
            var nextId = _persistedCacheContext.FTMPersonView.Count()+1;
            
            var ftmPersons = persons.Select(person => FTMPersonView.Create(person)).ToList();

            _persistedCacheContext.BulkInsertFTMPersonView(nextId, importId, ftmPersons);
        }

       
        public void SaveMarriages(int importId, List<RelationSubSet> marriages)
        {
            var nextId = _persistedCacheContext.FTMMarriages.Count() + 1;
             
            var ftmPersons = marriages.Select(person => FTMMarriage.Create(person)).ToList();

            _persistedCacheContext.BulkInsertMarriages(nextId, importId, ftmPersons);
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

            var treeIds =this._persistedCacheContext.FTMPersonView.Where(w => w.RootPerson).Select(s => s.Id).ToList();

            var tp = this._persistedCacheContext.FTMMarriages
                .Select(s => new RelationSubSet() { Person1Id = s.GroomId, Person2Id = s.BrideId }).ToList();

            var nameDict = this._persistedCacheContext.FTMPersonView.Where(w => w.RootPerson).ToDictionary(i => i.Id, i => i.FirstName + " "+i.Surname);

            var groupNames = this._persistedCacheContext.FTMPersonView
                .Where(p => p.LinkNode)
                .Cast<IPerson>().ToDictionary(i => i.Id, i => i.FullName);

            foreach (var treeId in treeIds)
            {
                var groupMembers = tp.Where(t => t.MatchEither(treeId)).Select(s => s.GetOtherSide(treeId)).Distinct().ToList();
                
                results.Add(nameDict[treeId], (from gm in groupMembers where groupNames.ContainsKey(gm) select groupNames[gm]).ToList());
            }

            return results;
        }
    
        public Dictionary<int, string> GetGroupPerson()
        {
           
            var gps = this._persistedCacheContext.FTMPersonView.Where(x=>x.LinkNode).ToDictionary(s => s.PersonId, x => x.FirstName + " " + x.Surname);


            return gps;
        }
    }
}
