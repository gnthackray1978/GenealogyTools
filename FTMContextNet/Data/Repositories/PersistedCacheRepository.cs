using System;
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
using QuickGed.Types;

namespace FTMContextNet.Data.Repositories
{
    public class PersistedCacheRepository
    {        
        private readonly PersistedCacheContext _persistedCacheContext;
        private readonly Ilog _iLog;

        private int _saveCounter;
        private int _counter;
        private int _totalRecords;
        private int _nextId;
        private int _nextImportId;

        public PersistedCacheRepository(PersistedCacheContext persistedCacheContext, Ilog iLog)
        {
            _persistedCacheContext = persistedCacheContext;
            _iLog = iLog;
        }


        public void BeginSavePersons(int importId, int totalRecords)
        {
            
            _saveCounter = 0;
            _counter = 0;
            _totalRecords = totalRecords;
            _nextId = 0;
            _nextImportId = importId;
            _iLog.WriteLine(_totalRecords + " person records to process");
        }

        public void BeginSaveMarriages(int importId, int totalRecords)
        {
            _saveCounter = 0;
            _counter = 0;
            _totalRecords = totalRecords;
            _nextId = 0;
            _nextImportId = importId;

            _iLog.WriteLine(_totalRecords + " marriage records to process");
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

        public List<string> GetPlaces()
        {
            var comparisonPersons = this._persistedCacheContext
                .FTMPersonView.Where(w=>!w.LocationsCached).Select(s=>s.BirthLocation).ToList();

            comparisonPersons.AddRange(this._persistedCacheContext
                .FTMPersonView.Where(w => !w.LocationsCached).Select(s => s.AltLocation).ToList());

            return comparisonPersons;
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

        public void SavePersons(PersonSubset personSubset)
        {

            if (_counter == 0)
            {
                _nextId = _persistedCacheContext.FTMPersonView.Count()+1;
            }

            var ftmPersonView = FTMPersonView.Create(_nextId, _nextImportId, personSubset);
            
            _persistedCacheContext.FTMPersonView.Add(ftmPersonView);


            _counter++;
            _nextId++;

            if (_saveCounter == 1000)
            {
                _iLog.ProgressUpdate(_counter, _totalRecords, " Person Records");
                _persistedCacheContext.SaveChanges();
                _saveCounter = 0;
            }

            _saveCounter++;
        }

        public void SaveAll()
        {
            _persistedCacheContext.SaveChanges();
            _iLog.ProgressUpdate(100, 100, " Records");
        }

        public void SaveMarriages(RelationSubSet r)
        {
            if (_counter == 0)
            {
                _nextId = _persistedCacheContext.FTMMarriages.Count() + 1;
            }
 
            _persistedCacheContext.FTMMarriages.Add(FTMMarriage.Create(_nextId, _nextImportId,r));

            _counter++;
            _nextId++;

            if (_saveCounter == 1000)
            {
                _iLog.ProgressUpdate(_counter, _totalRecords, " Marriage Records");
                _persistedCacheContext.SaveChanges();
                _saveCounter = 0;
            }

            _saveCounter++;
        }

        public HashSet<int> GetListOfPersonIds()
        {
            var lst = GetTreeRootPersons().Keys;

            var set = new HashSet<int>();

            foreach (var i in lst)
            {
                set.Add(i);
            }

            return set;
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
        private Dictionary<int, string> GetTreeRootPersons()
        {
            int personId = GetMyId();

            var r = new Regex(@"|[1-9]\d*(\.\d+)?|");
            string keyword = "|";

            var result = this._persistedCacheContext
                .FTMPersonView
                .Where(x => EF.Functions
                    .Like(x.FirstName, $"%{keyword}%") 
                            || EF.Functions.Like(x.Surname, $"%{keyword}%")).ToList();


            foreach (var node in result)
            {
                node.Surname = node.FirstName + " " + node.Surname;
            }

            //this should only be a small number of records so the performance hit
            //ought to not be noticeable

            return result.Where(person => (!person.Surname.ToLower().Contains("group") 
                && r.IsMatch(person.Surname))|| person.PersonId == personId).ToDictionary(s => s.PersonId, v => v.Surname);
        }
        
        public Dictionary<string, List<string>> GetGroups()
        {
            var results = new Dictionary<string, List<string>>();

            var personIds = GetListOfPersonIds();


            var marriages = this._persistedCacheContext.FTMMarriages
                .Select(s => new RelationSubSet() { Person1Id = s.GroomId, Person2Id = s.BrideId }).ToList();

            var nameDict = GetTreeRootNameDictionary();

            var groupNames = GetTreeGroupNameDictionary();

            foreach (var personId in personIds)
            {

                var groupMembers = marriages.Where(t => t.MatchEither(personId)).Select(s => s.GetOtherSide(personId)).Distinct().ToList();

                var names = IdsToNames(groupMembers, groupNames);

                results.Add(nameDict[personId], names);
            }

            return results;
        }
        public Dictionary<int, string> GetTreeRootNameDictionary()
        {
            var nameDictionary = new Dictionary<int, string>();

            var lst = GetTreeRootPersons();

            foreach (var i in lst)
            {
                nameDictionary.Add(i.Key, i.Value);
            }

            return nameDictionary;
        }

        public Dictionary<int, string> GetTreeGroupNameDictionary()
        {
           
            var gps = this._persistedCacheContext.FTMPersonView.Where(w =>
                w.Surname.ToLower().Contains("group") || w.FirstName.ToLower().Contains("group")).ToDictionary(s=>s.PersonId,x=>x.FirstName + " " + x.Surname);

            return gps;
        }

        private static List<string> IdsToNames(List<int> groupMembers, Dictionary<int, string> nameDict)
        {
            return (from gm in groupMembers where nameDict.ContainsKey(gm) select nameDict[gm]).ToList();
        }

        public Dictionary<int, string> GetGroupPerson()
        {
            string keyword = "group";
            var gps = this._persistedCacheContext.FTMPersonView.Where(x => EF.Functions.Like(x.FirstName, $"%{keyword}%") || EF.Functions.Like(x.Surname, $"%{keyword}%")).ToDictionary(s => s.PersonId, x => x.FirstName + " " + x.Surname);


            return gps;
        }
    }
}
