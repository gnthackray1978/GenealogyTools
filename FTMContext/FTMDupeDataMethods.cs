using FTMContext;
using FTMContext.Models;
using MyFamily.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FTMContext
{
    public class ProcessDateReturnType
    {
        public string RangeString { get; set; }
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
    }

    public class ProcessLocationReturnType {
        public string LocationString { get; set; }
        public string BirthLocation { get; set; }
        public double BirthLocationLat { get; set; }
        public double BirthLocationLong { get; set; }

        public string AltLocation { get; set; }
        public double AltLocationLat { get; set; }
        public double AltLocationLong { get; set; }
    }

    public class FTMDupeDataMethods
    {
        protected FTMakerContext _context;

        protected FTMakerContext _de_context;

        protected Dictionary<int, string> originDictionary;

        protected Dictionary<int, FactSubset> baptismFactCache;

        protected Dictionary<int, FactSubset> marriageFactCache;

        protected Dictionary<int, PersonSubset> personCache;


        protected Dictionary<int, int> childRelationshipPersonIndex;

        protected Dictionary<int, int> childRelationshipIndex;

        protected List<ChildRelationshipSubset> childRelationshipCache;

     
        protected Dictionary<int, List<int>> personMapRelationshipDictionary;

        protected Dictionary<int, RelationSubSet> relationshipDictionary;

        protected Dictionary<int, FTMPlaceCacheSubset> fTMPlaceCaches;

        private static Date MakeDate(string ftmDate) {

            if (uint.TryParse(ftmDate, out uint marriageDate))
            {
                return Date.CreateInstance(marriageDate);                
            }

            return null;
        }

        public FTMDupeDataMethods() {

            _context = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Software MacKiev\Family Tree Maker\",
                FileName = @"DNA Match File.ftm",
                IsEncrypted = true
            });

            _de_context = new FTMakerContext(new ConfigObj
            {
                Path = @"C:\Users\george\Documents\Repos\FTMCRUD\ftmframework\",
                FileName = @"decrrypted.db",
                IsEncrypted = false
            });

            fTMPlaceCaches = new Dictionary<int, FTMPlaceCacheSubset>();

            foreach (var f in _de_context.FTMPlaceCache.Select(s => new FTMPlaceCacheSubset()
            {
                FTMPlaceId = s.FTMPlaceId,
                Id = s.Id,
                Country = s.Country,
                County = s.County,
                FTMOrginalNameFormatted = s.FTMOrginalNameFormatted,
                location_lat = FTMGeoCoding.GetLocation(s.JSONResult).lat,
                location_long = FTMGeoCoding.GetLocation(s.JSONResult).lng
            })) {
                if (!fTMPlaceCaches.ContainsKey(f.FTMPlaceId))
                    fTMPlaceCaches.Add(f.FTMPlaceId, f);
            };


            baptismFactCache = new Dictionary<int, FactSubset>();

            foreach (var f in _context.Fact.Where(w => w.FactTypeId == 7
                                                 && w.LinkTableId == 5).Select(s => new FactSubset()
                                                 {
                                                     Id = s.Id,
                                                     Date = MakeDate(s.Date),
                                                     LinkId = s.LinkId,
                                                     PlaceId = s.PlaceId,
                                                     Text = s.Text
                                                 })) {

                if (f.Date != null && !baptismFactCache.ContainsKey(f.LinkId))
                    baptismFactCache.Add(f.LinkId, f);
            };


            originDictionary = new Dictionary<int, string>();

            foreach (var f in _context.Fact.Where(w => w.FactTypeId == 14
                                                 && w.LinkTableId == 5).Select(s => new FactSubset()
                                                 {
                                                     Id = s.Id,                                            
                                                     Text = s.Text,
                                                     LinkId = s.LinkId
                                                 }))
            {

                if (!originDictionary.ContainsKey(f.LinkId))
                    originDictionary.Add(f.LinkId, f.Text);
            };



            var relationships = _context.Relationship.Select(s => new RelationSubSet()
            {
                Id = s.Id,
                Person1Id = s.Person1Id,
                Person2Id = s.Person2Id
            }).ToList();

            marriageFactCache = new Dictionary<int, FactSubset>();


            Action<Dictionary<int, FactSubset>, FactSubset, int?> updateDictionary = (marrDict, factsubset, personId) => {
                //if there isn't a marriage in the dictionary then just add it
                if (personId == null) return;

            
                if (!marrDict.ContainsKey(personId.Value))
                {
                    marrDict.Add(personId.Value, factsubset);
                }
                else
                {
                    if (marrDict[personId.Value].Date == null && factsubset.Date!=null)
                    {
                        marrDict[personId.Value] = factsubset;
                    }
                    else
                    {
                        var prevAddedMar = marrDict[personId.Value].Date;

                        if (prevAddedMar.Year != null && prevAddedMar.HasYear())
                        {
                            if (factsubset.Date.Year != null && factsubset.Date.HasYear())
                            {
                                if (factsubset.Date.Year < prevAddedMar.Year)
                                {
                                    marrDict[personId.Value] = factsubset;
                                }
                            }
                        }
                    }
                }
            };

            foreach (var f in _context.Fact.Where(w => w.FactTypeId == 4
                                                 && w.LinkTableId == 7
                                                 && w.Date!=null && w.PlaceId != null).Select(s => new FactSubset()
            {
                Id = s.Id,
                Date = MakeDate(s.Date),
                LinkId = s.LinkId,
                PlaceId = s.PlaceId,
                Text = s.Text
            })) {

                var relation = relationships.FirstOrDefault(r=>r.Id == f.LinkId);

                updateDictionary(marriageFactCache, f, relation.Person1Id);

                updateDictionary(marriageFactCache, f, relation.Person2Id);
            }
             
            personMapRelationshipDictionary = new Dictionary<int, List<int>>();
            relationshipDictionary = new Dictionary<int, RelationSubSet>();

            var relationshipMapPersonDictionary = new Dictionary<int, int>();

            foreach (var relationship in relationships) {
                //if (person.Person1Id == 8290) {
                //    Debug.WriteLine("break");
                //}

                //relationshipMapPersonDictionary.

                if (!relationshipDictionary.ContainsKey(relationship.Id))
                {
                    relationshipDictionary.Add(relationship.Id, relationship);
                }
                else {
                    Debug.Assert(true, "No dupe IDs should exist - but in these tables who really knows");
                }


                if (relationship.Person1Id != null) {
                    if (personMapRelationshipDictionary.ContainsKey(relationship.Person1Id.Value)) {
                        personMapRelationshipDictionary[relationship.Person1Id.Value].Add(relationship.Id);
                    }
                    else {
                        var tpList = new List<int>();
                        tpList.Add(relationship.Id);

                        personMapRelationshipDictionary.Add(relationship.Person1Id.Value, tpList);
                    }
                }
                    
            };
 
            foreach (var person in relationships)
            {
                if (person.Person2Id != null)
                {
                    if (personMapRelationshipDictionary.ContainsKey(person.Person2Id.Value))
                    {
                        personMapRelationshipDictionary[person.Person2Id.Value].Add(person.Id);
                    }
                    else
                    {
                        var tpList = new List<int>();
                        tpList.Add(person.Id);

                        personMapRelationshipDictionary.Add(person.Person2Id.Value, tpList);
                    }
                }
            };



            personCache = new Dictionary<int, PersonSubset>();

            foreach (var p in _context.Person.Select(s => new PersonSubset()
            {
                Id = s.Id,
                Sex = s.Sex,
                BirthDate = s.BirthDate,
                BirthPlaceId = s.BirthPlaceId,
                DeathDate = s.DeathDate,
                DeathPlaceId = s.DeathPlaceId,
                FamilyName = s.FamilyName,
                GivenName = s.GivenName
            })) {
                personCache.Add(p.Id, p);
            };

            childRelationshipPersonIndex = new Dictionary<int, int>();
            childRelationshipIndex = new Dictionary<int, int>();

            childRelationshipCache = new List<ChildRelationshipSubset>(_context.ChildRelationship.Select(s => new ChildRelationshipSubset()
            {
                Id = s.Id,
                PersonId = s.PersonId,
                RelationshipId = s.RelationshipId
            }));

            childRelationshipCache = childRelationshipCache.OrderBy(o => o.RelationshipId).ToList();

            var idx = 0;
            var currentRelationShip = childRelationshipCache[0].RelationshipId;

            childRelationshipCache[0].StartIndex = 0;
            var startIdx = 0;

            while (idx < childRelationshipCache.Count) {
                //if (childRelationshipCache[idx].RelationshipId == 3479) {
                //    Debug.WriteLine("");
                //}
                
                if (currentRelationShip != childRelationshipCache[idx].RelationshipId)
                {
                    startIdx = idx;
                    currentRelationShip = childRelationshipCache[idx].RelationshipId;               
                }

                childRelationshipCache[idx].StartIndex = startIdx;

                if (!childRelationshipPersonIndex.ContainsKey(childRelationshipCache[idx].PersonId)) {
                    childRelationshipPersonIndex.Add(childRelationshipCache[idx].PersonId, idx);
                }

                if (!childRelationshipIndex.ContainsKey(childRelationshipCache[idx].RelationshipId))
                {
                    childRelationshipIndex.Add(childRelationshipCache[idx].RelationshipId, idx);
                }

                idx++;
            }


        }
        public void CheckForChangedLocs()
        {
            int idx = 0;

            foreach (var p in personCache.Values)
            {               
                GetAllLocationsForPerson(p.Id);

                if (idx % 250 == 0) {
                    Console.WriteLine(idx);
                }

                idx++;
            }

            Console.WriteLine("finished");
        }


        public void CheckForChangedDates() {
           

            //missing persons
            //changed persons


            var existingData = _context.Fact.Where(w => w.FactTypeId == 90 && w.LinkTableId == 5)
                .Select(s => new KeyValuePair<int, string>(s.LinkId, s.Text)).ToList();

            List<int> missingPersonIds = new List<int>();
            List<int> updatedPersonIds = new List<int>();

            int peopleCount = personCache.Count();
            int counter = 0;
            int saveCounter = 0;

            Console.WriteLine(peopleCount + " Records ");
 
            foreach (var p in personCache.Values)
            {
                if (!existingData.Select(s => s.Key).Contains(p.Id))
                {
                    missingPersonIds.Add(p.Id);
                }
                else {

                    var processDateReturnType = ProcessDates(p.Id);
                    var currentOriginDates = processDateReturnType.RangeString.Trim();

                    var existingPair = existingData.FirstOrDefault(f => f.Key == p.Id);
                    var existingDateRange = "";

                    var parts = Regex.Split(existingPair.Value, @"\|\|");

                    if (parts.Length > 0) {
                        existingDateRange = parts[0].Trim();
                    }

                    if (existingDateRange != currentOriginDates) {
                        updatedPersonIds.Add(p.Id);
                    }

                }                
            }

        }

        /// <summary>
        /// Look through birth dates marriage dates and child birth dates to 
        /// generate a range of birth dates
        /// Create list of counties associated with person
        /// assign dates and locations to fact
        /// </summary>
        public void Run() {

           
            string originString = "";
 
            int peopleCount = personCache.Count();
            int counter = 0;
            int saveCounter = 0;

            Console.WriteLine(peopleCount + " Records ");

            int idCounter = 1;

            foreach (var p in personCache.Values)
            {
                if (p.Id == 1924)
                {
                    Debug.WriteLine("");
                }


                var processDateReturnType = ProcessDates(p.Id);

                var processLocationReturnType = GetAllLocationsForPerson(p.Id);

                originString = processDateReturnType.RangeString;

                originString += " || " + processLocationReturnType.LocationString;

                //if (originString.Trim() != "||")
                //    FTMTools.SaveFact(_context, 90, originString, p.Id);

                originDictionary.TryGetValue(p.Id, out string origin);

                var fTMPersonView = new FTMPersonView() {
                    Id = idCounter,
                    PersonId = p.Id,
                    BirthFrom = processDateReturnType.YearFrom,
                    BirthTo = processDateReturnType.YearTo,

                    AltLat = processLocationReturnType.AltLocationLat,
                    AltLocation = processLocationReturnType.AltLocation,
                    AltLocationDesc = "n/a",
                    AltLong = processLocationReturnType.AltLocationLong,
                    BirthLocation = processLocationReturnType.BirthLocation,
                    BirthLat = processLocationReturnType.BirthLocationLat,
                    BirthLong = processLocationReturnType.BirthLocationLong,
                    FirstName = p.GivenName,
                    Surname = p.FamilyName,
                    Origin = origin
                };

                _de_context.FTMPersonView.Add(fTMPersonView);
               

                counter++;
                idCounter++;

                if (saveCounter == 1000) {
                    Console.WriteLine(counter + " of " + peopleCount);
                    Console.WriteLine("Saving: ");
                    // _context.SaveChanges();
                    _de_context.SaveChanges();
                    saveCounter = 0;

                }
                saveCounter++;
            }

            _context.SaveChanges();
        }

        #region age related methods

        public DupeAgeInfo GetBirthDate(int personId)
        {
            List<DupeAgeInfo> list = new List<DupeAgeInfo>();
                  
            if (personCache.TryGetValue(personId, out PersonSubset person))
            {
                if (uint.TryParse(person.BirthDate, out uint birthDate))
                {
                    var tp = Date.CreateInstance(birthDate);

                    if (FactSubset.ValidYear(tp))
                    {
                        list.Add(new DupeAgeInfo()
                        {
                            PlaceID = person.BirthPlaceId,
                            Type = DupeAgeInfoTypes.BirthBap,
                            Year = tp.Year.Value
                        });
                    }
                }

                //get baptisms from fact table
               // var baptisms = baptismFactCache.Where(w => w.LinkId == personId);//fact type 7

                if (baptismFactCache.TryGetValue(personId, out FactSubset bap))
                {                                         
                    if (FactSubset.ValidYear(bap.Date))
                    {
                        list.Add(new DupeAgeInfo()
                        {
                            PlaceID = person.BirthPlaceId,
                            Type = DupeAgeInfoTypes.BirthBap,
                            Year = bap.Date.Year.Value
                        });
                    }                    
                }
            }

            if (list.Count > 0)
            {
                return list.OrderBy(o => o.Year).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public DupeAgeInfo GetFirstChildBirth(int personId)
        {
            List<DupeAgeInfo> list = new List<DupeAgeInfo>();

            //var parentalRelationship = relationshipCache.Where(w => w.Person1Id == personId
            //                            || w.Person2Id == personId).ToList();

            if (!personMapRelationshipDictionary.ContainsKey(personId)) return null;

            //is this guy part of any parental relationships
            foreach (var relationshipId in personMapRelationshipDictionary[personId])
            {

              //  var relationshipId = pr;

                //    var otherChildren = childRelationshipCache.Where(w => w.RelationshipId == relationshipId).ToList();
                List<int> children = new List<int>();

                if (childRelationshipIndex.TryGetValue(relationshipId, out int index))
                {

                    while (childRelationshipCache[index].RelationshipId == relationshipId)
                    {
                        children.Add(childRelationshipCache[index].PersonId);
                        index++;
                    }

                }


                foreach (var childId in children)
                {
                    DupeAgeInfo childBirth = null;

                    //var person = personCache.FirstOrDefault(w => w.Id == child.PersonId);

                    if (personCache.TryGetValue(childId, out PersonSubset person))
                    {
                        if (uint.TryParse(person.BirthDate, out uint birthDate))
                        {
                            var tp = Date.CreateInstance(birthDate);

                            if (FactSubset.ValidYear(tp))
                            {
                                childBirth = new DupeAgeInfo()
                                {
                                    PlaceID = person.BirthPlaceId,
                                    Type = DupeAgeInfoTypes.FirstChildBirthBap,
                                    Year = tp.Year.Value
                                };
                            }
                        }
                    }


                    if (childBirth == null)
                    {
                        //get baptisms from fact table
                        //var baptisms = baptismFactCache.Where(w => w.LinkId == personId);// && w.FactTypeId == 7);

                        if (baptismFactCache.TryGetValue(childId, out FactSubset bap))
                        {                          
                            if (FactSubset.ValidYear(bap.Date))
                            {
                                    childBirth = new DupeAgeInfo()
                                    {
                                        PlaceID = person.BirthPlaceId,
                                        Type = DupeAgeInfoTypes.FirstChildBirthBap,
                                        Year = bap.Date.Year.Value
                                    };

                                    break;
                            }
                   
                        }

                    }

                    if (childBirth != null)
                    {
                        list.Add(childBirth);
                    }
                }

            }


            //sort out children

            if (list.Count > 0)
            {
                return list.OrderBy(o => o.Year).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public DupeAgeInfo GetDeathDate(int personId)
        {
            List<DupeAgeInfo> list = new List<DupeAgeInfo>();

          
            if (personCache.TryGetValue(personId, out PersonSubset person))
            {
                if (uint.TryParse(person.DeathDate, out uint deathDate))
                {
                    var tp = Date.CreateInstance(deathDate);

                    if (FactSubset.ValidYear(tp))
                    {
                        list.Add(new DupeAgeInfo()
                        {
                            PlaceID = person.DeathPlaceId,
                            Type = DupeAgeInfoTypes.Death,
                            Year = tp.Year.Value
                        });
                    }
                }


            }


            if (list.Count > 0)
            {
                return list.OrderBy(o => o.Year).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public DupeAgeInfo GetMarriageDate(int personId)
        {
            List<DupeAgeInfo> list = new List<DupeAgeInfo>();
                     
            if (marriageFactCache.TryGetValue(personId, out FactSubset marriage))
            {
                if (FactSubset.ValidYear(marriage.Date))
                {
                    list.Add(new DupeAgeInfo()
                    {
                        PlaceID = marriage.PlaceId,
                        Type = DupeAgeInfoTypes.Wedding,
                        Year = marriage.Date.Year.Value
                    });
                }
            }
          

            if (list.Count > 0)
            {
                return list.OrderBy(o => o.Year).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        #endregion

        public ProcessDateReturnType ProcessDates(int personId)
        {
            var processDateReturnType = new ProcessDateReturnType();

            var bd = GetBirthDate(personId);

            if (bd != null)
            {
                processDateReturnType.YearFrom = bd.Year;
                processDateReturnType.YearTo = bd.Year;
                processDateReturnType.RangeString = bd.Year.ToString() + " " + bd.Year.ToString();

                return processDateReturnType;
            }

            var md = GetMarriageDate(personId);

            if (md != null)
            {
                processDateReturnType.YearFrom = (md.Year - 50);
                processDateReturnType.YearTo = (md.Year - 18);
                processDateReturnType.RangeString = (md.Year - 50).ToString() + " " + (md.Year - 18).ToString();

                return processDateReturnType;
                 
            }

            var fcb = GetFirstChildBirth(personId);

            if (fcb != null)
            {
                processDateReturnType.YearFrom = (fcb.Year - 50);
                processDateReturnType.YearTo = (fcb.Year - 18);
                processDateReturnType.RangeString = (fcb.Year - 50).ToString() + " " + (fcb.Year - 18).ToString();

                return processDateReturnType;
                 
            }


            var dd = GetDeathDate(personId);

            if (dd != null)
            {
                processDateReturnType.YearFrom = (dd.Year - 75);
                processDateReturnType.YearTo = dd.Year;
                processDateReturnType.RangeString = (dd.Year - 75).ToString() + " " + dd.Year.ToString();

                return processDateReturnType;
                 
            }

            return processDateReturnType;
        }
         
        public ProcessLocationReturnType GetAllLocationsForPerson(int personId)
        {
            var returnType = new ProcessLocationReturnType();

            List<DupeLocInfo> list = new List<DupeLocInfo>();

            list.AddRange(GetPersonsLocDetails(personId, false, false, false));

            list.AddRange(GetSiblings(personId));

            list.AddRange(GetParents(personId));

            List<string> countries = new List<string>();
            List<string> counties = new List<string>();

            List<FTMPlaceCacheSubset> foundLocations = new List<FTMPlaceCacheSubset>();

            foreach (var place in list) {
                //var cachedPlace = fTMPlaceCaches.FirstOrDefault(fpc => fpc.FTMPlaceId == place.PlaceID);

                if (fTMPlaceCaches.TryGetValue(place.PlaceID, out FTMPlaceCacheSubset cachedPlace)) {

                    if (place.Type == DupeLocInfoTypes.BirthBapLoc)
                    {
                        returnType.BirthLocation = cachedPlace.FTMOrginalNameFormatted;
                        returnType.BirthLocationLat = cachedPlace.location_lat;
                        returnType.BirthLocationLong = cachedPlace.location_long;
                    }
                    else
                    {
                        if (cachedPlace.FTMOrginalNameFormatted != "")
                        {
                            cachedPlace.DupeLocInfoType = place.Type;

                            foundLocations.Add(cachedPlace);
                        }
                    }
                    
                    if (!counties.Contains(cachedPlace.County))
                        counties.Add(cachedPlace.County);

                    if (!countries.Contains(cachedPlace.Country))
                        countries.Add(cachedPlace.Country);
                }
            }

            if (foundLocations.Count > 0)
            {
                var location = foundLocations.OrderBy(o => (int)(o.DupeLocInfoType)).First();

                returnType.AltLocation = location.FTMOrginalNameFormatted;
                returnType.AltLocationLat = location.location_lat;
                returnType.AltLocationLong = location.location_long;

            }



            string countylist = "";

            foreach (var c in counties.Where(c => c != ""))
            {
                countylist += c + ",";
            }

            returnType.LocationString = countylist.TrimEnd(',');

            return returnType; 
        }



        public List<DupeLocInfo> GetMarriageLocations(int personId, bool isSibling, bool isFather, bool isMother)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            DupeLocInfoTypes dupeLocInfoTypes = DupeLocInfoTypes.MarriageLoc;

            if (isSibling) dupeLocInfoTypes = DupeLocInfoTypes.SiblingMarriageLoc;
            if (isFather) dupeLocInfoTypes = DupeLocInfoTypes.FatherMarriageLoc;
            if (isMother) dupeLocInfoTypes = DupeLocInfoTypes.MotherMarriageLoc;

            if (marriageFactCache.TryGetValue(personId, out FactSubset marriage))
            {
                int? marriageYear = null;

                if (FactSubset.ValidYear(marriage.Date))
                {
                    //list.Add(new DupeLocInfo()
                    //{
                    //    PlaceID = marriage.PlaceId.Value,
                    //    Type = dupeLocInfoTypes,
                    //    Year = tp.Year.Value
                    //});

                    marriageYear = marriage.Date.Year.Value;
                }
               

                if (marriage.PlaceId.HasValue) {
                    list.Add(new DupeLocInfo()
                    {
                        PlaceID = marriage.PlaceId.Value,
                        Type = dupeLocInfoTypes,
                        Year = marriageYear
                    });
                }
            }
            


            return list;
        }

        public List<DupeLocInfo> GetBirthLocations(int personId, bool isSibling, bool isFather, bool isMother)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            DupeLocInfoTypes dupeLocInfoTypes = DupeLocInfoTypes.BirthBapLoc;

            if (isSibling) dupeLocInfoTypes = DupeLocInfoTypes.SiblingBirthBapLoc;
            if (isFather) dupeLocInfoTypes = DupeLocInfoTypes.FatherBirthBapLoc;
            if (isMother) dupeLocInfoTypes = DupeLocInfoTypes.MotherBapLoc;

        
            if (personCache.TryGetValue(personId, out PersonSubset person))
            {
                int? birthYear = null;

                if (uint.TryParse(person.BirthDate, out uint birthDate))
                {
                    var tp = Date.CreateInstance(birthDate);

                    if (FactSubset.ValidYear(tp))
                    {
                        birthYear = tp.Year.Value;
                    }
                }

                if (person.BirthPlaceId != null && person.BirthPlaceId.HasValue)
                {
                    list.Add(new DupeLocInfo()
                    {
                        PlaceID = person.BirthPlaceId.Value,
                        Type = dupeLocInfoTypes,
                        Year = birthYear
                    });
                }

                //get baptisms from fact table
               
                if (baptismFactCache.TryGetValue(personId, out FactSubset bap))
                {
                    int? bapYear = null;

                    if (FactSubset.ValidYear(bap.Date))
                    {
                        bapYear = bap.Date.Year.Value;
                    }                    

                    if (person.BirthPlaceId != null && person.BirthPlaceId.HasValue)
                    {
                        list.Add(new DupeLocInfo()
                        {
                            PlaceID = person.BirthPlaceId.Value,
                            Type = DupeLocInfoTypes.BirthBapLoc,
                            Year = bapYear
                        });
                    }
                }
            }

            return list;
        }

        public List<DupeLocInfo> GetDeathLocations(int personId, bool isSibling,
            bool isFather, bool isMother)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

           
            DupeLocInfoTypes dupeLocInfoTypes = DupeLocInfoTypes.DeathLoc;

            if (isSibling) dupeLocInfoTypes = DupeLocInfoTypes.SiblingDeathLoc;
            if (isFather) dupeLocInfoTypes = DupeLocInfoTypes.FatherDeathLoc;
            if (isMother) dupeLocInfoTypes = DupeLocInfoTypes.MotherDeathLoc;



            if (personCache.TryGetValue(personId, out PersonSubset person))
            {
                int? deathYear = null;

                if (uint.TryParse(person.DeathDate, out uint deathDate))
                {
                    var tp = Date.CreateInstance(deathDate);

                    if (FactSubset.ValidYear(tp))
                    {
                        deathYear = tp.Year.Value;
                    }
                }

                if (person.DeathPlaceId.HasValue)
                {
                    list.Add(new DupeLocInfo()
                    {
                        PlaceID = person.DeathPlaceId.Value,
                        Type = dupeLocInfoTypes,
                        Year = deathYear
                    });
                }
            }

            return list;
        }

        public List<DupeLocInfo> GetChildBirthLocations(int personId)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();
            //get parents relationship           
            //var parentalRelationship = relationshipCache.Where(w => w.Person1Id == personId
            //                            || w.Person2Id == personId).ToList();

            //is this guy part of any parental relationships
            if (!personMapRelationshipDictionary.ContainsKey(personId)) return list;

            foreach (var relationshipId in personMapRelationshipDictionary[personId])
            {
                //RelationshipId

                List<int> children = new List<int>();

                if (childRelationshipIndex.TryGetValue(relationshipId, out int index)) {
               
                    while (index < childRelationshipCache.Count() 
                        && childRelationshipCache[index].RelationshipId == relationshipId) {                        
                        children.Add(childRelationshipCache[index].PersonId);
                        index++;
                    }

                }
 

                foreach (var childId in children)
                {                   
                   
                    int? year = null;

                    if (personCache.TryGetValue(childId, out PersonSubset person))
                    {
                        if (uint.TryParse(person.BirthDate, out uint birthDate))
                        {
                            var tp = Date.CreateInstance(birthDate);
                            year = (tp.Year != null && tp.HasYear()) ? tp.Year : 0;
                        }

                        if (person.BirthPlaceId != null && person.BirthPlaceId.Value != 0)
                        {
                            list.Add(new DupeLocInfo()
                            {
                                PlaceID = person.BirthPlaceId.Value,
                                Type = DupeLocInfoTypes.ChildBirthLoc,
                                Year = year
                            });
                        }
                    }
                     
                    //get baptisms from fact table
                    
                    if (baptismFactCache.TryGetValue(childId, out FactSubset bap))
                    {
                        year = null;

                       
                        if (FactSubset.ValidYear(bap.Date)) {
                            year = bap.Date.Year;
                        }

                        if (bap.PlaceId != null && bap.PlaceId.Value!=0)
                        {
                            list.Add(new DupeLocInfo()
                            {
                                PlaceID = bap.PlaceId.Value,
                                Type = DupeLocInfoTypes.ChildBirthLoc,
                                Year = year
                            }); 
                        }

                    }

                    
                }

            }


            return list;
        }
        
        public List<DupeLocInfo> GetPersonsLocDetails(int personId
            , bool isSibling, bool isFather, bool isMother) {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            list.AddRange(GetMarriageLocations(personId, isSibling, isFather, isMother));

            list.AddRange(GetBirthLocations(personId, isSibling, isFather, isMother));

            list.AddRange(GetDeathLocations(personId, isSibling, isFather, isMother));

            list.AddRange(GetChildBirthLocations(personId));
            
            return list;
        }

        public List<DupeLocInfo> GetSiblings(int personId) {

            List<int> siblingList = new List<int>();

            if (childRelationshipPersonIndex.TryGetValue(personId, out int index)) {
                var startIdx = childRelationshipCache[index].StartIndex;
                var relationshipId = childRelationshipCache[startIdx].RelationshipId;
                var currentRelationshipId = childRelationshipCache[startIdx].RelationshipId;

                while (relationshipId == currentRelationshipId && startIdx < childRelationshipCache.Count) {

                    currentRelationshipId = childRelationshipCache[startIdx].RelationshipId;

                    siblingList.Add(childRelationshipCache[startIdx].PersonId);
                    startIdx++;
                }
            }

            List<DupeLocInfo> list = new List<DupeLocInfo>();

            foreach (var sibling in siblingList)
            {
                if (sibling != personId)
                    list.AddRange(GetPersonsLocDetails(sibling, true, false, false));
            }


            //var relationship = childRelationshipCache
            //    .FirstOrDefault(p => p.PersonId == personId);

            //List<DupeLocInfo> list = new List<DupeLocInfo>();

            //if (relationship != null)
            //{               
            //    var siblings = childRelationshipCache
            //        .Where(p => p.RelationshipId == relationship.RelationshipId);

            //    foreach(var sibling in siblings)
            //    {
            //        if(sibling.PersonId != personId)
            //            list.AddRange(GetPersonsLocDetails(sibling.PersonId,true,false,false));
            //    }
            //}

            return list;
        }

        public List<DupeLocInfo> GetParents(int personId)
        {
            //child relationship joins a person to a relationship i.e. their parents relationship
         //   var crs = childRelationshipCache.Where(w => w.PersonId == personId).ToList();
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            if (childRelationshipPersonIndex.TryGetValue(personId, out int index)) {               
                //the RelationshipId should ALWAYS be in this dictionary but but but 
                if (relationshipDictionary.TryGetValue(childRelationshipCache[index].RelationshipId, out RelationSubSet r))
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                    {

                        if (personCache.TryGetValue(p1.Value, out PersonSubset person1))
                            list.AddRange(GetPersonsLocDetails(r.Person1Id.Value, false, (person1.Sex == "0"), (person1.Sex == "1")));
                    }

                    var p2 = r.Person2Id;
                    if (p2 != null)
                    {
                        //var person2 = personCache.FirstOrDefault(p => p.Id == p2);

                        if (personCache.TryGetValue(p2.Value, out PersonSubset person2))
                            list.AddRange(GetPersonsLocDetails(r.Person2Id.Value, false, (person2.Sex == "0"), (person2.Sex == "1")));
                    }
                }

            }


           

            //foreach (var c in crs)
          //  {
             
          //  }

            return list;
        }

    }
}
