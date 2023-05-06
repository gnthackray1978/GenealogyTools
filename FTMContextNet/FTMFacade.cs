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
using FTMContextNet.Application.Models.Write;
using PlaceLibNet;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Data.Repositories;

namespace FTMContextNet
{

    public class FTMFacade
    {

        private Ilog _outputHandler;
        private IMapper _mapper;
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

        public IEnumerable<PlaceModel> GetUnknownPlaces(int count)
        {
            //var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var placeRepository = new FtmPlaceCacheRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);


            var service = new GetUnknownPlacesService(placeRepository, _outputHandler, _mapper);

            return service.Execute(count);
        }
        public IEnumerable<PlaceModel> GetPlaceNotGeocoded(int amount) {
            //var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var placeRepository = new FtmPlaceCacheRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);


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
        public void UpdatePlaceGeoData(GeoCodeResultModel value)
        {
           // var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var placeRepository = new FtmPlaceCacheRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);


            var service = new UpdatePlaceGeoData(placeRepository, _outputHandler, _mapper);

            service.Execute(value);
        }
         
        /// <summary>
        /// look in GED add any places to the cache that aren't already in there.
        /// </summary>
        public void AddUnknownPlaces()
        {
            var ftmPlaceCacheRepository = new FtmPlaceCacheRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var pr2 = new PlacesRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);
        
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            
            var pr = new PlaceRecord(ftmPlaceCacheRepository, pr2, persistedCacheRepository, _outputHandler);

            pr.Process();

        }

        public void UpdatePlaceMetaData()
        {
            var placesRepository = new PlacesRepository(new PlacesContext(_iMSGConfigHelper),_outputHandler);

            var placeRepository = new FtmPlaceCacheRepository(new PlacesContext(_iMSGConfigHelper),_outputHandler);

            var service = new UpdatePlaceMetaData(placeRepository, placesRepository, _outputHandler, _mapper);

            service.Execute();
        }

        #endregion


        #region tree data

        public void ImportPersons()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var gr = new GedRepository(_iMSGConfigHelper.GedPath, _outputHandler);

            var createPersonsAndMarriages = new CreatePersonsAndMarriages(persistedCacheRepository, gr, _outputHandler);

            createPersonsAndMarriages.Execute();
        }

        public void CreateDupeView()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var createDupeEntrys = new CreateDupeEntrys(persistedCacheRepository, _outputHandler);

            createDupeEntrys.Execute();

            _outputHandler.WriteLine("Finished Creating Dupe View");
        }

        public void CreateTreeRecord()
        {
            
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ctr = new CreateTreeRecords(persistedCacheRepository, _outputHandler);

            ctr.Execute();

            _outputHandler.WriteLine("Finished Creating Tree Record View");
        }

        public void CreateTreeGroups()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ctr = new CreateTreeGroups(persistedCacheRepository, _outputHandler);

            ctr.Execute();
             
        }

        public void CreateTreeGroupMappings()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ctr = new CreateTreeGroupMappings(persistedCacheRepository, _outputHandler);

            ctr.Execute();

        }

        #endregion


        public InfoModel GetInfo() {
                   
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ftmPlaceCacheRepository = new FtmPlaceCacheRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var tp = new GetInfoService(persistedCacheRepository, ftmPlaceCacheRepository, _outputHandler, _mapper);

            return tp.Execute();
        }
    }


}

