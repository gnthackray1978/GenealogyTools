using System;
using DNAGedLib;
using RootsLib;

namespace RootsReader
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("*************************");
            Console.WriteLine("*****Roots Importer!*****");
            Console.WriteLine("*************************");

            RootParser rp = new RootParser();

             rp.Init();

            // LocationUpdater.UpdateLocations();
             
             Console.WriteLine("finished press key to exit");
             Console.ReadKey();

        }

    }
}
