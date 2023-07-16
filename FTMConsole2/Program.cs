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

            Console.WriteLine("2. Update Places");

            Console.WriteLine("3. Debug Option");
            
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
                facade.ImportSavedGed();
            }
            
            if (sin == '2')
            {
                facade.UpdatePlaceMetaData();
            }

            if (sin == '3')
            {
                facade.UpdatePersonLocations();
               // Console.WriteLine(facade.GetInfo().Unsearched);
            }
        }
    }
}
