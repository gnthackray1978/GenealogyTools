using AzureContext.Models;
using ConsoleTools;
using FTMContext;
using FTMContext.Models;
using System;
using System.Linq;
using Clusterer;
using ConfigHelper;

namespace FTMConsole2
{
    //
    class Program
    {
        static void Main(string[] args)
        {
            IMSGConfigHelper imsgConfigHelper = new MSGConfigHelper();


            Console.WriteLine("1. Update FTM DBs");

            Console.WriteLine("2. Update FTMCache counties and countries");

            Console.WriteLine("3. Update Birth and Location facts");

            Console.WriteLine("4. Mark Common Ancestors");

            Console.WriteLine("5. Run Grouping");

            Console.WriteLine("6. Debug Option");

            Console.WriteLine("7. Clustering");

            Console.WriteLine("8. Quit");




            int sin = 0;

            var input = Console.ReadKey();

            while (!int.TryParse(input.KeyChar.ToString(), out sin) || sin > 8 || sin == 0)
            {
                Console.WriteLine("Not a valid Selection");
                input = Console.ReadKey();
            }

            if (sin == 1)
            {

                var sourceDB = FTMakerContext.CreateSourceDB(imsgConfigHelper);
                var cacheDB = FTMakerCacheContext.CreateCacheDB(imsgConfigHelper);
                var sourcePlaces = sourceDB.Place.ToList();

                Console.WriteLine("Press a key to Add Missing Places");
                Console.ReadKey();

                FTMGeoCoding.AddMissingPlaces(sourcePlaces, FTMakerCacheContext.CreateCacheDB(imsgConfigHelper), new ConsoleWrapper());

                Console.WriteLine("Press a key to Reset Updated Places");
                Console.ReadKey();

                FTMGeoCoding.ResetUpdatedPlaces(sourcePlaces, FTMakerCacheContext.CreateCacheDB(imsgConfigHelper), new ConsoleWrapper());

                // now loadftmservices and geolocate the cache entries 
                // which dont have a json result set.

                // once place ids have been updated
                // set date and location fact to each person in db.

                Console.WriteLine("Finished");
                Console.ReadKey();
            }

            if (sin == 2)
            {
                FTMGeoCoding.UpdateFTMCacheMetaData(FTMakerCacheContext.CreateCacheDB(imsgConfigHelper), new ConsoleWrapper());

                Console.WriteLine("Finished");
                Console.ReadKey();
            }


            if (sin == 3)
            {
                var ftmDupe = new FTMViewCreator(FTMakerContext.CreateSourceDB(imsgConfigHelper), FTMakerCacheContext.CreateCacheDB(imsgConfigHelper), new ConsoleWrapper());

                ftmDupe.Run();

                Console.WriteLine("Finished");
                Console.ReadKey();
            }

            if (sin == 4)
            {
                //var ftmMostRecentAncestor = new FTMMostRecentAncestor(FTMakerContext.CreateSourceDB(), new ConsoleWrapper());

                //ftmMostRecentAncestor.MarkMostRecentAncestor();

                Console.WriteLine("Finished");
                Console.ReadKey();
            }

            if (sin == 5)
            {
                var pg = new PersonGrouper(FTMakerContext.CreateSourceDB(imsgConfigHelper),
                                FTMakerCacheContext.CreateCacheDB(imsgConfigHelper), new ConsoleWrapper());

                pg.PopulateDupeEntries();
            }

            if (sin == 6)
            {

                AzureDBContext ac = new AzureDBContext(imsgConfigHelper.MSGGenDB01Local);


                var c = ac.ParishTranscriptionDetails.ToList();


                AzureDBContext ac2 = new AzureDBContext(imsgConfigHelper.MSGGenDB01);

                foreach (var rec in c)
                {
                    ac2.ParishTranscriptionDetails2.Add(new ParishTranscriptionDetails2()
                    {

                        ParishDataString = rec.ParishDataString,
                        ParishId = rec.ParishId,
                        ParishTranscriptionId = rec.ParishTranscriptionId
                    });
                }

                ac2.SaveChanges();

                Console.WriteLine(c);

                Console.ReadKey();
            }

            if (sin == 7)
            {
                var c = new Cluster();
            }

        }

    }
}
