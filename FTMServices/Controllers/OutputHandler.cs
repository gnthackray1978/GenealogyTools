using ConsoleTools;

namespace FTMServices.Controllers
{
    public class OutputHandler : IConsoleWrapper
    {
        public void WriteLine(string line)
        {
            Global.LogMessage(line);
        }

        public void ClearCurrentConsoleLine()
        {
           
        }

        public void ProgressSearch(double counter, double total, string message, string tailMessage = "")
        {
            
        }

        public void ProgressUpdate(double counter, double total, string message, string tailMessage = "")
        {
          

        }

        public void StatusReport(string message, bool forceNewLine, bool pause = false)
        {
           
        }


    }
}
