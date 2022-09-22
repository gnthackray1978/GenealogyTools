using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FTM.Dates;
using FTMContext;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using LoggingLib;
using Microsoft.EntityFrameworkCore;

namespace FTMContextNet.Data
{
    public class InMemoryCacheContext
    {
        public Dictionary<int, string> originDictionary;

        public Dictionary<int, FactSubset> baptismFactCache;

        public Dictionary<int, FactSubset> marriageFactCache;

        public Dictionary<int, PersonSubset> personCache;


        public Dictionary<int, int> childRelationshipPersonIndex;

        public Dictionary<int, int> childRelationshipIndex;

        public List<ChildRelationshipSubset> childRelationshipCache;


        public Dictionary<int, List<int>> personMapRelationshipDictionary;

        public Dictionary<int, RelationSubSet> relationshipDictionary;

        public Dictionary<int, FTMPlaceCacheSubset> fTMPlaceCaches;

        public List<KeyValuePair<int, string>> Type90Facts;


        public InMemoryCacheContext(FTMakerContext _sourceContext,
                           DbSet<FtmPlaceCache> fTMPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins)
        {
            
            Type90Facts = _sourceContext.Fact.Where(w => w.FactTypeId == 90 && w.LinkTableId == 5)
                    .Select(s => new KeyValuePair<int, string>(s.LinkId, s.Text)).ToList();


            fTMPlaceCaches = new Dictionary<int, FTMPlaceCacheSubset>();

            foreach (var f in fTMPlaceCache.Select(s => new FTMPlaceCacheSubset()
            {
                FTMPlaceId = s.FTMPlaceId,
                Id = s.Id,
                Country = s.Country,
                County = s.County,
                FTMOrginalNameFormatted = s.FTMOrginalNameFormatted,
                location_lat = GoogleMapsHelpers.Location.GetLocation(s.JSONResult).lat,
                location_long = GoogleMapsHelpers.Location.GetLocation(s.JSONResult).lng
            }))
            {
                if (!fTMPlaceCaches.ContainsKey(f.FTMPlaceId))
                    fTMPlaceCaches.Add(f.FTMPlaceId, f);
            }


            baptismFactCache = new Dictionary<int, FactSubset>();

            foreach (var f in _sourceContext.Fact.Where(w => w.FactTypeId == 7
                                                 && w.LinkTableId == 5).Select(s => new FactSubset()
                                                 {
                                                     Id = s.Id,
                                                     Date = MakeDate(s.Date),
                                                     LinkId = s.LinkId,
                                                     PlaceId = s.PlaceId,
                                                     Text = s.Text
                                                 }))
            {

                if (f.Date != null && !baptismFactCache.ContainsKey(f.LinkId))
                    baptismFactCache.Add(f.LinkId, f);
            }


            originDictionary = new Dictionary<int, string>();

            foreach (var person in ftmPersonOrigins)
            {
                if (!originDictionary.ContainsKey(person.PersonId))
                    originDictionary.Add(person.PersonId, person.Origin);
            }
            //foreach (var f in _sourceContext.Fact.Where(w => w.FactTypeId == 14
            //                                     && w.LinkTableId == 5).Select(s => new FactSubset()
            //                                     {
            //                                         Id = s.Id,
            //                                         Text = s.Text,
            //                                         LinkId = s.LinkId
            //                                     }))
            //{

            //    if (!originDictionary.ContainsKey(f.LinkId))
            //        originDictionary.Add(f.LinkId, f.Text);
            //};



            var relationships = _sourceContext.Relationship.Select(s => new RelationSubSet()
            {
                Id = s.Id,
                Person1Id = s.Person1Id,
                Person2Id = s.Person2Id
            }).ToList();

            marriageFactCache = new Dictionary<int, FactSubset>();


            Action<Dictionary<int, FactSubset>, FactSubset, int?> updateDictionary = (marrDict, factsubset, personId) =>
            {
                //if there isn't a marriage in the dictionary then just add it
                if (personId == null) return;


                if (!marrDict.ContainsKey(personId.Value))
                {
                    marrDict.Add(personId.Value, factsubset);
                }
                else
                {
                    if (marrDict[personId.Value].Date == null && factsubset.Date != null)
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

            foreach (var f in _sourceContext.Fact.Where(w => w.FactTypeId == 4
                                                 && w.LinkTableId == 7
                                                 && w.Date != null && w.PlaceId != null).Select(s => new FactSubset()
                                                 {
                                                     Id = s.Id,
                                                     Date = MakeDate(s.Date),
                                                     LinkId = s.LinkId,
                                                     PlaceId = s.PlaceId,
                                                     Text = s.Text
                                                 }))
            {

                var relation = relationships.FirstOrDefault(r => r.Id == f.LinkId);

                var origin = "";

                if (originDictionary.ContainsKey(relation.Person1Id.GetValueOrDefault()))
                    origin = originDictionary[relation.Person1Id.GetValueOrDefault()];


                relation.Date = f.Date;
                relation.Text = f.Text;
                relation.LinkId = f.LinkId;
                relation.PlaceId = f.PlaceId.GetValueOrDefault();
                relation.Origin = origin;


                updateDictionary(marriageFactCache, f, relation.Person1Id);

                updateDictionary(marriageFactCache, f, relation.Person2Id);
            }

            foreach (var r in relationships)
            {
                var origin = "";

                if (originDictionary.ContainsKey(r.Person1Id.GetValueOrDefault()))
                    origin = originDictionary[r.Person1Id.GetValueOrDefault()];

                if (origin == "")
                {
                    if (originDictionary.ContainsKey(r.Person2Id.GetValueOrDefault()))
                        origin = originDictionary[r.Person2Id.GetValueOrDefault()];
                }

                r.Origin = origin;
                //fTMPlaceCaches

                if (r.PlaceId != null && fTMPlaceCaches.ContainsKey(r.PlaceId.Value))
                {
                    var placeName = fTMPlaceCaches[r.PlaceId.Value];

                    r.PlaceName = placeName.FTMOrginalNameFormatted;
                }
            }


            personMapRelationshipDictionary = new Dictionary<int, List<int>>();
            relationshipDictionary = new Dictionary<int, RelationSubSet>();

            foreach (var relationship in relationships)
            {
                //if (person.Person1Id == 8290) {
                //    Debug.WriteLine("break");
                //}

                //relationshipMapPersonDictionary.

                if (!relationshipDictionary.ContainsKey(relationship.Id))
                {
                    relationshipDictionary.Add(relationship.Id, relationship);
                }
                else
                {
                    Debug.Assert(true, "No dupe IDs should exist - but in these tables who really knows");
                }


                if (relationship.Person1Id != null)
                {
                    if (personMapRelationshipDictionary.ContainsKey(relationship.Person1Id.Value))
                    {
                        personMapRelationshipDictionary[relationship.Person1Id.Value].Add(relationship.Id);
                    }
                    else
                    {
                        var tpList = new List<int> { relationship.Id };

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
                        var tpList = new List<int> { person.Id };

                        personMapRelationshipDictionary.Add(person.Person2Id.Value, tpList);
                    }
                }
            }


            personCache = new Dictionary<int, PersonSubset>();

            foreach (var p in _sourceContext.Person.Select(s => new PersonSubset()
            {
                Id = s.Id,
                Sex = s.Sex,
                BirthDate = s.BirthDate,
                BirthPlaceId = s.BirthPlaceId,
                DeathDate = s.DeathDate,
                DeathPlaceId = s.DeathPlaceId,
                Surname = s.FamilyName,
                Forename = s.GivenName
            }))
            {
                personCache.Add(p.Id, p);
            }

            childRelationshipPersonIndex = new Dictionary<int, int>();
            childRelationshipIndex = new Dictionary<int, int>();

            childRelationshipCache = new List<ChildRelationshipSubset>(_sourceContext.ChildRelationship.Select(s => new ChildRelationshipSubset()
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

            while (idx < childRelationshipCache.Count)
            {
                //if (childRelationshipCache[idx].RelationshipId == 3479) {
                //    Debug.WriteLine("");
                //}

                if (currentRelationShip != childRelationshipCache[idx].RelationshipId)
                {
                    startIdx = idx;
                    currentRelationShip = childRelationshipCache[idx].RelationshipId;
                }

                childRelationshipCache[idx].StartIndex = startIdx;

                if (!childRelationshipPersonIndex.ContainsKey(childRelationshipCache[idx].PersonId))
                {
                    childRelationshipPersonIndex.Add(childRelationshipCache[idx].PersonId, idx);
                }

                if (!childRelationshipIndex.ContainsKey(childRelationshipCache[idx].RelationshipId))
                {
                    childRelationshipIndex.Add(childRelationshipCache[idx].RelationshipId, idx);
                }

                idx++;
            }

            

        }

        public static InMemoryCacheContext Create(FTMakerContext _sourceContext,
                           DbSet<FtmPlaceCache> fTMPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins,
                            Ilog ilog)
        {
            return new InMemoryCacheContext(_sourceContext, fTMPlaceCache, ftmPersonOrigins);
        }

        private static Date MakeDate(string ftmDate)
        {

            if (uint.TryParse(ftmDate, out uint marriageDate))
            {
                return Date.CreateInstance(marriageDate);
            }

            return null;
        }

        #region age related methods

        private DupeAgeInfo GetBirthDate(int personId)
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
                return list.MinBy(o => o.Year);
            }
            else
            {
                return null;
            }
        }

        private DupeAgeInfo GetFirstChildBirth(int personId)
        {
            if (personId == 20905)
            {

                Debug.WriteLine("");
            }

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

                    while (childRelationshipCache.Count > index &&
                           childRelationshipCache[index].RelationshipId == relationshipId)
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
                return list.MinBy(o => o.Year);
            }
            else
            {
                return null;
            }
        }

        private DupeAgeInfo GetDeathDate(int personId)
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
                return list.MinBy(o => o.Year);
            }
            else
            {
                return null;
            }
        }

        private DupeAgeInfo GetMarriageDate(int personId)
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
                return list.MinBy(o => o.Year);
            }
            else
            {
                return null;
            }
        }


        public ProcessDateReturnType GetPersonBirthDateRange(int personId)
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
                processDateReturnType.YearFrom = md.Year - 50;
                processDateReturnType.YearTo = md.Year - 18;
                processDateReturnType.RangeString = (md.Year - 50).ToString() + " " + (md.Year - 18).ToString();

                return processDateReturnType;

            }

            var fcb = GetFirstChildBirth(personId);

            if (fcb != null)
            {
                processDateReturnType.YearFrom = fcb.Year - 50;
                processDateReturnType.YearTo = fcb.Year - 18;
                processDateReturnType.RangeString = (fcb.Year - 50).ToString() + " " + (fcb.Year - 18).ToString();

                return processDateReturnType;

            }


            var dd = GetDeathDate(personId);

            if (dd != null)
            {
                processDateReturnType.YearFrom = dd.Year - 75;
                processDateReturnType.YearTo = dd.Year;
                processDateReturnType.RangeString = (dd.Year - 75).ToString() + " " + dd.Year.ToString();

                return processDateReturnType;

            }

            return processDateReturnType;
        }


        #endregion


        #region dupe stuff

        private List<DupeLocInfo> GetMarriageLocations(int personId, bool isSibling, bool isFather, bool isMother)
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


                if (marriage.PlaceId.HasValue)
                {
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

        private List<DupeLocInfo> GetBirthLocations(int personId, bool isSibling, bool isFather, bool isMother)
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

        private List<DupeLocInfo> GetDeathLocations(int personId, bool isSibling,
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

        private List<DupeLocInfo> GetChildBirthLocations(int personId)
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

                if (childRelationshipIndex.TryGetValue(relationshipId, out int index))
                {

                    while (index < childRelationshipCache.Count()
                        && childRelationshipCache[index].RelationshipId == relationshipId)
                    {
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
                            year = tp.Year != null && tp.HasYear() ? tp.Year : 0;
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


                        if (FactSubset.ValidYear(bap.Date))
                        {
                            year = bap.Date.Year;
                        }

                        if (bap.PlaceId != null && bap.PlaceId.Value != 0)
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
            , bool isSibling, bool isFather, bool isMother)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            list.AddRange(GetMarriageLocations(personId, isSibling, isFather, isMother));

            list.AddRange(GetBirthLocations(personId, isSibling, isFather, isMother));

            list.AddRange(GetDeathLocations(personId, isSibling, isFather, isMother));

            list.AddRange(GetChildBirthLocations(personId));

            return list;
        }

        private List<DupeLocInfo> GetSiblings(int personId)
        {

            List<int> siblingList = new List<int>();

            if (childRelationshipPersonIndex.TryGetValue(personId, out int index))
            {
                var startIdx = childRelationshipCache[index].StartIndex;
                var relationshipId = childRelationshipCache[startIdx].RelationshipId;
                var currentRelationshipId = childRelationshipCache[startIdx].RelationshipId;

                while (relationshipId == currentRelationshipId && startIdx < childRelationshipCache.Count)
                {

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

        /// <summary>
        /// mother is first in the list, father 2nd
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public List<int> GetParentIds(int personId)
        {
            //   var crs = childRelationshipCache.Where(w => w.PersonId == personId).ToList();

            int fatherId = 0;
            int motherId = 0;

            if (childRelationshipPersonIndex.TryGetValue(personId, out int index))
            {
                //the RelationshipId should ALWAYS be in this dictionary but but but 
                if (relationshipDictionary.TryGetValue(childRelationshipCache[index].RelationshipId, out RelationSubSet r))
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                    {
                        if (personCache.TryGetValue(p1.Value, out PersonSubset person1))
                        {
                            if (person1.Sex == "0")
                                motherId = p1.Value;
                            else
                                fatherId = p1.Value;

                        }
                    }

                    var p2 = r.Person2Id;
                    if (p2 != null)
                    {
                        //var person2 = personCache.FirstOrDefault(p => p.Id == p2);
                        if (personCache.TryGetValue(p2.Value, out PersonSubset person2))
                        {
                            if (person2.Sex == "0")
                                motherId = p2.Value;
                            else
                                fatherId = p2.Value;

                        }
                    }
                }

            }

            return new List<int>() { motherId, fatherId };

        }


        private List<DupeLocInfo> GetParents(int personId)
        {
            //child relationship joins a person to a relationship i.e. their parents relationship
            //   var crs = childRelationshipCache.Where(w => w.PersonId == personId).ToList();
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            if (childRelationshipPersonIndex.TryGetValue(personId, out int index))
            {
                //the RelationshipId should ALWAYS be in this dictionary but but but 
                if (relationshipDictionary.TryGetValue(childRelationshipCache[index].RelationshipId, out RelationSubSet r))
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                    {

                        if (personCache.TryGetValue(p1.Value, out PersonSubset person1))
                            list.AddRange(GetPersonsLocDetails(r.Person1Id.Value, false, person1.Sex == "0", person1.Sex == "1"));
                    }

                    var p2 = r.Person2Id;
                    if (p2 != null)
                    {
                        //var person2 = personCache.FirstOrDefault(p => p.Id == p2);

                        if (personCache.TryGetValue(p2.Value, out PersonSubset person2))
                            list.AddRange(GetPersonsLocDetails(r.Person2Id.Value, false, person2.Sex == "0", person2.Sex == "1"));
                    }
                }

            }




            //foreach (var c in crs)
            //  {

            //  }

            return list;
        }

        private List<DupeLocInfo> GetDupeLocationInfo(int personId)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            list.AddRange(GetPersonsLocDetails(personId, false, false, false));

            list.AddRange(GetSiblings(personId));

            list.AddRange(GetParents(personId));

            return list;
        }

        public ProcessLocationReturnType GetAllLocationsForPerson(int personId)
        {
            var returnType = new ProcessLocationReturnType();



            List<string> countries = new List<string>();
            List<string> counties = new List<string>();

            List<FTMPlaceCacheSubset> foundLocations = new List<FTMPlaceCacheSubset>();

            var list = GetDupeLocationInfo(personId);

            foreach (var place in list)
            {
                //var cachedPlace = fTMPlaceCaches.FirstOrDefault(fpc => fpc.FTMPlaceId == place.PlaceID);

                if (fTMPlaceCaches.TryGetValue(place.PlaceID, out FTMPlaceCacheSubset cachedPlace))
                {

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
                var location = foundLocations.OrderBy(o => (int)o.DupeLocInfoType).First();

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


        #endregion

    }
}
