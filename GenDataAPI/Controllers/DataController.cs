﻿using System.Collections.Generic;
using AzureContext;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Models.Read;
using GenDataAPI.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenDataAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        
        private readonly IMSGConfigHelper _iMSGConfigHelper;
        private readonly OutputHandler _outputHandler;
        private readonly FTMFacade _facade;

        public DataController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
        {


            _iMSGConfigHelper = iMSGConfigHelper;
            _outputHandler = new OutputHandler(hubContext);
            _facade = new FTMFacade(_iMSGConfigHelper, _outputHandler);
        }
        
        
        // GET api/values
        public IEnumerable<PlaceModel> Get()
        {
            return _facade.GetUnknownPlaces(75);
        }

        [HttpPost]
        [Route("/data/places")]
        public IActionResult AddToCache()
        {
            _facade.CreateMissingPersonLocations();

            return Ok(true);
        }

        [HttpPut]
        [Route("/data/places")]
        public IActionResult UpdateMeta()
        {
            _facade.UpdatePlaceMetaData();

            return Ok(true);
        }

        [HttpPost]
        [Route("/data/origins")]
        public IActionResult AddOrigins()
        {
            //dont need this now as origins added automatically when tree is read.
            return Ok(true);
        }

        [HttpPost]
        [Route("/data/persons")]
        public IActionResult AddPersons()
        {
            _facade.ImportPersons();

            return Ok(true);
        }

        [HttpPost]
        [Route("/data/dupes")]
        public IActionResult AddDupes()
        {
            _facade.CreateDupeView();

            return Ok(true);
        }

        [HttpPost]
        [Route("/data/trees")]
        public IActionResult AddTrees()
        {
            _facade.CreateTreeRecord();

            return Ok(true);
        }

        [HttpPost]
        [Route("/data/treegroups")]
        public IActionResult AddTreeGroups()
        {
            _facade.CreateTreeGroups();

            return Ok(true);
        }

        [HttpPost]
        [Route("/data/treegroupmappings")]
        public IActionResult AddTreeGroupMappings()
        {
            _facade.CreateTreeGroupMappings();

            return Ok(true);
        }

        [HttpPost]
        [Route("/data/azure")]
        public IActionResult AddAzure()
        {
            var az = new AzureDbImporter(_outputHandler, _iMSGConfigHelper);
            _outputHandler.WriteLine("Importing into azure DB");
            az.Import();
            _outputHandler.WriteLine("Finished importing into azure DB");

            return Ok(true);
        }


        [HttpDelete]
        [Route("/data")]
        public IActionResult Delete()
        {
            _facade.ClearData();
            return Ok(true);
        }
         
    }
}
