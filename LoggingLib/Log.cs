using System;

namespace LoggingLib
{
    public class Log : Ilog
    {
        public void WriteLine(string line) {
            Console.WriteLine(line);
        }

        public void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public void ProgressSearch(int counter, int total, string message, string tailMessage = "")
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

        public void ProgressUpdate(int counter, int total, string message, string tailMessage = "")
        {
            
            double percentage = ((double)counter / (double)total) * 100;
             
            if (counter < 1)
            {
                Console.WriteLine("");
                Console.WriteLine("UPDATING " + message.Trim() + " " + percentage.ToString("F") + " %   of " + total + " " + tailMessage.Trim());
            }
            else
            {
                if (counter == total)
                {
                    Console.Write("\rUPDATING " + message.Trim() + " " + percentage.ToString("F") + " %   of " + total + " " + tailMessage.Trim());
                    Console.WriteLine("");
                }
                else
                {
                    Console.Write("\rUPDATING " + message.Trim() + " " + percentage.ToString("F") + " %   of " + total + " " + tailMessage.Trim());
                }

            }
        }

        public void StatusReport(string message, bool forceNewLine, bool pause = false)
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

        public void WriteCounter(string message)
        {
            Console.WriteLine(message);
        }
    }



}
