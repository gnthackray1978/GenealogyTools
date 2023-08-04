using AutoMapper;
using ConfigHelper;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Application.Models.Create;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Data;
using FTMContextNet.Data.Repositories;
using MSGIdent;
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
using System.Threading;
using FTMContextNet.Application.UserServices.CreateDuplicateList;
using FTMContextNet.Application.UserServices.CreateGedImport;
using FTMContextNet.Application.UserServices.CreatePersonLocationsInCache;
using FTMContextNet.Application.UserServices.CreatePersonsAndRelationships;
using FTMContextNet.Application.UserServices.DeleteImport;
using FTMContextNet.Application.UserServices.GetGedList;
using FTMContextNet.Application.UserServices.GetInfoList;
using FTMContextNet.Application.UserServices.UpdateImportStatus;
using FTMContextNet.Application.UserServices.UpdatePersonLocations;
using FTMContextNet.Data.Repositories.GedImports;
using FTMContextNet.Domain.Caching;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using FTMContextNet.Domain.Commands;
using PlaceLibNet.Application.Services.GetPlaceInfoService;
using PlaceLibNet.Application.Services.GetUnknownPlacesSearchedAlreadyService;
using PlaceLibNet.Application.Services.UpdatePlaceCacheNameFormatting;
using PlaceLibNet.Application.Services.UpdatePlaceGeoData;
using PlaceLibNet.Application.Services.UpdatePlaceMetaData;
using PlaceLibNet.Domain.Commands;
using CommandResult = FTMContextNet.Domain.Commands.CommandResult;
using CommandResultType = FTMContextNet.Domain.Commands.CommandResultType;

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
            return service.Handle(new GetUnknownPlacesSearchedAlreadyQuery(amount), new CancellationToken(false)).Result;
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


            var service = new UpdatePlaceGeoData(placeRepository, _outputHandler, _mapper,new Auth());
             
            service.Handle(new UpdatePlaceGeoDataCommand(value.placeid, value.results), new CancellationToken(false)).Wait();
        }

        /// <summary>
        /// Updates PlaceFormatting field of the place cache table
        /// </summary>
        public void UpdatePlaceCachePlaceFormattingEntry()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);
            
            var service = new UpdatePlaceCacheNameFormatting(placeRepository,new PlaceNameFormatter(), _outputHandler, new Auth() );

            service.Handle(new UpdatePlaceCacheCommand(), new CancellationToken(false)).Wait();
        }

        /// <summary>
        /// Updates Persons table with lat and longs.
        /// </summary>
        public CommandResult UpdatePersonLocations()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var service = new UpdatePersonLocations(placeRepository, persistedCacheRepository, _outputHandler, new Auth());

            return service.Handle(new UpdatePersonLocationsCommand(),new CancellationToken(false)).Result;
        }
        
        public void UpdatePlaceMetaData()
        {
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper),_outputHandler);

            var service = new UpdatePlaceMetaData(placeRepository, _outputHandler,new Auth());

            service.Handle(new UpdatePlaceMetaDataCommand(), new CancellationToken(false)).Wait();
        }

        #endregion

      

        public CommandResult ImportSavedGed()
        {
            //dependencies
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var persistedImportedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var placeRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);
            
            var personPlaceCache = new PersonPlaceCache(persistedCacheRepository, persistedImportedCacheRepository, new PlaceNameFormatter());
            var placeCache = new PlaceLookupCache(placeRepository, new PlaceNameFormatter());
            var placeLibCoordCache = new PlaceLibCoordCache(placeRepository, new PlaceNameFormatter());
          
            var gr = new GedRepository(_outputHandler, new GedParser(new NodeTypeCalculator(), 
                Path.Combine(_iMSGConfigHelper.GedPath, persistedImportedCacheRepository.GedFileName())));

            CommandResult CommandResult = null;
            
            var createPersonsAndMarriages = new CreatePersonsAndMarriages(persistedCacheRepository, persistedImportedCacheRepository, gr, new Auth(), _outputHandler);
            CommandResult = createPersonsAndMarriages.Handle(new CreatePersonAndRelationshipsCommand(),new CancellationToken(false)).Result;
            if (CommandResult.CommandResultType != CommandResultType.Success)
                return CommandResult;

            var createDupeEntrys = new CreateDupeEntrys(persistedCacheRepository, persistedImportedCacheRepository, new Auth(), _outputHandler);
            CommandResult = createDupeEntrys.Handle(new CreateDuplicateListCommand(),new CancellationToken(false)).Result;
            if (CommandResult.CommandResultType != CommandResultType.Success)
                return CommandResult;
            
            var service = new CreatePersonLocationsInCache(placeRepository,
                placeLibCoordCache,
                personPlaceCache,
                placeCache,
                new Auth(),
                _outputHandler);
            
            return service.Handle(new CreatePersonLocationsCommand(),new CancellationToken(false)).Result;
        }
        
        
        public InfoModel GetInfo() 
        {
            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
            var auth = new Auth();

            var tp = new GetInfoService(persistedCacheRepository, _outputHandler, _mapper,auth);

            return tp.Handle(new GetInfoServiceQuery(), new CancellationToken(false)).Result;
        }

        public IEnumerable<ImportModel> ReadImports()
        {
            var persistedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var tp = new GetGedFiles(persistedCacheRepository, _outputHandler, _mapper);

            return tp.Handle(new GetGedFilesQuery(), new CancellationToken(false)).Result;
        }

        public CommandResult CreateImport(CreateImportModel createImportModel)
        {
            var persistedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
 
            var tp = new CreateImport(persistedCacheRepository, _outputHandler, new Auth());



            return tp.Handle(new CreateImportCommand(createImportModel.FileName,createImportModel.FileSize, createImportModel.Selected),new CancellationToken(false)).Result;
        }

        public CommandResult SelectImport(int importId)
        {
            var persistedCacheRepository = new PersistedImportCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);

            var tp = new UpdateImportStatus(persistedCacheRepository, _outputHandler, new Auth());

            return tp.Handle(new UpdateImportStatusCommand(importId),new CancellationToken(false)).Result;
        }

        public CommandResult DeleteImport(int importId)
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

            d.Handle(new DeleteTreeCommand(), new CancellationToken(false)).Wait();

            var tp = new DeleteImport(persistedImportedCacheRepository, _outputHandler, new Auth());

            return tp.Handle(new DeleteImportCommand(importId), new CancellationToken(false)).Result;
        }

        public PlaceInfoModel GetPlaceInfo()
        {
            var ftmPlaceCacheRepository = new PlaceRepository(new PlacesContext(_iMSGConfigHelper), _outputHandler);

            var tp = new GetPlaceInfoService(ftmPlaceCacheRepository, _outputHandler);

            return tp.Handle(new GetPlaceInfoQuery(), new CancellationToken(false)).Result;
        }
    }
}