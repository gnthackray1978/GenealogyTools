using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FTMServices.Startup))]

namespace FTMServices
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            
        }
    }
}
