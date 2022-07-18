using GardenHub.Monitor;
using Microsoft.Extensions.Configuration;
using System.Text;
using CommandLine;
using GardenHub.Monitor.Framework;
using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Events;
using GardenHub.Monitor.Framework.Interfaces;
using GardenHub.Shared.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// dotnet publish -o /Users/Andrew/Sandbox/Releases/GardenHub/Monitor -r linux-arm64 --sc -c Release

Console.WriteLine("Starting up GardenHub monitor unit.");
Console.WriteLine("Loading monitor configuration...");

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false);
    
IConfiguration appConfig = configBuilder.Build();

MonitorConfig monitorConfig = new(); //appConfig);
await monitorConfig.LoadSetup();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<IConfiguration>(appConfig);
        services.AddSingleton(monitorConfig);

        services.AddSingleton<ISoilMoistureSensorManager, SoilMoistureSensorManager>();
        services.AddSingleton<ITemperatureSensorManager, TemperatureSensorManager>();
        services.AddSingleton<IGardenHubClient, GardenHubMQTTClient>();
        services.AddSingleton<IEventManager, EventAggregator>();
    })
    .ConfigureLogging((_, logging) =>
    {
        logging.AddConsole();
    }).Build();

var optionsResult = CommandLine.Parser.Default.ParseArguments<MonitorOptions>(args);

MonitorManager manager = new(host.Services);

switch (optionsResult.Tag)
{
    case ParserResultType.Parsed:
        manager.RunOptions(optionsResult.Value);
        break;
    case ParserResultType.NotParsed:
        manager.HandleParseError(optionsResult.Errors);
        break;
}

Console.WriteLine("");
Console.WriteLine("Press any key to exit");
Console.ReadKey();