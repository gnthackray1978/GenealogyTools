using System;
using ConfigHelper;
using FTMContextNet;
using LoggingLib;

namespace FTMConsole2
{
    
    class Program
    {
        static void Main()
        {
            IMSGConfigHelper imsgConfigHelper = new MSGConfigHelper();

           
            var facade = new FTMFacade(imsgConfigHelper, new Log());


            Console.WriteLine("1. Add Unknown Places");

            Console.WriteLine("2. Update Places");

            Console.WriteLine("3. Import Persons");

            Console.WriteLine("4. Create Tree Records");

            Console.WriteLine("5. Create Tree Groups");

            Console.WriteLine("6. Create Tree Group Mappings");

            Console.WriteLine("7. Create Dupes");

            Console.WriteLine("8. Debug Option");
            
            Console.WriteLine("9. Quit");
            
            while (true)
            {

                var input = Console.ReadKey();
                Console.WriteLine("");
                if (!int.TryParse(input.KeyChar.ToString(),out int sin))
                {
                    Console.WriteLine("Not a valid Selection");
                    continue;
                }

                Actions(sin, facade);

                if (sin > 8 || sin == 0)
                    break;
            }

            
        }

        private static void Actions(int sin, FTMFacade facade)
        {
            if (sin == 1)
            {
                facade.AddUnknownPlaces();
            }

            if (sin == 2)
            {
                facade.UpdatePlaceMetaData();
            }


            if (sin == 3)
            {
                facade.ImportPersons();
            }

            if (sin == 4)
            {
                facade.CreateTreeRecord();
            }

            if (sin == 5)
            {
                facade.CreateTreeGroups();
            }

            if (sin == 6)
            {
                facade.CreateTreeGroupMappings();
            }

            if (sin == 7)
            {
                facade.CreateDupeView();
            }

            if (sin == 8)
            {
                Console.WriteLine("Debug test");
            }
        }
    }
}
