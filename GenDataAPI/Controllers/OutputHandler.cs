
using GenDataAPI.Hub;
using LoggingLib;
using Microsoft.AspNetCore.SignalR;
using System;

namespace GenDataAPI.Controllers
{
    public class OutputHandler : Ilog
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        public OutputHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public OutputHandler() { 
        
        }

        public void WriteLine(string line)
        {
            if (_hubContext == null)
                throw new Exception("Hub context can't be null");

            _hubContext.Clients.All.SendAsync("Notify", line);
        }

        public void ClearCurrentConsoleLine()
        {
           
        }

        public void ProgressSearch(int counter, int total, string message, string tailMessage = "")
        {
            
        }

        public void ProgressUpdate(int counter, int total, string message, string tailMessage = "")
        {
          

        }

        public void StatusReport(string message, bool forceNewLine, bool pause = false)
        {
           
        }

        public void WriteCounter(string message)
        {
            _hubContext.Clients.All.SendAsync("Update", message);
        }
    }
}
