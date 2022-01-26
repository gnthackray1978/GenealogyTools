using AzureContext.Models;
using FTMContext;
using FTMContext.Models;
using System;
using System.Linq; 
using ConfigHelper;
using FTMContextNet;
using LoggingLib;

namespace FTMConsole2
{
    
    class Program
    {
        static void Main(string[] args)
        {
            IMSGConfigHelper imsgConfigHelper = new MSGConfigHelper();

           
            var facade = new FTMFacade(imsgConfigHelper, new Log());


            Console.WriteLine("1. Update FTM DBs");

            Console.WriteLine("2. Update FTMCache counties and countries");

            Console.WriteLine("3. Update Birth and Location facts");

            Console.WriteLine("4. Mark Common Ancestors");

            Console.WriteLine("5. Run Grouping");

            Console.WriteLine("6. Debug Option");
            
            Console.WriteLine("7. Quit");




            int sin = 0;

            var input = Console.ReadKey();

            while (!int.TryParse(input.KeyChar.ToString(), out sin) || sin > 7 || sin == 0)
            {
                Console.WriteLine("Not a valid Selection");
                input = Console.ReadKey();
            }

            if (sin == 1)
            {
                facade.UpdateMissingPlaces();
                Console.ReadKey();
            }

            if (sin == 2)
            {
                facade.UpdatePlaceMetaData();
                Console.ReadKey();
            }


            if (sin == 3)
            {
                facade.SetDateLocPop();
                Console.ReadKey();
            }

            if (sin == 4)
            {
                facade.SetOriginPerson();
                Console.ReadKey();
            }

            if (sin == 5)
            {
                facade.CreateDupeView();
            }

            if (sin == 6)
            {

                AzureDBContext ac = new AzureDBContext(imsgConfigHelper.MSGGenDB01);

                var gb = new GraphBuilder(ac);

                gb.GenerateDescendantGraph();
                
                 
                Console.ReadKey();
            }

            

        }

    }
}
