using System;
using System.Collections.Generic;
using ConfigHelper;
using LoggingLib;
using FTMContextNet.Data;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Application.Services;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Application.Mapping;
using AutoMapper;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Application.Models.Write;
using PlaceLibNet.Application.Services;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Data.Repositories;
using QuickGed.Domain;
using QuickGed.Services;

namespace FTMContextNet
{
    public class FTMFacade
    {
        private readonly Ilog _outputHandler;
        private readonly IMapper _mapper;
        private readonly IMSGConfigHelper _iMSGConfigHelper;

        public FTMFacade(IMSGConfigHelper iMSGConfigHelper, Ilog iLog)
        {
            //the source db is created by the adapter
            //the purpose of the facade is to first populate the persisted cache
            //then update the azure db.

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfiguration());
            });
            
            _mapper = config.CreateMapper();

            _iMSGConfigHelper = iMSGConfigHelper;

             _outputHandler = iLog;

            //_outputHandler.WriteLine("Service Cache Created");
        }

        #region places
        
        public IEnumerable<PlaceModel> GetPlaceNotGeocoded(int amount) {
            
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);


            var service = new GetUnknownPlacesSearchedAlreadyServices(placeRepository, _outputHandler, _mapper);
            return service.Execute(amount);
        }

        public void ClearData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates place entry in cacheData.FTMPlaceCache with result we got back from google geocode.
        /// </summary>
        public void WriteGeoCodedData(GeoCodeResultModel value)
        {
           // var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);


            var service = new UpdatePlaceGeoData(placeRepository, _outputHandler, _mapper);

            service.Execute(value);
        }

        /// <summary>
        /// Updates PlaceFormatting field of the place cache table
        /// </summary>
        public void UpdatePlaceCachePlaceFormattingEntry()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);
            
            var service = new UpdatePlaceCacheNameFormatting(placeRepository, _outputHandler);

            service.Execute();
        }

        /// <summary>
        /// Updates Persons table with lat and longs.
        /// </summary>
        public void UpdatePersonLocations()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var service = new UpdatePersonLocations(placeRepository, persistedCacheRepository, _outputHandler);

            service.Execute();
        }

        /// <summary>
        /// look in person table add any places to the cache that aren't already in there.
        /// </summary>
        public void CreateMissingPersonLocations()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);
 
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            
            var service = new CreatePersonLocationsInCache(placeRepository, persistedCacheRepository, _outputHandler);

            service.Execute();

        }
        
        public void UpdatePlaceMetaData()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper),_outputHandler);

            var service = new UpdatePlaceMetaData(placeRepository, _outputHandler);

            service.Execute();
        }

        #endregion
        
        #region tree data

        public void ImportPersons()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var gr = new GedRepository(_outputHandler, new GedParser(new NodeTypeCalculator(), _iMSGConfigHelper.GedPath));

            var createPersonsAndMarriages = new CreatePersonsAndMarriages(persistedCacheRepository, gr, _outputHandler);

            createPersonsAndMarriages.Execute();
        }

        public void CreateDupeView()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var createDupeEntrys = new CreateDupeEntrys(persistedCacheRepository, _outputHandler);

            createDupeEntrys.Execute();

            _outputHandler.WriteLine("Finished Creating Dupe View");
        }

        public void CreatePersonOrigins()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var createPersonOrigin = new CreatePersonOrigin(persistedCacheRepository, _outputHandler);

            createPersonOrigin.Execute();

            _outputHandler.WriteLine("Finished Creating Person Origins");
        }

        public void CreateTreeRecord()
        {
            
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var ctr = new CreateTreeRecords(persistedCacheRepository, _outputHandler);

            ctr.Execute();

            _outputHandler.WriteLine("Finished Creating Tree Record View");
        }

        public void CreateTreeGroups()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var ctr = new CreateTreeGroups(persistedCacheRepository, _outputHandler);

            ctr.Execute();
        }

        public void CreateTreeGroupMappings()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var ctr = new CreateTreeGroupMappings(persistedCacheRepository, _outputHandler);

            ctr.Execute();

        }

        #endregion
        
        public InfoModel GetInfo() 
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            
            var tp = new GetInfoService(persistedCacheRepository, _outputHandler, _mapper);

            return tp.Execute();
        }

        public PlaceInfoModel GetPlaceInfo()
        {
            var ftmPlaceCacheRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var tp = new GetPlaceInfoService(ftmPlaceCacheRepository, _outputHandler);

            return tp.Execute();
        }
    }
}