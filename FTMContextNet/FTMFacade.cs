using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigHelper;
using FTMContext;
using FTMContext.Models;
using LoggingLib;

namespace FTMContextNet
{
    public class FTMFacade
    {
        private FTMakerCacheContext cacheDB;
        private FTMakerContext sourceDB;
        private Ilog outputHandler;

        public FTMFacade(IMSGConfigHelper iMSGConfigHelper, Ilog outputHandlerp)
        {
            cacheDB = FTMakerCacheContext.CreateCacheDB(iMSGConfigHelper);
            sourceDB = FTMakerContext.CreateSourceDB(iMSGConfigHelper);
            outputHandler = outputHandlerp;
        }
        public IEnumerable<PlaceLookup> GetUnknownPlaces(int count)
        {
            return FTMGeoCoding.GetUnknownPlaces(cacheDB, outputHandler).Take(count);
        }

        public void UpdateMissingPlaces()
        {
         
            var sourcePlaces = sourceDB.Place.ToList();


            outputHandler.WriteLine("Adding missing places");
            
            FTMGeoCoding.AddMissingPlaces(sourcePlaces, cacheDB, outputHandler);

            outputHandler.WriteLine("Updating places where required");

            FTMGeoCoding.ResetUpdatedPlaces(sourcePlaces, cacheDB, outputHandler);

            outputHandler.WriteLine("Finished Updating Place Names");


        }

        public void UpdatePlaceMetaData()
        {

            FTMGeoCoding.UpdateFTMCacheMetaData(cacheDB, outputHandler);

            outputHandler.WriteLine("Finished setting counties and countries in FTMPlaceCache table");

        }

        public void ClearData()
        {
            outputHandler.WriteLine("Clearing existing data");

            cacheDB.DeleteTempData();

            outputHandler.WriteLine("Finished Deleting data");
        }

        public void SetOriginPerson()
        {

            var ftmMostRecentAncestor = new FTMMostRecentAncestor(sourceDB, cacheDB, outputHandler);

            ftmMostRecentAncestor.MarkMostRecentAncestor();

            outputHandler.WriteLine("Finished Setting Origin Person");
        }


        public void SetDateLocPop()
        {
            var ftmDupe = new FTMViewCreator(sourceDB, cacheDB, outputHandler);

            ftmDupe.Run();

            outputHandler.WriteLine("Finished Setting Date Loc Pop");
        }

        public void CreateDupeView()
        {
            var pg = new PersonGrouper(sourceDB, cacheDB, outputHandler);

            pg.PopulateDupeEntries();

            outputHandler.WriteLine("Finished Creating Dupe View");
        }

        public void CreateTreeRecord()
        {
            FTMTreeRecordCreator ftmTreeRecordCreator = new FTMTreeRecordCreator(sourceDB, cacheDB, outputHandler);

            ftmTreeRecordCreator.Create();

            outputHandler.WriteLine("Finished Creating Tree Record View");
        }

        
    }


}

