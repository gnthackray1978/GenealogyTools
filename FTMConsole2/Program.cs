using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using AutoMapper;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Mapping;
using FTMContextNet.Application.UserServices.GetInfoList;
using FTMContextNet.Data;
using MSGIdent;
using LoggingLib;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FTMContextNet.Data.Repositories.TreeAnalysis;

namespace FTMConsole2
{
    class Program
    {
        static void Main()
        {
            var config = new MapperConfiguration(cfg =>
            {

                cfg.AddProfile(new AutoMapperConfiguration());
            });
           //Ilog _outputHandler = new Log();
          //  IMapper _mapper = config.CreateMapper();
        //    IMSGConfigHelper _iMSGConfigHelper = new MSGConfigHelper();

//            var persistedCacheRepository = new PersistedCacheRepository(PersistedCacheContext.Create(_iMSGConfigHelper, _outputHandler), _outputHandler);
           // var auth = ;
            //IPersistedCacheRepository persistedCacheRepository, Ilog outputHandlerp, IMapper iMapper, IAuth auth

            var serviceCollection = new ServiceCollection()
                .AddMediatR(cfg => cfg
                    .RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddSingleton<Ilog>(new Log())
                    .AddSingleton<IMSGConfigHelper>(new MSGConfigHelper())
                    .AddSingleton<IMapper>(config.CreateMapper())
                    .AddSingleton<IAuth>(new Auth())
                    .AddTransient<IPersistedCacheContext, SQLitePersistedCacheContext>()
                    .AddTransient<IPersistedCacheRepository, PersistedCacheRepository>()
                    
                    .BuildServiceProvider();

           // var mediator = serviceCollection.GetRequiredService<IMediator>();


            //builder.Services.AddSingleton<FakeDataStore>();

            var facade = new FTMFacade(new MSGConfigHelper(), new Log());
             
            Console.WriteLine("1. Import Persons");

            Console.WriteLine("2. Update Places");

            Console.WriteLine("3. Debug Option");
            
            Console.WriteLine("q. Quit");
            
            while (true)
            { 
                var input = Console.ReadKey();
                Console.WriteLine("");
                if ((input.KeyChar <49 || input.KeyChar > 57) && input.KeyChar!='q')
                {
                    Console.WriteLine("Not a valid Selection");
                    continue;
                }

                Actions(input.KeyChar, facade, serviceCollection);

                if (input.KeyChar == 'q')
                    break;
            }
        }

        private static void Actions(char sin, FTMFacade facade, ServiceProvider sp )
        {
           
            if (sin == '1')
            {
                facade.ImportSavedGed();
            }
            
            if (sin == '2')
            {
                facade.UpdatePlaceMetaData();
            }

            if (sin == '3')
            {
                var mediator = sp.GetRequiredService<IMediator>();

                var timer = new Stopwatch();
                timer.Start();

                var tp = mediator.Send(new GetInfoServiceQuery(), new CancellationToken(false));

                timer.Stop();


                

                Console.WriteLine(tp.Result.PersonViewCount);

                Console.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));
            }


         
        } 
    }
}
