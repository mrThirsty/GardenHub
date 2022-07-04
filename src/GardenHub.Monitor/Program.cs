using GardenHub.Monitor;
using Microsoft.Extensions.Configuration;
using System.Text;
using GardenHub.Monitor.Framework;
using GardenHub.Shared.Messages;

// dotnet publish -o /Users/Andrew/Sandbox/Releases/GardenHub/Monitor -r linux-arm64 --sc -c Release

Console.WriteLine("Starting up GardenHub monitor unit.");
Console.WriteLine("Loading monitor configuration...");

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false);
    
IConfiguration appConfig = configBuilder.Build();

MonitorConfig monitorConfig = new(appConfig);

Console.WriteLine("Monitor configuration loaded.");

Console.WriteLine("Configuring MQTT...");
IGardenHubClient gardenClient = new GardenHubMQTTClient(monitorConfig);
await gardenClient.Connect();
Console.WriteLine("Loading GPIO controller");

SoilMoistureSensorManager sensorManager = new(monitorConfig);
await sensorManager.InitialiseAsync();

bool runApp = true;

while (runApp)
{
    Console.WriteLine("Reading sensors");
    var readings = sensorManager.GetReadings();
    StringBuilder readingLine = new StringBuilder();
    
    foreach (var reading in readings)
    {
        readingLine.Append($"Sensor{reading.Sensor}:{reading.Moisture}    ");

        var msg = new MoistureReadingMessage(monitorConfig.MonitorName, $"{monitorConfig.MonitorName}-{reading.Sensor}", reading.Moisture);

        await gardenClient.SendMessage<MoistureReadingMessage>("SoilMoistureReading", msg);
    }
    
    Console.WriteLine(readingLine.ToString());

    Thread.Sleep(monitorConfig.ReadingDelay);
}

await gardenClient.Disconnect();