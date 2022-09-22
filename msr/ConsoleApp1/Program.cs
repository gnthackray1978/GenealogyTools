using FTM.Dates;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //1250890755

            uint.TryParse("1250890755", out uint birthDate);

            Date d = Date.CreateInstance(birthDate);

            var tp = d.ToString();

            Console.WriteLine(tp);

            Console.ReadKey();
        }
    }
}
