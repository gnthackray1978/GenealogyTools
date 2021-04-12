using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ConfigHelper;
using ConsoleTools;
using FTMContext.lib;
using Microsoft.EntityFrameworkCore;
using MyFamily.Shared;

namespace FTMContext.Models
{
    
    public class CacheObject
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

        public void Create(FTMakerContext _sourceContext,
                           DbSet<FTMPlaceCache> fTMPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins,
                            IConsoleWrapper _consoleWrapper)
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
                location_lat = FTMGeoCoding.GetLocation(s.JSONResult).lat,
                location_long = FTMGeoCoding.GetLocation(s.JSONResult).lng
            }))
            {
                if (!fTMPlaceCaches.ContainsKey(f.FTMPlaceId))
                    fTMPlaceCaches.Add(f.FTMPlaceId, f);
            };


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
            };


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


            Action<Dictionary<int, FactSubset>, FactSubset, int?> updateDictionary = (marrDict, factsubset, personId) => {
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

                updateDictionary(marriageFactCache, f, relation.Person1Id);

                updateDictionary(marriageFactCache, f, relation.Person2Id);
            }

            personMapRelationshipDictionary = new Dictionary<int, List<int>>();
            relationshipDictionary = new Dictionary<int, RelationSubSet>();

            var relationshipMapPersonDictionary = new Dictionary<int, int>();

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

            foreach (var p in _sourceContext.Person.Select(s => new PersonSubset()
            {
                Id = s.Id,
                Sex = s.Sex,
                BirthDate = s.BirthDate,
                BirthPlaceId = s.BirthPlaceId,
                DeathDate = s.DeathDate,
                DeathPlaceId = s.DeathPlaceId,
                FamilyName = s.FamilyName,
                GivenName = s.GivenName
            }))
            {
                personCache.Add(p.Id, p);
            };

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

            _consoleWrapper.WriteLine("Cache Created");

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
                return list.OrderBy(o => o.Year).FirstOrDefault();
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
                return list.OrderBy(o => o.Year).FirstOrDefault();
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
                return list.OrderBy(o => o.Year).FirstOrDefault();
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
                return list.OrderBy(o => o.Year).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }


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

        private List<DupeLocInfo> GetDupeLocationInfo(int personId)
        {
            List<DupeLocInfo> list = new List<DupeLocInfo>();

            list.AddRange(this.GetPersonsLocDetails(personId, false, false, false));

            list.AddRange(this.GetSiblings(personId));

            list.AddRange(this.GetParents(personId));

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


        #endregion

    }

    public partial class FTMakerContext : DbContext
    {

        private IMSGConfigHelper _configObj { get; set; }
        private SQLiteConnection _sqlConnection { get; set; }



        public string FileName
        {
            get
            {
                return _configObj.DNA_Match_File_FileName;
            }
        }

        public FTMakerContext(IMSGConfigHelper config)
        {
            _configObj = config;
        }

        public FTMakerContext(DbContextOptions<FTMakerContext> options)
            : base(options)
        {
        }

        public List<string> DumpCount()
        {

            List<string> results = new List<string>();


            DumpRecordCount(results, ChildRelationship, "ChildRelationship");
            DumpRecordCount(results, Deleted, "Deleted");
            DumpRecordCount(results, Fact, "Fact");
            DumpRecordCount(results, FactType, "FactType");
            DumpRecordCount(results, HistoryList, "HistoryList");
            DumpRecordCount(results, MasterSource, "MasterSource");
            DumpRecordCount(results, MediaFile, "MediaFile");
            DumpRecordCount(results, MediaLink, "MediaLink");
            DumpRecordCount(results, Note, "Note");
            DumpRecordCount(results, Person, "Person");
            DumpRecordCount(results, PersonExternal, "PersonExternal");
            DumpRecordCount(results, PersonGroup, "PersonGroup");
            DumpRecordCount(results, Place, "Place");
            DumpRecordCount(results, Publication, "Publication");
            DumpRecordCount(results, Relationship, "Relationship");
            DumpRecordCount(results, Repository, "Repository");
            DumpRecordCount(results, Setting, "Setting");
            DumpRecordCount(results, Source, "Source");
            DumpRecordCount(results, SourceLink, "SourceLink");
            DumpRecordCount(results, SyncArtifact, "SyncArtifact");
            DumpRecordCount(results, SyncArtifactReference, "SyncArtifactReference");
            DumpRecordCount(results, SyncCitation, "SyncCitation");
            DumpRecordCount(results, SyncCitationReference, "SyncCitationReference");
            DumpRecordCount(results, SyncFact, "SyncFact");
            DumpRecordCount(results, SyncFactDelete, "SyncFactDelete");
            DumpRecordCount(results, SyncMedia, "SyncMedia");
            DumpRecordCount(results, SyncNote, "SyncNote");
            DumpRecordCount(results, SyncPerson, "SyncPerson");
            DumpRecordCount(results, SyncRelationship, "SyncRelationship");
            DumpRecordCount(results, SyncRepository, "SyncRepository");
            DumpRecordCount(results, SyncSetting, "SyncSetting");
            DumpRecordCount(results, SyncSource, "SyncSource");
            DumpRecordCount(results, SyncState, "SyncState");
            DumpRecordCount(results, SyncWeblink, "SyncWeblink");
            DumpRecordCount(results, Tag, "Tag");
            DumpRecordCount(results, TagLink, "TagLink");
            DumpRecordCount(results, Task, "Task");
            DumpRecordCount(results, WebLink, "WebLink");

            return results;
        }

        private void DumpRecordCount<t>(List<string> results, DbSet<t> set, string name) where t : class
        {
            string result = "";

            var count = set.Count();

            if (count > 0)
                result = name + " " + set.Count();

            if (result != "")
                results.Add(result);
        }

        public CacheObject CacheObject { get; set; }


        public static FTMakerContext CreateDestinationDB(IMSGConfigHelper imsGConfigHelper)
        {
            return new FTMakerContext(imsGConfigHelper);
        }

        public static FTMakerContext CreateSourceDB(IMSGConfigHelper imsGConfigHelper)
        {
            var source = new FTMakerContext(imsGConfigHelper);

            source.CacheObject = new CacheObject();

            return source;
        }

        public void DeleteAll()
        {
            var tp = GetCon();


            var command = tp.CreateCommand();
            tp.Open();

            command.CommandText = @"delete from Person";
            Console.WriteLine("Delete From Person");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from ChildRelationship";
            Console.WriteLine("Delete From ChildRelationship");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Deleted";
            Console.WriteLine("Delete From Deleted");
            command.ExecuteNonQuery();

            command.CommandText = @"delete from HistoryList";
            Console.WriteLine("Delete From HistoryList");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from MasterSource";
            Console.WriteLine("Delete From MasterSource");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from MediaLink";
            Console.WriteLine("Delete From MediaLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Note";
            Console.WriteLine("Delete From Note");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from PersonExternal";
            Console.WriteLine("Delete From PersonExternal");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from PersonGroup";
            Console.WriteLine("Delete From PersonGroup");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Publication";
            Console.WriteLine("Delete From Publication");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Repository";
            Console.WriteLine("Delete From Repository");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Setting";
            Console.WriteLine("Delete From Setting");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Source";
            Console.WriteLine("Delete From Source");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from SourceLink";
            Console.WriteLine("Delete From SourceLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Tag";
            Console.WriteLine("Delete From Tag");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from TagLink";
            Console.WriteLine("Delete From TagLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Task";
            Console.WriteLine("Delete From Task");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Tag";
            Console.WriteLine("Delete From Tag");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Place";
            Console.WriteLine("Delete From Place");
            command.ExecuteNonQuery();


            command.CommandText = @"delete from Relationship";
            Console.WriteLine("Delete From Relationship");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from MediaFile";
            Console.WriteLine("Delete From MediaFile");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from WebLink";
            Console.WriteLine("Delete From WebLink");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from Fact";
            Console.WriteLine("Delete From Fact");
            command.ExecuteNonQuery();
            command.CommandText = @"delete from FactType";
            Console.WriteLine("Delete From FactType");
            command.ExecuteNonQuery();
            tp.Close();




        }


        public virtual DbSet<ChildRelationship> ChildRelationship { get; set; }
        public virtual DbSet<Deleted> Deleted { get; set; }
        public virtual DbSet<Fact> Fact { get; set; }
        public virtual DbSet<FactType> FactType { get; set; }
        public virtual DbSet<HistoryList> HistoryList { get; set; }
        public virtual DbSet<MasterSource> MasterSource { get; set; }
        public virtual DbSet<MediaFile> MediaFile { get; set; }
        public virtual DbSet<MediaLink> MediaLink { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonExternal> PersonExternal { get; set; }
        public virtual DbSet<PersonGroup> PersonGroup { get; set; }
        public virtual DbSet<Place> Place { get; set; }
        public virtual DbSet<Publication> Publication { get; set; }
        public virtual DbSet<Relationship> Relationship { get; set; }
        public virtual DbSet<Repository> Repository { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<Source> Source { get; set; }
        public virtual DbSet<SourceLink> SourceLink { get; set; }
        public virtual DbSet<SyncArtifact> SyncArtifact { get; set; }
        public virtual DbSet<SyncArtifactReference> SyncArtifactReference { get; set; }
        public virtual DbSet<SyncCitation> SyncCitation { get; set; }
        public virtual DbSet<SyncCitationReference> SyncCitationReference { get; set; }
        public virtual DbSet<SyncFact> SyncFact { get; set; }
        public virtual DbSet<SyncFactDelete> SyncFactDelete { get; set; }
        public virtual DbSet<SyncMedia> SyncMedia { get; set; }
        public virtual DbSet<SyncNote> SyncNote { get; set; }
        public virtual DbSet<SyncPerson> SyncPerson { get; set; }
        public virtual DbSet<SyncRelationship> SyncRelationship { get; set; }
        public virtual DbSet<SyncRepository> SyncRepository { get; set; }
        public virtual DbSet<SyncSetting> SyncSetting { get; set; }
        public virtual DbSet<SyncSource> SyncSource { get; set; }
        public virtual DbSet<SyncState> SyncState { get; set; }
        public virtual DbSet<SyncWeblink> SyncWeblink { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<TagLink> TagLink { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<WebLink> WebLink { get; set; }


        public void CreateCacheObject(DbSet<FTMPlaceCache> fTMPlaceCache, DbSet<FTMPersonOrigin> ftmPersonOrigins, IConsoleWrapper _consoleWrapper)
        {
            this.CacheObject.Create(this, fTMPlaceCache, ftmPersonOrigins, _consoleWrapper);

        }
        private SQLiteConnection GetCon()
        {

            string cs = "";

            if (_configObj.DNA_Match_File_IsEncrypted)
            {
                cs = "data source=\"" + _configObj.DNA_Match_File_Path
                                      + _configObj.DNA_Match_File_FileName
                                      + "\";synchronous=Off;pooling=False;journal mode=Memory;foreign keys=True;"
                                      + _configObj.FTMConString
                                      + ";datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
            }
            else
            {
                cs = "data source=\"" + _configObj.DNA_Match_File_Path
                                      + _configObj.DNA_Match_File_FileName
                                      + "\";pooling=False;journal mode=Memory;foreign keys=True;datetimekind=Utc;datetimeformat=UnixEpoch;failifmissing=False;read only=False;collate settings=\"en@colCaseFirst=upper\"";
            }

            _sqlConnection = new System.Data.SQLite.SQLiteConnection(cs);

            _sqlConnection.Flags |= SQLiteConnectionFlags.AllowNestedTransactions;

            return _sqlConnection;
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(GetCon());
            }
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChildRelationship>(entity =>
            {
                entity.Property(e => e.RelationshipId).HasColumnName("RelationshipID");

                entity.HasIndex(e => new { e.PersonId, e.RelationshipId })
                    .HasName("CT_ChildRelationship")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PersonId).HasColumnName("PersonID");



                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.ChildRelationship)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChildRelationship_PersonID");

                entity.HasOne(d => d.Relationship)
                    .WithMany(p => p.ChildRelationship)
                    .HasForeignKey(d => d.RelationshipId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChildRelationship_RelationshipID");
            });

            modelBuilder.Entity<Deleted>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.TableId });

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.TableId).HasColumnName("TableID");

                entity.Property(e => e.DeleteDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Fact>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Date).HasMaxLength(255);

                entity.Property(e => e.FactTypeId).HasColumnName("FactTypeID");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.PlaceId).HasColumnName("PlaceID");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.Text).HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.FactType)
                    .WithMany(p => p.Fact)
                    .HasForeignKey(d => d.FactTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Fact_FactTypeID");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.Fact)
                    .HasForeignKey(d => d.PlaceId)
                    .HasConstraintName("FK_Fact_PlaceID");
            });

            modelBuilder.Entity<FactType>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Abbreviation).HasMaxLength(1001);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.SentenceFormat).HasMaxLength(1001);

                entity.Property(e => e.ShortName).HasMaxLength(1001);

                entity.Property(e => e.Tag)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<HistoryList>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ActionDescription)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.HistoryDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ObjectId).HasColumnName("ObjectID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<MasterSource>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Author).HasMaxLength(1001);

                entity.Property(e => e.CallNumber).HasMaxLength(1001);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Pid)
                    .HasColumnName("PID")
                    .HasMaxLength(250);

                entity.Property(e => e.PublishDate).HasMaxLength(1001);

                entity.Property(e => e.PublisherLocation).HasMaxLength(1001);

                entity.Property(e => e.PublisherName).HasMaxLength(1001);

                entity.Property(e => e.RepositoryId).HasColumnName("RepositoryID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Repository)
                    .WithMany(p => p.MasterSource)
                    .HasForeignKey(d => d.RepositoryId)
                    .HasConstraintName("FK_MasterSource_RepositoryID");
            });

            modelBuilder.Entity<MediaFile>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FileCaption).HasMaxLength(1001);

                entity.Property(e => e.FileDate).HasMaxLength(1001);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.Guid).HasColumnName("GUID");

                entity.Property(e => e.ModifiedTime).HasColumnType("datetime");

                entity.Property(e => e.Pid)
                    .HasColumnName("PID")
                    .HasMaxLength(250);

                entity.Property(e => e.Thumbnail)
                    .HasMaxLength(1)
                    .IsFixedLength();

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<MediaLink>(entity =>
            {
                entity.HasIndex(e => new { e.LinkId, e.LinkTableId, e.MediaFileId })
                    .HasName("CT_MediaLink")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.MediaFileId).HasColumnName("MediaFileID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MediaFile)
                    .WithMany(p => p.MediaLink)
                    .HasForeignKey(d => d.MediaFileId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaLink_MediaFileID");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasIndex(e => new { e.LinkId, e.LinkTableId, e.Category })
                    .HasName("CT_Note")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BirthDate).HasMaxLength(255);

                entity.Property(e => e.BirthFactId).HasColumnName("BirthFactID");

                entity.Property(e => e.BirthPlace).HasMaxLength(1001);

                entity.Property(e => e.BirthPlaceId).HasColumnName("BirthPlaceID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DeathDate).HasMaxLength(255);

                entity.Property(e => e.DeathFactId).HasColumnName("DeathFactID");

                entity.Property(e => e.DeathPlace).HasMaxLength(1001);

                entity.Property(e => e.DeathPlaceId).HasColumnName("DeathPlaceID");

                entity.Property(e => e.FamilyName).IsUnicode(false);

                entity.Property(e => e.FullName).IsUnicode(false);

                entity.Property(e => e.FullNameReversed).IsUnicode(false);

                entity.Property(e => e.GivenName).IsUnicode(false);

                entity.Property(e => e.MarriageDate).HasMaxLength(255);

                entity.Property(e => e.MarriageFactId).HasColumnName("MarriageFactID");

                entity.Property(e => e.MarriagePlace).HasMaxLength(1001);

                entity.Property(e => e.MarriagePlaceId).HasColumnName("MarriagePlaceID");

                entity.Property(e => e.NameFactId).HasColumnName("NameFactID");

                entity.Property(e => e.NameSuffix).IsUnicode(false);

                entity.Property(e => e.PersonGuid)
                    .IsRequired()
                    .HasColumnName("PersonGUID");

                entity.Property(e => e.PreferredMediaFileId).HasColumnName("PreferredMediaFileID");

                entity.Property(e => e.PreferredRelId).HasColumnName("PreferredRelID");

                entity.Property(e => e.Sex).HasMaxLength(10);

                entity.Property(e => e.SexFactId).HasColumnName("SexFactID");

                entity.Property(e => e.Title).IsUnicode(false);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.BirthFact)
                    .WithMany(p => p.PersonBirthFact)
                    .HasForeignKey(d => d.BirthFactId)
                    .HasConstraintName("FK_Person_BirthFactID");

                entity.HasOne(d => d.BirthPlaceNavigation)
                    .WithMany(p => p.PersonBirthPlaceNavigation)
                    .HasForeignKey(d => d.BirthPlaceId)
                    .HasConstraintName("FK_Person_BirthPlaceID");

                entity.HasOne(d => d.DeathFact)
                    .WithMany(p => p.PersonDeathFact)
                    .HasForeignKey(d => d.DeathFactId)
                    .HasConstraintName("FK_Person_DeathFactID");

                entity.HasOne(d => d.DeathPlaceNavigation)
                    .WithMany(p => p.PersonDeathPlaceNavigation)
                    .HasForeignKey(d => d.DeathPlaceId)
                    .HasConstraintName("FK_Person_DeathPlaceID");

                entity.HasOne(d => d.MarriageFact)
                    .WithMany(p => p.PersonMarriageFact)
                    .HasForeignKey(d => d.MarriageFactId)
                    .HasConstraintName("FK_Person_MarriageFactID");

                entity.HasOne(d => d.MarriagePlaceNavigation)
                    .WithMany(p => p.PersonMarriagePlaceNavigation)
                    .HasForeignKey(d => d.MarriagePlaceId)
                    .HasConstraintName("FK_Person_MarriagePlaceID");

                entity.HasOne(d => d.NameFact)
                    .WithMany(p => p.PersonNameFact)
                    .HasForeignKey(d => d.NameFactId)
                    .HasConstraintName("FK_Person_NameFactID");

                entity.HasOne(d => d.PreferredMediaFile)
                    .WithMany(p => p.Person)
                    .HasForeignKey(d => d.PreferredMediaFileId)
                    .HasConstraintName("FK_Person_PreferredMediaFileID");

                //entity.HasOne(d => d.PreferredRel)
                //    .WithMany(p => p.Person)
                //    .HasForeignKey(d => d.PreferredRelId)
                //    .HasConstraintName("FK_Person_PreferredRelID");

                entity.HasOne(d => d.SexFact)
                    .WithMany(p => p.PersonSexFact)
                    .HasForeignKey(d => d.SexFactId)
                    .HasConstraintName("FK_Person_SexFactID");
            });

            modelBuilder.Entity<PersonExternal>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExternalId)
                    .IsRequired()
                    .HasColumnName("ExternalID")
                    .HasMaxLength(100);

                entity.Property(e => e.PersonId).HasColumnName("PersonID");

                entity.Property(e => e.ProviderId).HasColumnName("ProviderID");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.PersonExternal)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonExternal_PersonID");
            });

            modelBuilder.Entity<PersonGroup>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.RootPersonId).HasColumnName("RootPersonID");

                entity.Property(e => e.SubgroupFfid).HasColumnName("SubgroupFFID");

                entity.Property(e => e.SubgroupFmid).HasColumnName("SubgroupFMID");

                entity.Property(e => e.SubgroupMfid).HasColumnName("SubgroupMFID");

                entity.Property(e => e.SubgroupMmid).HasColumnName("SubgroupMMID");

                entity.Property(e => e.Type).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DisplayName).HasMaxLength(512);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(475);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Publication>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.Xml).IsRequired();
            });

            modelBuilder.Entity<Relationship>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Person1Id).HasColumnName("Person1ID");

                entity.Property(e => e.Person2Id).HasColumnName("Person2ID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Person1)
                    .WithMany(p => p.RelationshipPerson1)
                    .HasForeignKey(d => d.Person1Id)
                    .HasConstraintName("FK_Relationship_Person1ID");

                entity.HasOne(d => d.Person2)
                    .WithMany(p => p.RelationshipPerson2)
                    .HasForeignKey(d => d.Person2Id)
                    .HasConstraintName("FK_Relationship_Person2ID");
            });

            modelBuilder.Entity<Repository>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(1001);

                entity.Property(e => e.Contact).HasMaxLength(1001);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(1001);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.Phone).HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.StringValue).HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Abbreviated).HasMaxLength(1001);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MasterSourceId).HasColumnName("MasterSourceID");

                entity.Property(e => e.Pid)
                    .HasColumnName("PID")
                    .HasMaxLength(250);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.MasterSource)
                    .WithMany(p => p.Source)
                    .HasForeignKey(d => d.MasterSourceId)
                    .HasConstraintName("FK_Source_MasterSourceID");
            });

            modelBuilder.Entity<SourceLink>(entity =>
            {
                entity.HasIndex(e => new { e.LinkId, e.LinkTableId, e.SourceId })
                    .HasName("CT_SourceLink")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExternalId)
                    .HasColumnName("ExternalID")
                    .HasMaxLength(250);

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.SourceId).HasColumnName("SourceID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Source)
                    .WithMany(p => p.SourceLink)
                    .HasForeignKey(d => d.SourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SourceLink_SourceID");
            });

            modelBuilder.Entity<SyncArtifact>(entity =>
            {
                entity.ToTable("Sync_Artifact");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Art__6A58991E63C7AE64")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Art__34C76C94B44F19AB")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncArtifactReference>(entity =>
            {
                entity.ToTable("Sync_ArtifactReference");

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncCitation>(entity =>
            {
                entity.ToTable("Sync_Citation");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Cit__6A58991EB7853947")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Cit__34C76C942C6FB621")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.SyncCitation)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Citation_0");
            });

            modelBuilder.Entity<SyncCitationReference>(entity =>
            {
                entity.ToTable("Sync_CitationReference");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Cit__6A58991EF18BDDC0")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Cit__34C76C94BD7F744A")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncFact>(entity =>
            {
                entity.ToTable("Sync_Fact");

                //entity.HasIndex(e => e.AmtId)
                //    .HasName("UQ__Sync_Fac__6A58991E1F7D95A7")
                //    .IsUnique();

                //entity.HasIndex(e => e.FtmId)
                //    .HasName("UQ__Sync_Fac__34C76C9444800CF0")
                //    .IsUnique();

                //entity.Property(e => e.AmtId)
                //    .IsRequired()
                //    .HasMaxLength(50);

                //entity.Property(e => e.CreateDate)
                //    .HasColumnType("datetime")
                //    .HasDefaultValueSql("(getdate())");

                //entity.Property(e => e.FtmId)
                //    .IsRequired()
                //    .HasMaxLength(50);

                //entity.Property(e => e.ModifyDate)
                //    .HasColumnType("datetime")
                //    .HasDefaultValueSql("(getdate())");

                //entity.HasOne(d => d.Owner)
                //    .WithMany(p => p.SyncFact)
                //    .HasForeignKey(d => d.OwnerId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_Fact_Person");
            });

            modelBuilder.Entity<SyncFactDelete>(entity =>
            {
                entity.ToTable("Sync_FactDelete");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Fac__6A58991E0ECADCFE")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Fac__34C76C945929B044")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.SyncFactDelete)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FactDelete_Person");
            });

            modelBuilder.Entity<SyncMedia>(entity =>
            {
                entity.ToTable("Sync_Media");

                entity.Property(e => e.Amtid).HasColumnName("AMTId");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DownloadUrl).HasMaxLength(256);

                entity.Property(e => e.EndpointType).HasMaxLength(1000);

                entity.Property(e => e.LastAttemptDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.State).HasMaxLength(100);
            });

            modelBuilder.Entity<SyncNote>(entity =>
            {
                entity.ToTable("Sync_Note");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Not__6A58991E6BD2B1EA")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Not__34C76C944C7FEDA4")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncPerson>(entity =>
            {
                entity.HasKey(e => e.FtmId)
                    .HasName("PK__Sync_Per__34C76C95DE3410F1");

                entity.ToTable("Sync_Person");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Per__6A58991ED5AFBDD1")
                    .IsUnique();

                entity.Property(e => e.FtmId).ValueGeneratedNever();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FamilySearchId).HasMaxLength(20);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncRelationship>(entity =>
            {
                entity.ToTable("Sync_Relationship");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Rel__6A58991E5A379705")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Rel__34C76C94ADDBEC32")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FtmId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.SyncRelationship)
                    .HasForeignKey(d => d.PersonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Relationship_Person");
            });

            modelBuilder.Entity<SyncRepository>(entity =>
            {
                entity.ToTable("Sync_Repository");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Rep__6A58991EA0138CA9")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Rep__34C76C941B4835C1")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncSetting>(entity =>
            {
                entity.ToTable("Sync_Setting");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.StringValue).HasMaxLength(1001);
            });

            modelBuilder.Entity<SyncSource>(entity =>
            {
                entity.ToTable("Sync_Source");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Sou__6A58991E7157F5F3")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Sou__34C76C94F5C3D95E")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SyncState>(entity =>
            {
                entity.ToTable("Sync_State");

                entity.Property(e => e.ErrorMessage)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.LastSync).HasColumnType("datetime");

                entity.Property(e => e.SyncEnd).HasColumnType("datetime");

                entity.Property(e => e.SyncStart).HasColumnType("datetime");

                entity.Property(e => e.TreeModified).HasColumnType("datetime");

                entity.Property(e => e.TreeName)
                    .IsRequired()
                    .HasMaxLength(180);
            });

            modelBuilder.Entity<SyncWeblink>(entity =>
            {
                entity.ToTable("Sync_Weblink");

                entity.HasIndex(e => e.AmtId)
                    .HasName("UQ__Sync_Web__6A58991ED50A9BAE")
                    .IsUnique();

                entity.HasIndex(e => e.FtmId)
                    .HasName("UQ__Sync_Web__34C76C9438A5F05D")
                    .IsUnique();

                entity.Property(e => e.AmtId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.SyncWeblink)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Weblink_Person");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TableId).HasColumnName("TableID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<TagLink>(entity =>
            {
                entity.HasIndex(e => new { e.TagId, e.LinkId, e.LinkTableId })
                    .HasName("CT_TagLink")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.TagId).HasColumnName("TagID");

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.TagLink)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TagID");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.TaskText)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<WebLink>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LinkId).HasColumnName("LinkID");

                entity.Property(e => e.LinkTableId).HasColumnName("LinkTableID");

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Url).HasMaxLength(1001);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
