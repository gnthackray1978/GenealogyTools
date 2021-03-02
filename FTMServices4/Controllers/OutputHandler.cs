﻿using ConsoleTools;
using Microsoft.AspNetCore.SignalR;

namespace FTMServices4.Controllers
{
    public class OutputHandler : IConsoleWrapper
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
            _hubContext.Clients.All.SendAsync("Notify", line);
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

        public void WriteCounter(string message)
        {
            _hubContext.Clients.All.SendAsync("Update", message);
        }
    }
}
