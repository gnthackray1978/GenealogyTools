using System;
using System.Reflection;
using AutoMapper;
using ConfigHelper;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Data;
using FTMContextNet.Data.Repositories;
using FTMContextNet.Data.Repositories.GedImports;
using MSGIdent;
using GenDataAPI.Hub;
using LoggingLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlaceLibNet.Data.Contexts;
using PlaceLibNet.Domain.Caching;

namespace GenDataAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var msgConfigHelper = new MSGConfigHelper();

            var config = new MapperConfiguration(cfg =>
            {

                cfg.AddProfile(new AutoMapperConfiguration());
            });

            services.AddSingleton<IMSGConfigHelper>(msgConfigHelper)
                    .AddMediatR(cfg => cfg
                    .RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                    .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                    .AddSingleton<Ilog>(new Log())
                    .AddSingleton<IMapper>(config.CreateMapper())
                    .AddSingleton<IAuth>(new Auth())
                    .AddTransient<IPlacesContext,PlacesContext>()
                    .AddTransient<IPersistedCacheContext, PersistedCacheContext>()
                    .AddTransient<IPersistedImportCacheRepository, PersistedImportCacheRepository>()
                    .AddTransient<IPersistedCacheRepository, PersistedCacheRepository>()
                    .AddTransient<IPlaceLibCoordCache, PlaceLibCoordCache>()
                    .AddTransient<IPlaceLookupCache,PlaceLookupCache>();
            
            //PersistedImportCacheRepository
            services.AddControllers();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<NotificationHub>("notificationhub");
            }); 
        }
    }
}
