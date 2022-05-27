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
using LoggingLib;

namespace FTMContext
{
    public class FTMViewCreator
    {
        protected Ilog Ilog;

        protected CacheObject _srcCache;

        protected FTMakerCacheContext _destinationContext;
        
        public FTMViewCreator(FTMakerContext _sourceContext,  FTMakerCacheContext _destinationContext, 
            Ilog ilog) {

            Ilog = ilog;

            this._destinationContext = _destinationContext;

            _sourceContext.CreateCacheObject(_destinationContext.FTMPlaceCache,this._destinationContext.FTMPersonOrigins, ilog);

            this._srcCache = _sourceContext.CacheObject;


        }

        public void CheckForChangedLocs()
        {
            int idx = 0;

            foreach (var p in this._srcCache.personCache.Values)
            {
                this._srcCache.GetAllLocationsForPerson(p.Id);

                if (idx % 250 == 0) {
                    Ilog.WriteLine(idx.ToString());
                }

                idx++;
            }

            Ilog.WriteLine("finished");
        }


        public void CheckForChangedDates() {
           

            //missing persons
            //changed persons

            List<int> missingPersonIds = new List<int>();
            List<int> updatedPersonIds = new List<int>();

            int peopleCount = this._srcCache.personCache.Count();
            int counter = 0;
            int saveCounter = 0;

            Ilog.WriteLine(peopleCount + " Records ");
 
            foreach (var p in this._srcCache.personCache.Values)
            {
                if (!_srcCache.Type90Facts.Select(s => s.Key).Contains(p.Id))
                {
                    missingPersonIds.Add(p.Id);
                }
                else {

                    var processDateReturnType = this._srcCache.ProcessDates(p.Id);
                    var currentOriginDates = processDateReturnType.RangeString.Trim();

                    var existingPair = _srcCache.Type90Facts.FirstOrDefault(f => f.Key == p.Id);
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

           
            string locationList = "";
 
            int peopleCount = this._srcCache.personCache.Count();
            int counter = 0;
            int saveCounter = 0;

            Ilog.WriteLine(peopleCount + " record to process");

            int idCounter = _destinationContext.FTMPersonView.Count() +1;

            foreach (var p in this._srcCache.personCache.Values)
            {
                //if (p.Id == 1924)
                //{
                //    _consoleWrapper.WriteLine("");
                //}


                var processDateReturnType = this._srcCache.ProcessDates(p.Id);

                var processLocationReturnType = this._srcCache.GetAllLocationsForPerson(p.Id);

                var parents = this._srcCache.GetParentIds(p.Id);



                this._srcCache.originDictionary.TryGetValue(p.Id, out string origin);

                

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
                    Origin = origin,
                    LinkedLocations = processLocationReturnType.LocationString,
                    FatherId = parents[0],
                    MotherId = parents[1]
                };

                _destinationContext.FTMPersonView.Add(fTMPersonView);

             

                //_destinationContext.FTMMarriages

                counter++;
                idCounter++;

                if (saveCounter == 1000) {
                    Ilog.WriteCounter("Saving - " + counter + " of " + peopleCount);
                    _destinationContext.SaveChanges();
                    saveCounter = 0;

                }
                saveCounter++;
            }

            _destinationContext.SaveChanges();
            

            

            idCounter = _destinationContext.FTMMarriages.Count() + 1;

            foreach (var r in _srcCache.relationshipDictionary.Values)
            {
                int year = 0;

                if (r.Date != null && r.Date.HasYear())
                    year = r.Date.Year.GetValueOrDefault();

                var marriage = new FTMMarriage()
                {
                    Id = idCounter,
                    BrideId = r.Person1Id.GetValueOrDefault(),
                    GroomId = r.Person2Id.GetValueOrDefault(),
                    MarriageDateStr = year.ToString(),
                    MarriageLocationId = r.PlaceId.GetValueOrDefault(),
                    MarriageYear = year,
                    Notes = r.Text,
                    Origin = r.Origin,
                    MarriageLocation = r.PlaceName

                };

                _destinationContext.FTMMarriages.Add(marriage);
                idCounter++;
            }

            _destinationContext.SaveChanges();
        }


    }
}
