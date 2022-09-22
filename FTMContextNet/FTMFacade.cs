using System;
using System.Collections.Generic;
using ConfigHelper;
using FTMContext;
using LoggingLib;
using FTMContextNet.Data;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Application.Services;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Application.Mapping;
using AutoMapper;
using PlaceLib.Model;

namespace FTMContextNet
{

    public class FTMFacade
    {
        private readonly PersistedCacheRepository _persistedCacheRepository;
        private readonly PlacesRepository _placesRepository;
        private readonly FTMMakerRepository _ftMakerRepository;
        private readonly InMemoryCacheRepository _inMemoryCacheRepository;
        private readonly Ilog _outputHandler;
        private readonly IMapper _mapper;

        public FTMFacade(IMSGConfigHelper iMSGConfigHelper, Ilog iLog)
        {
            //the source db is created by the adapter
            //the purpose of the facade is to first populate the persisted cache
            //then update the azure db.

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfiguration());
            });

            var persistedCacheContext = PersistedCacheContext.Create(iMSGConfigHelper);

            var ftMakerContext = FTMakerContext.CreateSourceDB(iMSGConfigHelper);

            _inMemoryCacheRepository = new InMemoryCacheRepository(InMemoryCacheContext.Create(ftMakerContext, persistedCacheContext.FTMPlaceCache, persistedCacheContext.FTMPersonOrigins, iLog), iLog);

            _persistedCacheRepository = new PersistedCacheRepository(persistedCacheContext, iLog);

            _ftMakerRepository = new FTMMakerRepository(ftMakerContext);

            _mapper = config.CreateMapper();

            _placesRepository = new PlacesRepository(new PlacesContext(iMSGConfigHelper));

            _outputHandler = iLog;

            _outputHandler.WriteLine("Service Cache Created");
        }
        public IEnumerable<PlaceModel> GetUnknownPlaces(int count)
        {
            var service = new GetUnknownPlacesService(_persistedCacheRepository, _outputHandler, _mapper);

            return service.Execute(count);
        }
        public IEnumerable<PlaceModel> GetPlaceNotGeocoded(int amount) {
            
            var service = new GetUnknownPlacesSearchedAlreadyServices(_persistedCacheRepository, _outputHandler, _mapper);
            return service.Execute(amount);
        }

        public void ClearData()
        {
            throw new NotImplementedException();
        }

        public void UpdatePlaceGeoData(PlaceLookup value)
        {
            var service = new UpdatePlaceGeoData(_persistedCacheRepository, _outputHandler, _mapper);

            service.Execute();
        }
         
        /// <summary>
        /// Add unknown places and reset locations for any place ids that have changed.
        /// </summary>
        public void AddUnknownPlaces()
        {
            var service = new UpdatePlaceCache(_persistedCacheRepository, _outputHandler, _mapper);

            service.Execute(_ftMakerRepository.GetPlaces());
        }

        public void UpdatePlaceMetaData()
        {
            var service = new UpdatePlaceMetaData(_persistedCacheRepository, _placesRepository, _outputHandler, _mapper);

            service.Execute();
        }

        public void AssignTreeNamesToPersons()
        {
            var treeStartPerson = UpdateTreePersonOrigins.Create(_ftMakerRepository, _persistedCacheRepository, _outputHandler);

            treeStartPerson.Execute();

            _outputHandler.WriteLine("Finished Setting Origin Person");
        }

        public void ImportPersons()
        {

            var createPersonsAndMarriages = new CreatePersonsAndMarriages(_persistedCacheRepository, _inMemoryCacheRepository, _outputHandler);

            createPersonsAndMarriages.Execute();
        }

        public void CreateDupeView()
        {
            var createDupeEntrys = new CreateDupeEntrys(_persistedCacheRepository, _outputHandler);

            createDupeEntrys.Execute();

            _outputHandler.WriteLine("Finished Creating Dupe View");
        }

        public void CreateTreeRecord()
        {
            //    FTMTreeRecords.Create(sourceDB, cacheDB, outputHandler).Delete().SavePersons();

            var ctr = new CreateTreeRecords(_persistedCacheRepository, _outputHandler);

            ctr.Execute();

            _outputHandler.WriteLine("Finished Creating Tree Record View");
        }

        public InfoModel GetInfo() {

            var tp = new GetInfoService(_persistedCacheRepository, _outputHandler, _mapper);

            return tp.Execute();
        }
    }


}

