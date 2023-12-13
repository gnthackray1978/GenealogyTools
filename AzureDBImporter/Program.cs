// See https://aka.ms/new-console-template for more information
using AzureContext;
using ConfigHelper;
using LoggingLib;

Console.WriteLine("Hello, World!");


var az = new AzureDbImporter(new Log(),new MSGConfigHelper());

az.ImportPlaceCache();