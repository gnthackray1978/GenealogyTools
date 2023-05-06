namespace LoggingLib
{
    public interface Ilog
    {

        void WriteCounter(string message);
        void WriteLine(string line);
        void ClearCurrentConsoleLine();
        void ProgressSearch(int counter, int total, string message, string tailMessage = "");
        void ProgressUpdate(int counter, int total, string message, string tailMessage = "");
        void StatusReport(string message, bool forceNewLine, bool pause = false);
    }
}