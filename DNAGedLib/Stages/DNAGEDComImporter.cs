using System;
using System.Collections.Generic;

namespace DNAGedLib
{
    public class DNAGEDComImporter {
        public static void Import()
        {


            Console.WriteLine("Select User to update/import");
            Console.WriteLine("****************************");

            var p = new ImportationContext();

            var stages = new List<ImportStage>()
            {
                new SelectProfile(p),
                new ReadDNAGedComSQLLiteDB(p),
                new WriteStagePeople(p),
                new ImportStageTrees(p),
                new ImportStageICW(p),
                new ImportStageMatchDetailsWithGroups(p),
                new ImportStageKitStats(p),
                new AdjustAncestryURLs(p)
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
