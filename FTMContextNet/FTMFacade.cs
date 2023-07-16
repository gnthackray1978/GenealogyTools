using AutoMapper;
using ConfigHelper;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Application.Models.Create;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Application.Services;
using FTMContextNet.Data;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Domain.Auth;
using FTMContextNet.Domain.Entities.NonPersistent;
using LoggingLib;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Application.Models.Write;
using PlaceLibNet.Application.Services;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Data.Repositories;
using QuickGed.Domain;
using QuickGed.Services;
using System;
using System.Collections.Generic;
using System.IO;
using FTMContextNet.Application.Services.GedImport;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Caching;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;

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
        
        public void UpdatePlaceMetaData()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper),_outputHandler);

            var service = new UpdatePlaceMetaData(placeRepository, _outputHandler);

            service.Execute();
        }

        #endregion
        
       
        public void ImportSavedGed()
        {
            //dependencies
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var persistedImportedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);
            var persistedImportCacheRepository = new PersistedImportCacheRepository(new PersistedCacheContext(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var personPlaceCache = new PersonPlaceCache(persistedCacheRepository.MakePlaceRecordCache(persistedImportCacheRepository.GetCurrentImportId()), new PlaceNameFormatter());
            var placeCache = new PlaceLookupCache(placeRepository.GetCachedPlaces(), new PlaceNameFormatter());
            var placeLibCoordCache = new PlaceLibCoordCache(placeRepository, new PlaceNameFormatter());
            var gr = new GedRepository(_outputHandler, new GedParser(new NodeTypeCalculator(), Path.Combine(_iMSGConfigHelper.GedPath, persistedImportedCacheRepository.GedFileName())));


            //_facade.ImportPersons();
            var createPersonsAndMarriages = new CreatePersonsAndMarriages(persistedCacheRepository, persistedImportedCacheRepository, gr, new Auth(), _outputHandler);

            createPersonsAndMarriages.Execute();

            //_facade.CreateDupeView();
            var createDupeEntrys = new CreateDupeEntrys(persistedCacheRepository, persistedImportedCacheRepository, new Auth(), _outputHandler);

            createDupeEntrys.Execute();

            //_facade.CreateTreeRecord();
            var ctr = new CreateTreeRecords(persistedCacheRepository, persistedImportedCacheRepository, new Auth(), _outputHandler);
            ctr.Execute();

            //_facade.CreateTreeGroups();
            var ctr2 = new CreateTreeGroups(persistedCacheRepository, persistedImportedCacheRepository, new Auth(), _outputHandler);
            ctr2.Execute();

            //_facade.CreateTreeGroupMappings();
            var ctr3 = new CreateTreeGroupMappings(persistedCacheRepository, persistedImportedCacheRepository, new Auth(), _outputHandler);

            ctr3.Execute().Wait();
            
            //_facade.CreateMissingPersonLocations();
           
            var service = new CreatePersonLocationsInCache(placeRepository,
                placeLibCoordCache,
                personPlaceCache,
                placeCache,
                new Auth(),
                _outputHandler);

            service.Execute();

        }
        
        
        public InfoModel GetInfo() 
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var auth = new Auth();

            var tp = new GetInfoService(persistedCacheRepository, _outputHandler, _mapper,auth);

            return tp.Execute();
        }

        public IEnumerable<ImportModel> ReadImports()
        {
            var persistedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var tp = new GetGedFiles(persistedCacheRepository, _outputHandler, _mapper);

            return tp.Execute();
        }

        public APIResult CreateImport(CreateImportModel createImportModel)
        {
            var persistedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
 
            var tp = new CreateImport(persistedCacheRepository, _outputHandler, new Auth());

            return tp.Execute(createImportModel);
        }

        public APIResult SelectImport(int importId)
        {
            var persistedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var tp = new SelectImport(persistedCacheRepository, _outputHandler, new Auth());

            return tp.Execute(importId);
        }

        public APIResult DeleteImport(int importId)
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
          
            var persistedImportedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
          
            var auth = new Auth();

            var gr = new GedRepository(_outputHandler,
                new GedParser(new NodeTypeCalculator(),
                    Path.Combine(_iMSGConfigHelper.GedPath, 
                        persistedImportedCacheRepository.GedFileName())));

            var d = new DeleteImportService(persistedCacheRepository, persistedImportedCacheRepository, gr, auth,
                _outputHandler);

            d.Execute();

            var tp = new DeleteImport(persistedImportedCacheRepository, _outputHandler, new Auth());

            return tp.Execute(importId);
        }

        public PlaceInfoModel GetPlaceInfo()
        {
            var ftmPlaceCacheRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var tp = new GetPlaceInfoService(ftmPlaceCacheRepository, _outputHandler);

            return tp.Execute();
        }
    }
}