using System.Text;
using CommandLine;
using GardenHub.Monitor.Framework;
using GardenHub.Monitor.Framework.Config;
using GardenHub.Monitor.Framework.Enums;
using GardenHub.Monitor.Framework.Events.Payloads;
using GardenHub.Monitor.Framework.Interfaces;
using GardenHub.Shared.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GardenHub.Monitor;

public class MonitorManager
{
    public MonitorManager(IServiceProvider services)
    {
        IServiceScope scope = services.CreateScope();
        _services = scope.ServiceProvider;
        _logger = _services.GetRequiredService<ILogger<MonitorManager>>();
        _events = _services.GetRequiredService<IEventManager>();
    }
    
    private readonly IServiceProvider _services;
    private readonly ILogger<MonitorManager> _logger;
    private readonly IEventManager _events;

    private bool _restart = false;
    private bool _runApp = true;
    
    public async void RunOptions(MonitorOptions opts)
    {
        try
        {
            _events.GetEvent<MonitorConfigChangedEvent>().Subscribe(ConfigurationChanged);

            MonitorConfig monitorConfig = _services.GetRequiredService<MonitorConfig>();
            
            if (opts.Configure || !monitorConfig.Configured)
            {
                string capturedValue = "";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please enter values for the configuration of the monitor, the current settings are displayed in brackets, hit enter to keep current the current value.");
                
                Console.Write($"Monitor Name ({monitorConfig.MonitorName}): ");
                capturedValue = Console.ReadLine();
                monitorConfig.MonitorName =
                    string.IsNullOrWhiteSpace(capturedValue) ? monitorConfig.MonitorName : capturedValue;
                
                Console.Write($"GardenHub MQTT Address ({monitorConfig.MQTTServer}): ");
                capturedValue = Console.ReadLine();
                monitorConfig.MQTTServer =
                    string.IsNullOrWhiteSpace(capturedValue) ? monitorConfig.MQTTServer : capturedValue;
                
                Console.Write($"MQTT Port ({monitorConfig.MQTTPort}): ");
                capturedValue = Console.ReadLine();
                monitorConfig.MQTTPort = string.IsNullOrWhiteSpace(capturedValue) ? monitorConfig.MQTTPort : Convert.ToInt32(capturedValue);
                
                Console.WriteLine("Please enter the Reporting interval in the following format: days:hours:minutes:seconds, eg: 0:1:0:0 is every hour");

                do
                {
                    Console.Write($"Reporting Interval ({monitorConfig.ReportingInterval}): ");
                    capturedValue = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(capturedValue))
                    {
                        TimeSpan parsedValue;
                        bool parsed = TimeSpan.TryParse(capturedValue, out parsedValue);

                        if (!parsed) capturedValue = string.Empty;
                        else
                        {
                            monitorConfig.ReportingInterval = parsedValue;
                        }
                    }
                    else
                    {
                        capturedValue = monitorConfig.ReportingInterval.ToString();
                    }
                } while (string.IsNullOrWhiteSpace(capturedValue));

                Console.WriteLine("Please enter either 4, 8, or 16 for the controller type.");

                do
                {
                    Console.Write($"controller Type ({monitorConfig.ControllerType}): ");
                    capturedValue = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(capturedValue))
                    {
                        ControllerType parsedValue;
                        bool parsed = Enum.TryParse(capturedValue, out parsedValue);

                        if (!parsed) capturedValue = string.Empty;
                        else
                        {
                            monitorConfig.ControllerType = parsedValue;
                        }
                    }
                    else
                    {
                        capturedValue = monitorConfig.ReportingInterval.ToString();
                    }
                } while (string.IsNullOrWhiteSpace(capturedValue));
                
                monitorConfig.Enabled = true;
                
                monitorConfig.Configured = true;
                monitorConfig.Reconfigure();
                monitorConfig.SetSensorAddresses();
                await monitorConfig.SaveSetup();
            }
            else
            {
                do
                {
                    _restart = false;
                    
                    _logger.LogInformation("Configuring sensors.");
                    ISoilMoistureSensorManager sensorManager = _services.GetRequiredService<ISoilMoistureSensorManager>();//new(monitorConfig);
                    await sensorManager.InitialiseAsync();

                    _logger.LogInformation("Configuring MQTT...");
                    IGardenHubClient gardenClient = _services.GetRequiredService<IGardenHubClient>();
                    //new GardenHubMQTTClient(monitorConfig, sensorManager.GetSensors);
                    await gardenClient.Connect();

                    while (_runApp)
                    {
                        _logger.LogInformation("Reading sensors");
                        var readings = sensorManager.GetReadings();
                        StringBuilder readingLine = new StringBuilder();
        
                        foreach (var reading in readings)
                        {
                            readingLine.Append($"Sensor{reading.Sensor}:{reading.Moisture}    ");

                            var msg = new MoistureReadingMessage(monitorConfig.MonitorName, $"{monitorConfig.MonitorName}-{reading.Sensor}", reading.Moisture);

                            await gardenClient.SendMessage<MoistureReadingMessage>("SoilMoistureReading", msg);
                        }
        
                        _logger.LogInformation(readingLine.ToString());

                        Thread.Sleep(monitorConfig.ReportingInterval);
                    }

                    await gardenClient.Disconnect();
                } while (_restart);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("OOOPS");
            _logger.LogError(ex, "Something went wrong. Try restarting the monitor.");
        }
    }
    
    public void HandleParseError(IEnumerable<Error> errs)
    {
        _logger.LogError("OOOPS");
        _logger.LogError("An error has occurred due to the options selected, review your options and try again");
    }

    private void ConfigurationChanged(ClientReconfigure payload)
    {
        MonitorConfig monitorConfig = _services.GetRequiredService<MonitorConfig>();

        if (payload.ClientId.Equals(monitorConfig.MonitorName))
        {
            monitorConfig.Enabled = payload.Enabled;

            foreach (var sensor in payload.Sensors)
            {
                var configuredSensor = monitorConfig.Sensors.Where(s => s.SensorName.Equals(sensor.SensorId))
                    .FirstOrDefault();

                if (configuredSensor != null)
                {
                    configuredSensor.Enabled = sensor.Enabled;
                }
            }
        }

        monitorConfig.SaveSetup();

        _restart = true;
        _runApp = false;
    }
}