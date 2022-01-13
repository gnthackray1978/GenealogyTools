using System;

namespace LoggingLib
{
    public interface Ilog
    {

        void WriteCounter(string message);
        void WriteLine(string line);
        void ClearCurrentConsoleLine();
        void ProgressSearch(double counter, double total, string message, string tailMessage = "");
        void ProgressUpdate(double counter, double total, string message, string tailMessage = "");
        void StatusReport(string message, bool forceNewLine, bool pause = false);
    }

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

        public void ProgressSearch(double counter, double total, string message, string tailMessage = "")
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

        public void ProgressUpdate(double counter, double total, string message, string tailMessage = "")
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
