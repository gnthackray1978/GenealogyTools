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
           
            var facade = new FTMFacade(new MSGConfigHelper(), new Log());
             
            Console.WriteLine("1. Import Persons");

            Console.WriteLine("2. Create Tree Origins");

            Console.WriteLine("3. Create Tree Records");

            Console.WriteLine("4. Create Tree Groups");
            
            Console.WriteLine("5. Create Tree Group Mappings");

            Console.WriteLine("6. Create Dupes");

            Console.WriteLine("7. Add Unknown Places");

            Console.WriteLine("8. Update Places");

            Console.WriteLine("9. Debug Option");
            
            Console.WriteLine("q. Quit");
            
            while (true)
            { 
                var input = Console.ReadKey();
                Console.WriteLine("");
                if ((input.KeyChar <49 || input.KeyChar > 57) && input.KeyChar!='q')
                {
                    Console.WriteLine("Not a valid Selection");
                    continue;
                }

                Actions(input.KeyChar, facade);

                if (input.KeyChar == 'q')
                    break;
            }

            
        }

        private static void Actions(char sin, FTMFacade facade)
        {
           
            if (sin == '1')
            {
                facade.ImportPersons();
            }

            if (sin == '2')
            {
                facade.CreatePersonOrigins();
            }

            if (sin == '3')
            {
                facade.CreateTreeRecord();
            }

            if (sin == '4')
            {
                facade.CreateTreeGroups();
            }

            if (sin == '5')
            {
                facade.CreateTreeGroupMappings();
            }

            if (sin == '6')
            {
                facade.CreateDupeView();
            }

            if (sin == '7')
            {
                facade.CreateMissingPersonLocations();
            }

            if (sin == '8')
            {
                facade.UpdatePlaceMetaData();
            }

            if (sin == '9')
            {
                Console.WriteLine(facade.GetInfo().Unsearched);
            }
        }
    }
}
