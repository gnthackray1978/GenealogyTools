using System;
using System.Collections.Generic;

namespace DNAGedLib
{
    public class DNAGedImporter {
        public static void Import()
        {


            Console.WriteLine("Select User to update/import");
            Console.WriteLine("****************************");

            var importDataStore = new ImportDataStore();

            var stages = new List<ImportStage>()
            {
                new SelectProfile(importDataStore),
                new ReadDNAGedComSQLLiteDB(importDataStore),
                new WriteStagePeople(importDataStore),
                new ImportStageTrees(importDataStore),
                new ImportStageICW(importDataStore),
                new ImportStageMatchDetailsWithGroups(importDataStore),
                new ImportStageKitStats(importDataStore),
                new AdjustAncestryURLs(importDataStore)
            };


            foreach (var x in stages)
            {
                var result = x.Import();

                if (result.State == ReturnState.Failure)
                {
                    Console.WriteLine(result.Message);
                    break;
                }
                else if (result.State == ReturnState.Pause)
                {
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine(result.Message);
                }
            };

            Console.WriteLine("Finished");
            Console.ReadKey();
        }


    }






}
