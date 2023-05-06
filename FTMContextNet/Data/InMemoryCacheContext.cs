using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FTM.Dates;
using FTMContext;
using FTMContextNet.Domain.Entities.NonPersistent;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using FTMContextNet.Domain.Entities.Persistent.Source.Gedcom;
using GedcomParser.Services;
using LoggingLib;
using Microsoft.EntityFrameworkCore;
using PlaceLib.Model;
using QuickGed.Types;
using DupeAgeInfo = FTMContext.DupeAgeInfo;
using DupeAgeInfoTypes = FTMContext.DupeAgeInfoTypes;
using ProcessDateReturnType = FTMContext.ProcessDateReturnType;

namespace FTMContextNet.Data
{
    public interface IInMemoryCacheContext
    {
        ProcessDateReturnType GetPersonBirthDateRange(int personId);

        List<DupeLocInfo> GetPersonsLocDetails(int personId
            , bool isSibling, bool isFather, bool isMother);
        
        List<int> GetParentIds(int personId);

        ProcessLocationReturnType GetAllLocationsForPerson(int personId);

        Dictionary<int, PersonOrigin> OriginDictionary { get; set; }

        Dictionary<int, RelationSubSet> RelationshipDictionary { get; set; }

        Dictionary<int, PersonSubset> PersonCache { get; set; }
    }

    public class InMemoryGedCacheContext : IInMemoryCacheContext
    {
        public static DateObj GetDateObj(DatePlace birthPlace, DatePlace baptismPlace)
        {
            var returnObj = new DateObj()
            {
                Place = "",
                DateStr = "",
                YearInt = 0
            };

            if (birthPlace != null)
            {
                if (!string.IsNullOrEmpty(birthPlace.Date))
                {
                    returnObj.YearInt = MatchTreeHelpers.ExtractInt(birthPlace.Date);
                    returnObj.DateStr = birthPlace.Date;
                }

                if (!string.IsNullOrEmpty(birthPlace.Place))
                {
                    returnObj.Place = birthPlace.Place;
                }
            }

            if (baptismPlace != null)
            {
                if (!string.IsNullOrEmpty(baptismPlace.Date))
                {
                    int bapInt = MatchTreeHelpers.ExtractInt(baptismPlace.Date);
                    string bapStr = baptismPlace.Date;

                    if (returnObj.YearInt == 0)
                    {
                        returnObj.YearInt = bapInt;
                        returnObj.DateStr = bapStr;
                    }
                }

                if (!string.IsNullOrEmpty(baptismPlace.Place))
                {
                    if (returnObj.Place == "")
                    {
                        returnObj.Place = baptismPlace.Place;
                    }
                }
            }

            return returnObj;
        }


        public Dictionary<int, PersonOrigin> OriginDictionary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<int, RelationSubSet> RelationshipDictionary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Dictionary<int, PersonSubset> PersonCache { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public InMemoryGedCacheContext(GedPseudoContext gedContext,
                           DbSet<FtmPlaceCache> fTmPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins, int startPerson =0)
        {

            int total = gedContext.PersonContainer.Persons.Count;
            Console.WriteLine("Importing " + total + " persons");
            int countPeople = 0;

            Dictionary<string, int> idDictionary = new Dictionary<string, int>();

            foreach (var p in gedContext.PersonContainer.Persons)
            {
                idDictionary.Add(p.StrId,startPerson);
                startPerson++;
            }

            foreach (var p in gedContext.PersonContainer.Persons)
            {
                // fileParser.PersonContainer.ChildRelations[0].From.Id
                var parents = gedContext.PersonContainer.ChildRelations.Where(w => w.From.StrId == p.StrId).ToList();
                string fatherId = "";
                string motherId = "";
                //   

                if (parents.Count() == 2)
                {
                    fatherId = parents[0].To.StrId;
                    motherId = parents[1].To.StrId;
                }

                if (parents.Count() == 1)
                {
                    motherId = parents[0].To.StrId;
                }



                var birth = GetDateObj(p.Birth, p.Baptized);
                var death = GetDateObj(p.Death, null);

                

                PersonCache.Add(idDictionary[p.StrId], new PersonSubset()
                {
                     
                });

               // if (countPeople % 50 == 0)
               //     consoleWrapper.ProgressUpdate(countPeople, total, "");

                countPeople++;
            }



        }

        public static InMemoryGedCacheContext Create(GedPseudoContext gedContext,
                           DbSet<FtmPlaceCache> fTmPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins,
                            Ilog ilog)
        {
            return new InMemoryGedCacheContext(gedContext, fTmPlaceCache, ftmPersonOrigins);
        }

        public ProcessLocationReturnType GetAllLocationsForPerson(int personId)
        {
            throw new NotImplementedException();
        }

        public List<int> GetParentIds(int personId)
        {
            throw new NotImplementedException();
        }

        public ProcessDateReturnType GetPersonBirthDateRange(int personId)
        {
            throw new NotImplementedException();
        }

        public List<DupeLocInfo> GetPersonsLocDetails(int personId, bool isSibling, bool isFather, bool isMother)
        {
            throw new NotImplementedException();
        }
    }

    public class InMemoryCacheContext : IInMemoryCacheContext
    {
        public Dictionary<int, PersonOrigin> OriginDictionary { get; set; }

        public Dictionary<int, RelationSubSet> RelationshipDictionary { get; set; }

        public Dictionary<int, PersonSubset> PersonCache { get; set; }


        private Dictionary<int, FactSubset> _baptismFactCache;
        
        /// <summary>
        /// marriage fact linked to person ids
        /// </summary>
        private Dictionary<int, FactSubset> _marriageFactCache;

        private Dictionary<int, int> _childRelationshipPersonIndex;

        private Dictionary<int, int> _childRelationshipIndex;

        private List<ChildRelationshipSubset> _childRelationshipCache;

        private Dictionary<int, List<int>> _personMapRelationshipDictionary;
        
        private Dictionary<int, FTMPlaceCacheSubset> _currentImportPlaceCache;

        public InMemoryCacheContext(FTMakerContext sourceContext,
                           DbSet<FtmPlaceCache> fTmPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins)
        {
            
            _currentImportPlaceCache = new Dictionary<int, FTMPlaceCacheSubset>();
             
            foreach (var f in fTmPlaceCache.Select(s => new FTMPlaceCacheSubset()
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
                if (!_currentImportPlaceCache.ContainsKey(f.FTMPlaceId))
                    _currentImportPlaceCache.Add(f.FTMPlaceId, f);
            }


            _baptismFactCache = new Dictionary<int, FactSubset>();

            foreach (var f in sourceContext.Fact.Where(w => w.FactTypeId == 7
                                                 && w.LinkTableId == 5).Select(s => new FactSubset()
                                                 {
                                                     Id = s.Id,
                                                     Date = MakeDate(s.Date),
                                                     LinkId = s.LinkId,
                                                     PlaceId = s.PlaceId,
                                                     Text = s.Text
                                                 }))
            {

                if (f.Date != null && !_baptismFactCache.ContainsKey(f.LinkId))
                    _baptismFactCache.Add(f.LinkId, f);
            }


            OriginDictionary = new Dictionary<int, PersonOrigin>();

            foreach (var person in ftmPersonOrigins)
            {
                if (!OriginDictionary.ContainsKey(person.PersonId))
                    OriginDictionary.Add(person.PersonId, new PersonOrigin {DirectAncestor = person.DirectAncestor, Origin = person.Origin});
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



            var relationships = sourceContext.Relationship.Select(s => new RelationSubSet()
            {
                Id = s.Id,
                Person1Id = s.Person1Id,
                Person2Id = s.Person2Id
            }).ToList();

            _marriageFactCache = new Dictionary<int, FactSubset>();

            #region populate marriage fact cache

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

            foreach (var f in sourceContext.Fact.Where(w => w.FactTypeId == 4
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

                if (OriginDictionary.ContainsKey(relation.Person1Id.GetValueOrDefault()))
                    origin = OriginDictionary[relation.Person1Id.GetValueOrDefault()].Origin;


              //  relation.Date = f.Date;
                relation.Text = f.Text;
               // relation.PlaceId = f.PlaceId.GetValueOrDefault();
                relation.Origin = origin;


                updateDictionary(_marriageFactCache, f, relation.Person1Id);

                updateDictionary(_marriageFactCache, f, relation.Person2Id);
            }

            #endregion


            foreach (var r in relationships)
            {
                var origin = "";

                if (OriginDictionary.ContainsKey(r.Person1Id.GetValueOrDefault()))
                    origin = OriginDictionary[r.Person1Id.GetValueOrDefault()].Origin;

                if (origin == "")
                {
                    if (OriginDictionary.ContainsKey(r.Person2Id.GetValueOrDefault()))
                        origin = OriginDictionary[r.Person2Id.GetValueOrDefault()].Origin;
                }

                r.Origin = origin;
                //fTMPlaceCaches

                //if (r.PlaceId != null && _currentImportPlaceCache.ContainsKey(r.PlaceId.Value))
                //{
                //    var placeName = _currentImportPlaceCache[r.PlaceId.Value];

                //    r.PlaceName = placeName.FTMOrginalNameFormatted;
                //}
            }


            _personMapRelationshipDictionary = new Dictionary<int, List<int>>();
            RelationshipDictionary = new Dictionary<int, RelationSubSet>();

            foreach (var relationship in relationships)
            {
                //if (person.Person1Id == 8290) {
                //    Debug.WriteLine("break");
                //}

                //relationshipMapPersonDictionary.

                if (!RelationshipDictionary.ContainsKey(relationship.Id))
                {
                    RelationshipDictionary.Add(relationship.Id, relationship);
                }
                else
                {
                    Debug.Assert(true, "No dupe IDs should exist - but in these tables who really knows");
                }


                if (relationship.Person1Id != null)
                {
                    if (_personMapRelationshipDictionary.ContainsKey(relationship.Person1Id.Value))
                    {
                        _personMapRelationshipDictionary[relationship.Person1Id.Value].Add(relationship.Id);
                    }
                    else
                    {
                        var tpList = new List<int> { relationship.Id };

                        _personMapRelationshipDictionary.Add(relationship.Person1Id.Value, tpList);
                    }
                }

            };

            foreach (var person in relationships)
            {
                if (person.Person2Id != null)
                {
                    if (_personMapRelationshipDictionary.ContainsKey(person.Person2Id.Value))
                    {
                        _personMapRelationshipDictionary[person.Person2Id.Value].Add(person.Id);
                    }
                    else
                    {
                        var tpList = new List<int> { person.Id };

                        _personMapRelationshipDictionary.Add(person.Person2Id.Value, tpList);
                    }
                }
            }


            PersonCache = new Dictionary<int, PersonSubset>();

            //foreach (var p in sourceContext.Person.Select(s => new PersonSubset()
            //{
            //    Id = s.Id,
            //    Sex = s.Sex,
            //    BirthDate = s.BirthDate,
            //    BirthPlaceId = s.BirthPlaceId,
            //    DeathDate = s.DeathDate,
            //    DeathPlaceId = s.DeathPlaceId,
            //    FamilyName = s.FamilyName,
            //    Forename = s.GivenName
            //}))
            //{
            //    PersonCache.Add(p.Id, p);
            //}

            _childRelationshipPersonIndex = new Dictionary<int, int>();
            _childRelationshipIndex = new Dictionary<int, int>();

            _childRelationshipCache = new List<ChildRelationshipSubset>(sourceContext.ChildRelationship.Select(s => new ChildRelationshipSubset()
            {
                Id = s.Id,
                PersonId = s.PersonId,
                RelationshipId = s.RelationshipId
            }));

            _childRelationshipCache = _childRelationshipCache.OrderBy(o => o.RelationshipId).ToList();

            var idx = 0;
            var currentRelationShip = _childRelationshipCache[0].RelationshipId;

            _childRelationshipCache[0].StartIndex = 0;
            var startIdx = 0;

            while (idx < _childRelationshipCache.Count)
            {
                //if (childRelationshipCache[idx].RelationshipId == 3479) {
                //    Debug.WriteLine("");
                //}

                if (currentRelationShip != _childRelationshipCache[idx].RelationshipId)
                {
                    startIdx = idx;
                    currentRelationShip = _childRelationshipCache[idx].RelationshipId;
                }

                _childRelationshipCache[idx].StartIndex = startIdx;

                if (!_childRelationshipPersonIndex.ContainsKey(_childRelationshipCache[idx].PersonId))
                {
                    _childRelationshipPersonIndex.Add(_childRelationshipCache[idx].PersonId, idx);
                }

                if (!_childRelationshipIndex.ContainsKey(_childRelationshipCache[idx].RelationshipId))
                {
                    _childRelationshipIndex.Add(_childRelationshipCache[idx].RelationshipId, idx);
                }

                idx++;
            }

            

        }

        public static InMemoryCacheContext Create(FTMakerContext sourceContext,
                           DbSet<FtmPlaceCache> fTmPlaceCache,
                           DbSet<FTMPersonOrigin> ftmPersonOrigins,
                            Ilog ilog)
        {
            return new InMemoryCacheContext(sourceContext, fTmPlaceCache, ftmPersonOrigins);
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

            if (PersonCache.TryGetValue(personId, out PersonSubset person))
            {
                if (uint.TryParse(person.BirthDate, out uint birthDate))
                {
                    var tp = Date.CreateInstance(birthDate);

                    if (FactSubset.ValidYear(tp))
                    {
                        list.Add(new DupeAgeInfo()
                        {
                        //    PlaceID = person.BirthPlaceId,
                            Type = DupeAgeInfoTypes.BirthBap,
                            Year = tp.Year.Value
                        });
                    }
                }

                //get baptisms from fact table
                // var baptisms = baptismFactCache.Where(w => w.LinkId == personId);//fact type 7

                if (_baptismFactCache.TryGetValue(personId, out FactSubset bap))
                {
                    if (FactSubset.ValidYear(bap.Date))
                    {
                        list.Add(new DupeAgeInfo()
                        {
                        //    PlaceID = person.BirthPlaceId,
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

            if (!_personMapRelationshipDictionary.ContainsKey(personId)) return null;

            //is this guy part of any parental relationships
            foreach (var relationshipId in _personMapRelationshipDictionary[personId])
            {

                //  var relationshipId = pr;

                //    var otherChildren = childRelationshipCache.Where(w => w.RelationshipId == relationshipId).ToList();
                List<int> children = new List<int>();

                if (_childRelationshipIndex.TryGetValue(relationshipId, out int index))
                {

                    while (_childRelationshipCache.Count > index &&
                           _childRelationshipCache[index].RelationshipId == relationshipId)
                    {
                        children.Add(_childRelationshipCache[index].PersonId);
                        index++;
                    }

                }


                foreach (var childId in children)
                {
                    DupeAgeInfo childBirth = null;

                    //var person = personCache.FirstOrDefault(w => w.Id == child.PersonId);

                    if (PersonCache.TryGetValue(childId, out PersonSubset person))
                    {
                        if (uint.TryParse(person.BirthDate, out uint birthDate))
                        {
                            var tp = Date.CreateInstance(birthDate);

                            if (FactSubset.ValidYear(tp))
                            {
                                childBirth = new DupeAgeInfo()
                                {
                                 //   PlaceID = person.BirthPlaceId,
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

                        if (_baptismFactCache.TryGetValue(childId, out FactSubset bap))
                        {
                            if (FactSubset.ValidYear(bap.Date))
                            {
                                childBirth = new DupeAgeInfo()
                                {
                                //    PlaceID = person.BirthPlaceId,
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


            if (PersonCache.TryGetValue(personId, out PersonSubset person))
            {
                if (uint.TryParse(person.DeathDate, out uint deathDate))
                {
                    var tp = Date.CreateInstance(deathDate);

                    if (FactSubset.ValidYear(tp))
                    {
                        list.Add(new DupeAgeInfo()
                        {
                        //    PlaceID = person.DeathPlaceId,
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

            if (_marriageFactCache.TryGetValue(personId, out FactSubset marriage))
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

            if (_marriageFactCache.TryGetValue(personId, out FactSubset marriage))
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


            if (PersonCache.TryGetValue(personId, out PersonSubset person))
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

                //if (person.BirthPlaceId != null && person.BirthPlaceId.HasValue)
                //{
                //    list.Add(new DupeLocInfo()
                //    {
                //        PlaceID = person.BirthPlaceId.Value,
                //        Type = dupeLocInfoTypes,
                //        Year = birthYear
                //    });
                //}

                //get baptisms from fact table

                if (_baptismFactCache.TryGetValue(personId, out FactSubset bap))
                {
                    int? bapYear = null;

                    if (FactSubset.ValidYear(bap.Date))
                    {
                        bapYear = bap.Date.Year.Value;
                    }

                    //if (person.BirthPlaceId != null && person.BirthPlaceId.HasValue)
                    //{
                    //    list.Add(new DupeLocInfo()
                    //    {
                    //        PlaceID = person.BirthPlaceId.Value,
                    //        Type = DupeLocInfoTypes.BirthBapLoc,
                    //        Year = bapYear
                    //    });
                    //}
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



            if (PersonCache.TryGetValue(personId, out PersonSubset person))
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

                //if (person.DeathPlaceId.HasValue)
                //{
                //    list.Add(new DupeLocInfo()
                //    {
                //        PlaceID = person.DeathPlaceId.Value,
                //        Type = dupeLocInfoTypes,
                //        Year = deathYear
                //    });
                //}
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
            if (!_personMapRelationshipDictionary.ContainsKey(personId)) return list;

            foreach (var relationshipId in _personMapRelationshipDictionary[personId])
            {
                //RelationshipId

                List<int> children = new List<int>();

                if (_childRelationshipIndex.TryGetValue(relationshipId, out int index))
                {

                    while (index < _childRelationshipCache.Count()
                        && _childRelationshipCache[index].RelationshipId == relationshipId)
                    {
                        children.Add(_childRelationshipCache[index].PersonId);
                        index++;
                    }

                }


                foreach (var childId in children)
                {

                    int? year = null;

                    if (PersonCache.TryGetValue(childId, out PersonSubset person))
                    {
                        if (uint.TryParse(person.BirthDate, out uint birthDate))
                        {
                            var tp = Date.CreateInstance(birthDate);
                            year = tp.Year != null && tp.HasYear() ? tp.Year : 0;
                        }

                        //if (person.BirthPlaceId != null && person.BirthPlaceId.Value != 0)
                        //{
                        //    list.Add(new DupeLocInfo()
                        //    {
                        //        PlaceID = person.BirthPlaceId.Value,
                        //        Type = DupeLocInfoTypes.ChildBirthLoc,
                        //        Year = year
                        //    });
                        //}
                    }

                    //get baptisms from fact table

                    if (_baptismFactCache.TryGetValue(childId, out FactSubset bap))
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

            if (_childRelationshipPersonIndex.TryGetValue(personId, out int index))
            {
                var startIdx = _childRelationshipCache[index].StartIndex;
                var relationshipId = _childRelationshipCache[startIdx].RelationshipId;
                var currentRelationshipId = _childRelationshipCache[startIdx].RelationshipId;

                while (relationshipId == currentRelationshipId && startIdx < _childRelationshipCache.Count)
                {

                    currentRelationshipId = _childRelationshipCache[startIdx].RelationshipId;

                    siblingList.Add(_childRelationshipCache[startIdx].PersonId);
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

            if (_childRelationshipPersonIndex.TryGetValue(personId, out int index))
            {
                //the RelationshipId should ALWAYS be in this dictionary but but but 
                if (RelationshipDictionary.TryGetValue(_childRelationshipCache[index].RelationshipId, out RelationSubSet r))
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                    {
                        if (PersonCache.TryGetValue(p1.Value, out PersonSubset person1))
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
                        if (PersonCache.TryGetValue(p2.Value, out PersonSubset person2))
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

            if (_childRelationshipPersonIndex.TryGetValue(personId, out int index))
            {
                //the RelationshipId should ALWAYS be in this dictionary but but but 
                if (RelationshipDictionary.TryGetValue(_childRelationshipCache[index].RelationshipId, out RelationSubSet r))
                {
                    var p1 = r.Person1Id;
                    if (p1 != null)
                    {

                        if (PersonCache.TryGetValue(p1.Value, out PersonSubset person1))
                            list.AddRange(GetPersonsLocDetails(r.Person1Id.Value, false, person1.Sex == "0", person1.Sex == "1"));
                    }

                    var p2 = r.Person2Id;
                    if (p2 != null)
                    {
                        //var person2 = personCache.FirstOrDefault(p => p.Id == p2);

                        if (PersonCache.TryGetValue(p2.Value, out PersonSubset person2))
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

                if (_currentImportPlaceCache.TryGetValue(place.PlaceID, out FTMPlaceCacheSubset cachedPlace))
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
