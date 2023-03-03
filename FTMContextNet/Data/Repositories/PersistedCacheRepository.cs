using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FTM.Dates;
using FTMContext;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Locations;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using FTMContextNet.Domain.Entities.Source;
using FTMContextNet.Domain.Transient;
using GoogleMapsHelpers;
using LoggingLib;
using Microsoft.EntityFrameworkCore;

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

        public PersistedCacheRepository(PersistedCacheContext persistedCacheContext, Ilog iLog)
        {
            _persistedCacheContext = persistedCacheContext;
            _iLog = iLog;
        }


        public void BeginSavePersons(int totalRecords)
        {
            
            _saveCounter = 0;
            _counter = 0;
            _totalRecords = totalRecords;
            _nextId = 0;

            _iLog.WriteLine(_totalRecords + " person records to process");
        }

        public void BeginSaveMarriages(int totalRecords)
        {
            _saveCounter = 0;
            _counter = 0;
            _totalRecords = totalRecords;
            _nextId = 0;

            _iLog.WriteLine(_totalRecords + " marriage records to process");
        }

        public List<string> DumpCount()
        {

            List<string> results = new List<string>();

            DumpRecordCount(results, _persistedCacheContext.FTMPersonView, "FTMPersonView");
            DumpRecordCount(results, _persistedCacheContext.FTMPlaceCache, "FTMPlaceCache");
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

        public void SetPlaceGeoData(int placeId, string results)
        {
            try
            {
                _persistedCacheContext.UpdateFTMPlaceCache(placeId,results);
            
                Debug.WriteLine("ID : " + placeId);
            }
            catch (Exception e)
            {
                Debug.WriteLine("failed: " + e.Message);
            }

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
                Location = GoogleMapsHelpers.Location.FormatPlace(person.BirthLocation),
                ChristianName = person.FirstName,
                Surname = person.Surname
            };

            return dupeEntry;
        }

      
        #region deletes
        public void DeleteOrigins()
        {
            _persistedCacheContext.DeleteOrigins();

            var cnt = _persistedCacheContext.FTMPersonOrigins.Count();

            _iLog.WriteLine(cnt.ToString());
        }

        public void DeleteDupes()
        {
            _persistedCacheContext.DeleteDupes();
        }

        public void DeletePersons()
        {
            _persistedCacheContext.DeletePersons();
        }

        public void DeleteTreeRecords()
        {
            _persistedCacheContext.DeleteTreeRecords();
        }

        public void DeleteMarriages()
        {
            _persistedCacheContext.DeleteMarriages();
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

        public List<FtmPlaceCache> GetUnsetCountiesAndCountrys() {

            var unsetCountiesCount = _persistedCacheContext.FTMPlaceCache.Where(w => (w.County == "" || w.Country == "") && w.JSONResult != null);


            return unsetCountiesCount.ToList();
        }

        public int GetUnsetUkCountiesCount()
        {
            var places = _persistedCacheContext.FTMPlaceCache.Where(w => w.County == "" || w.Country == "").ToList();

            return places.Count;
        }
        public int GetUnsetJsonResultCount()
        {
            var places = _persistedCacheContext.FTMPlaceCache.Where(w => w.JSONResult == null).ToList();

            return places.Count;
        }

        /// <summary>
        /// sets county and country values in ftmcache. 
        /// </summary>
        public List<ExtendedPlace> GetUnsetUkCounties()
        {
            List<FtmPlaceCache> places = _persistedCacheContext.FTMPlaceCache.Where(w => w.County == "" || w.Country == "").ToList();
            
            var results = new List<ExtendedPlace>();

            foreach (var place in places)
            {
                var locationInfo = Location.GetLocationInfo(place.JSONResult);

                if (!locationInfo.IsValid) 
                    continue;

                place.Country = locationInfo.Country;
                place.County = "";

                results.Add(new ExtendedPlace() {
                    Place = place,
                    LocationInfo = locationInfo
                });
            }

            return results;

 
            //_iLog.WriteLine("FTMPlaceCache has ~" + foreignCounties + " foreign records");

            //_iLog.WriteLine("FTMPlaceCache has ~" + unsetCountiesCount + " unset records");
        }


        public void SavePersons() {
            _persistedCacheContext.SaveChanges();
        }

        private static (List<Place> missingPlaces, List<Place> updatedPlaces) 
            CheckForUpdates(PersistedCacheContext context, List<Place> sourcePlaces, Ilog iLog, bool showInfo = false)
        {
            iLog.WriteLine("Finding missing and updated places");
             
            var cacheDictionary = new Dictionary<int, string>();

            // for performance reasons create a place cache of the existing records
            foreach (var p in context.FTMPlaceCache)
            {
                if (!cacheDictionary.ContainsKey(p.FTMPlaceId))
                    cacheDictionary.Add(p.FTMPlaceId, p.FTMOrginalName);
            }

            List<Place> missingPlaces = new List<Place>();
            List<Place> updatedPlaces = new List<Place>();

            //loops through new records and check if they are in the cache
            //if not then its a missing place so add them to missing place list
            foreach (var p in sourcePlaces)
            {
                if (!cacheDictionary.ContainsKey(p.Id))
                {
                    missingPlaces.Add(p);
                }
                else
                {
                    //ok so the id is in the cache 
                    //is the name the same?
                    if (cacheDictionary[p.Id] != p.Name)
                    {
                        //if not
                        updatedPlaces.Add(p);
                    }
                }
            }

            if (showInfo)
            {
                iLog.WriteLine(updatedPlaces.Count + " updated places ");

                foreach (var m in updatedPlaces)
                {
                    iLog.WriteLine("Missing Place : " + m.Name);
                }

                iLog.WriteLine(missingPlaces.Count + " missing places ");

                foreach (var m in missingPlaces)
                {
                    iLog.WriteLine("Missing Place : " + m.Name);
                }
            }
            return (missingPlaces, updatedPlaces);
        }

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

        /// <summary>
        /// Looks through the cacheData.ftmplacecache table and if location is not in it
        /// then add it.
        /// Also looks to see if place has changed.
        /// The ids in the ftmplacecache.FTMPlaceId and dna_match_file place table should be aligned. If
        /// the place changes then it needs updating.
        /// </summary>
        /// <param name="sourcePlaces">list of all places from the dna_match_file DB</param>
        public void AddMissingPlaces(List<Place> sourcePlaces)
        {
            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(this._persistedCacheContext, sourcePlaces, this._iLog);

            _iLog.WriteLine("Adding " + data.missingPlaces.Count + " missing places");

            if (data.missingPlaces.Count > 0)
            {
                int newId = this._persistedCacheContext.FTMPlaceCache.Count() + 1;

                foreach (var p in data.missingPlaces)
                {
                    this._persistedCacheContext.FTMPlaceCache.Add(new FtmPlaceCache()
                    {
                        Id = newId,
                        FTMOrginalName = p.Name,
                        JSONResult = null,
                        FTMOrginalNameFormatted = GoogleMapsHelpers.Location.FormatPlace(p.Name),
                        FTMPlaceId = p.Id,
                        Searched = false,
                        BadData = false
                    });

                    newId++;
                    this._persistedCacheContext.SaveChanges();
                }
            }

        }

        /// <summary>
        /// finds place ids in the ftmcache and sets them to null when
        /// the place ids have been changed.
        /// </summary>
        public void ResetUpdatedPlaces(List<Place> sourcePlaces)
        {

            (List<Place> missingPlaces, List<Place> updatedPlaces) data = CheckForUpdates(this._persistedCacheContext, sourcePlaces, this._iLog);

            _iLog.WriteLine(data.updatedPlaces.Count + " updated places ");


            if (data.updatedPlaces.Count > 0)
            {
                foreach (var p in data.updatedPlaces)
                {
                    var cachedValue = _persistedCacheContext.FTMPlaceCache.FirstOrDefault(f => f.FTMPlaceId == p.Id);

                    if (cachedValue != null)
                    {
                        cachedValue.FTMOrginalName = p.Name;
                        cachedValue.JSONResult = null;
                        cachedValue.Country = "";
                        cachedValue.County = "";
                        cachedValue.FTMOrginalNameFormatted = Location.FormatPlace(p.Name);
                        cachedValue.Searched = false;
                    }

                    _persistedCacheContext.SaveChanges();
                }
            }

        }



        #region debug data




        public Info GetInfo() {
            
            var pCount = _persistedCacheContext.FTMPersonView.Count();
            var mcount = _persistedCacheContext.FTMMarriages.Count();
            var placeCount = _persistedCacheContext.FTMPlaceCache.Count();
            var originsCount =  _persistedCacheContext.FTMPersonOrigins.Count();

            //badlocations count
            var badLocationCount = _persistedCacheContext.FTMPlaceCache.Count(w => w.BadData == true);

            //not found
            var notFoundCount = _persistedCacheContext.FTMPlaceCache.Count(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]") && w.Searched == true);

            var unsearchedCount = _persistedCacheContext.FTMPlaceCache.Count(w => w.Searched == false);




            return new Info() { 
                BadLocationsCount = badLocationCount,
                MarriagesCount = mcount,
                NotFound = notFoundCount,
                PlacesCount = placeCount,
                PersonViewCount = pCount,
                Unsearched = unsearchedCount,
                OriginMappingCount = originsCount,
                DupeEntryCount = _persistedCacheContext.DupeEntries.Count(),
                TreeRecordCount = _persistedCacheContext.TreeRecords.Count()
            };
        }

        #endregion

        /// <summary>
        /// return list of entries in decrypt place cache that don't haven't been geolocated
        /// </summary>
        /// <returns></returns>
        public List<PlaceLookup> GetUnknownPlaces()
        {
            var places = new List<PlaceLookup>();

            places = _persistedCacheContext.FTMPlaceCache
                .Where(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]") && w.BadData == false)
                .Select(s => new PlaceLookup() { placeid = s.FTMPlaceId, placeformatted = s.FTMOrginalNameFormatted })
                .ToList();

            foreach (var f in places)
            {
                f.placeformatted = f.placeformatted.Replace("//", "").Replace("|", "");
            }

            return places;
        }

        public List<PlaceLookup> GetUnknownPlacesIgnoreSearchedAlready()
        {
            var places = new List<PlaceLookup>();

            places = _persistedCacheContext.FTMPlaceCache.Where(w => (w.JSONResult == null || w.JSONResult == "null" || w.JSONResult == "[]")
                                                      && !w.Searched && !w.BadData)
                .Select(s => new PlaceLookup() { placeid = s.FTMPlaceId, placeformatted = s.FTMOrginalNameFormatted })
                .ToList();

            foreach (var f in places)
            {
                f.placeformatted = f.placeformatted.Replace("//", "").Replace("|", "");
            }

            return places;
        }

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



        public int SaveFtmPersonOrigins(int nextId, Dictionary<int,bool> addedPersons, string origin, string firstName) {

            
            return _persistedCacheContext.BulkInsertFTMPersonOrigins(nextId, addedPersons, origin, firstName);
        }

        public int SaveTreeGroups(int nextId, string treeGroup)
        {
            return _persistedCacheContext.InsertGroups(nextId, treeGroup);
        }

        public int SaveTreeRecordMapGroup(int nextId, string treeGroup, string treeName)
        {
            return _persistedCacheContext.InsertRecordMapGroup(nextId, treeGroup, treeName);
        }

        public void SavePersons(PersonSubset personSubset,
                        ProcessDateReturnType processDateReturn,
                        ProcessLocationReturnType associatedLocationData,
                        List<int> parents, PersonOrigin origin)
        {

            if (_counter == 0)
            {
                _nextId = _persistedCacheContext.FTMPersonView.Count()+1;
            }

            var ftmPersonView = FTMPersonView.Create(_nextId, personSubset.Id, personSubset.Forename,personSubset.Surname,
                processDateReturn, associatedLocationData,parents, origin);
            
            _persistedCacheContext.FTMPersonView.Add(ftmPersonView);


            _counter++;
            _nextId++;

            if (_saveCounter == 1000)
            {
                _iLog.WriteCounter("Saving - " + _counter + " of " + _totalRecords);
                _persistedCacheContext.SaveChanges();
                _saveCounter = 0;
            }

            _saveCounter++;
        }

        public void SaveAll()
        {
            _persistedCacheContext.SaveChanges();
        }

        public void SaveMarriages(RelationSubSet r)
        {
            if (_counter == 0)
            {
                _nextId = _persistedCacheContext.FTMMarriages.Count() + 1;
            }

            int year = 0;

            if (r.Date != null && r.Date.HasYear())
                year = r.Date.Year.GetValueOrDefault();

            var marriage = new FTMMarriage()
            {
                Id = _nextId,
                BrideId = r.Person1Id.GetValueOrDefault(),
                GroomId = r.Person2Id.GetValueOrDefault(),
                MarriageDateStr = year.ToString(),
                MarriageLocationId = r.PlaceId.GetValueOrDefault(),
                MarriageYear = year,
                Notes = r.Text,
                Origin = r.Origin,
                MarriageLocation = r.PlaceName

            };

            _persistedCacheContext.FTMMarriages.Add(marriage);

            _counter++;
            _nextId++;

            if (_saveCounter == 1000)
            {
                _iLog.WriteCounter("Saving - " + _counter + " of " + _totalRecords);
                _persistedCacheContext.SaveChanges();
                _saveCounter = 0;
            }

            _saveCounter++;
        }

      
        
    }
}
