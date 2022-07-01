using GardenHub.Monitor;
using Microsoft.Extensions.Configuration;
using System.Text;
using GardenHub.Monitor.Framework;

Console.WriteLine("Starting up GardenHub monitor unit.");
Console.WriteLine("Loading monitor configuration...");

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false);
    
IConfiguration appConfig = configBuilder.Build();

MonitorConfig monitorConfig = new(appConfig);

Console.WriteLine("Monitor configuration loaded.");

Console.WriteLine("Loading GPIO controller");

SoilMoistureSensorManager sensorManager = new();

bool runApp = true;

while (runApp)
{
    var readings = sensorManager.GetReadings();
    StringBuilder readingLine = new StringBuilder();
    
    foreach (var key in readings.Keys)
    {
        readingLine.Append($"Sensor{key}:{readings[key]}    ");
    }
    
    Console.WriteLine(readingLine.ToString());
}
