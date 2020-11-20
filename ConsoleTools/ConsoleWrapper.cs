using System;

namespace ConsoleTools
{

    public class ConsoleWrapper
    {
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void ProgressSearch(double counter, double total, string message, string tailMessage = "")
        {
            double percentage = 0.0;

            percentage = counter / total * 100;

            if (counter < 1)
            {
                Console.WriteLine("");
                Console.WriteLine("SEARCHING " + message.Trim() + " " + percentage + " %   of " + total + " " +
                              tailMessage.Trim());
            }
            else
            {
                Console.Write("\rSEARCHING " + message.Trim() + " " + percentage + " %   of " + total + " " +
                              tailMessage.Trim());
            }

        }

        public static void ProgressUpdate(double counter, double total, string message, string tailMessage = "")
        {
            double percentage = 0.0;

            percentage = counter / total * 100;

            //  Console.SetCursorPosition(0, Console.CursorTop - 1);

            // ClearCurrentConsoleLine();
            if (counter < 1)
            {
                Console.WriteLine("");
                Console.WriteLine("UPDATING " + message.Trim() + " " + percentage + " %   of " + total + " " + tailMessage.Trim());
            }
            else
            {
                Console.Write("\rUPDATING " + message.Trim() + " " + percentage + " %   of " + total + " " + tailMessage.Trim());

            }


        }

        public static void StatusReport(string message, bool forceNewLine, bool pause = false)
        {
            //   Console.WriteLine("");
            if (forceNewLine)
                Console.WriteLine("");

            Console.WriteLine(message);

            if (pause)
            {
                Console.WriteLine("press any key to continue");
                Console.ReadKey();
            }
        }
 

    }

}
