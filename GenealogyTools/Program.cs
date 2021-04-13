using DNAGedLib;
using PersonDupeLib;
using PlaceLib;
using System;
using System.Collections.Generic;
//
namespace GenealogyTools
{
    class Program2
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("===========================================");
            Console.WriteLine("================MATCH TOOLS ===============");
            Console.WriteLine("===========================================");


            InitialMenu();
        }


        private static void InitialMenu()
        {
            Console.WriteLine("1. Fix Locations");
            Console.WriteLine("2. Import DNAGEDCom Matches");
            Console.WriteLine("3. Group Persons Of Interest");
            Console.WriteLine("4. Rebuild Persons Of Interest Table");
            Console.WriteLine("5. Fix unknown counties");
            Console.WriteLine("6. Import ICW");
            Console.WriteLine("7. Import from GED File");
            Console.WriteLine("8. Check Con");
            Console.WriteLine("9. Quit");


            int sin = 0;

            var input = Console.ReadKey();

            while (!int.TryParse(input.KeyChar.ToString(), out sin) || sin > 9)
            {
                Console.WriteLine("Not a valid Selection");
                input = Console.ReadKey();
            }
            if (sin == 8)
            {
                PDLMethods.CheckCon();

            }

            if (sin == 7)
            {

                GedcomImporter.GedcomImporter.Run(@"C:\Users\GeorgePickworth\Downloads\DNA Match File.ged");
            }

            if (sin == 6)
            {

                var stages = new List<ImportStage>()
                {
                    new ImportStageICW(new ImportationContext()),
                };

                stages.ForEach(f => f.Import());
            }


            if (sin == 5)
            {
                LocationFixer.MapPlaceListToCounties();
                //  PlaceLib.PlaceOperations.FindMissingCounties();
            }

            if (sin == 4)
            {
                PDLMethods.ResetPersonsOfInterest();
            }

            if (sin == 3)
            {
                PDLMethods.CreateGroup();
            }

            //import from dnagedcom db
            if (sin == 2)
            {
                DNAGEDComImporter.Import();

                Console.WriteLine("Finished - press a key");
                Console.ReadKey();
            }

            if (sin == 1)
            {           
                LocationFixer.UpdateLocations();
            }
        }


    }
}
