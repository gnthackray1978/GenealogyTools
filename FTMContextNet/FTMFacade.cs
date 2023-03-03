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
using PlaceLib.Model;
using FTMContextNet.Domain.Entities.NonPersistent.Locations;

namespace FTMContextNet
{

    public class FTMFacade
    {

        private  Ilog _outputHandler;
        private  IMapper _mapper;
        private IMSGConfigHelper _iMSGConfigHelper;

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
        public IEnumerable<PlaceModel> GetUnknownPlaces(int count)
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);


            var service = new GetUnknownPlacesService(persistedCacheRepository, _outputHandler, _mapper);

            return service.Execute(count);
        }
        public IEnumerable<PlaceModel> GetPlaceNotGeocoded(int amount) {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var service = new GetUnknownPlacesSearchedAlreadyServices(persistedCacheRepository, _outputHandler, _mapper);
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
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var service = new UpdatePlaceGeoData(persistedCacheRepository, _outputHandler, _mapper);

            service.Execute(value);
        }
         
        /// <summary>
        /// Add unknown places and reset locations for any place ids that have changed.
        /// </summary>
        public void AddUnknownPlaces()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ftMakerContext = FTMakerContext.CreateSourceDB(_iMSGConfigHelper);

            var ftMakerRepository = new FTMMakerRepository(ftMakerContext);

            var service = new UpdatePlaceCache(persistedCacheRepository, _outputHandler, _mapper);

            service.Execute(ftMakerRepository.GetPlaces());
        }

        public void UpdatePlaceMetaData()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var placesRepository = new PlacesRepository(new PlacesContext(_iMSGConfigHelper));

            var service = new UpdatePlaceMetaData(persistedCacheRepository, placesRepository, _outputHandler, _mapper);

            service.Execute();
        }


        #region tree data

        public void AssignTreeNamesToPersons()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ftMakerContext = FTMakerContext.CreateSourceDB(_iMSGConfigHelper);

            var ftMakerRepository = new FTMMakerRepository(ftMakerContext);

            var treeStartPerson = UpdateTreePersonOrigins.Create(ftMakerRepository, persistedCacheRepository, _outputHandler);

            treeStartPerson.Execute();

            _outputHandler.WriteLine("Finished Setting Origin Person");
        }

        public void ImportPersons()
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var persistedCacheContext = PersistedCacheContext.Create(_iMSGConfigHelper);

            var ftMakerContext = FTMakerContext.CreateSourceDB(_iMSGConfigHelper);

            var inMemoryCacheRepository = new InMemoryCacheRepository(InMemoryCacheContext.Create(ftMakerContext, persistedCacheContext.FTMPlaceCache, persistedCacheContext.FTMPersonOrigins, _outputHandler), _outputHandler);

            var createPersonsAndMarriages = new CreatePersonsAndMarriages(persistedCacheRepository, inMemoryCacheRepository, _outputHandler);

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
            var ftMakerContext = FTMakerContext.CreateSourceDB(_iMSGConfigHelper);

            var ftMakerRepository = new FTMMakerRepository(ftMakerContext);

            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ctr = new CreateTreeRecords(persistedCacheRepository, ftMakerRepository, _outputHandler);

            ctr.Execute();

            _outputHandler.WriteLine("Finished Creating Tree Record View");
        }

        public void CreateTreeGroups()
        {
            var ftMakerContext = FTMakerContext.CreateSourceDB(_iMSGConfigHelper);

            var ftMakerRepository = new FTMMakerRepository(ftMakerContext);

            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ctr = new CreateTreeGroups(persistedCacheRepository, ftMakerRepository, _outputHandler);

            ctr.Execute();
             
        }

        public void CreateTreeGroupMappings()
        {
            var ftMakerContext = FTMakerContext.CreateSourceDB(_iMSGConfigHelper);

            var ftMakerRepository = new FTMMakerRepository(ftMakerContext);

            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var ctr = new CreateTreeGroupMappings(persistedCacheRepository, ftMakerRepository, _outputHandler);

            ctr.Execute();

        }

        #endregion


        public InfoModel GetInfo() {
                   
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper), _outputHandler);

            var tp = new GetInfoService(persistedCacheRepository, _outputHandler, _mapper);

            return tp.Execute();
        }
    }


}

