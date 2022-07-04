using GardenHub.Server;
using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

int mqttPort = builder.Configuration.GetValue<int>("Ports:MQTT");
int apiPort = builder.Configuration.GetValue<int>("Ports:API");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(mqttPort, options => options.UseMqtt());
    options.ListenAnyIP(apiPort, options => options.UseHttps());
});

builder.RegisterServices();

var app = builder.Build();

app.ConfigureApplication();

app.Run();