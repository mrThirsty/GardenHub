using GardenHub.Server;
using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(1883, l => l.UseMqtt());
//     options.ListenAnyIP(5000);
// });

builder.RegisterServices();

var app = builder.Build();

app.ConfigureApplication();

app.Run();