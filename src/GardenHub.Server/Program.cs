using GardenHub.Server;
using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// builder.Host.ConfigureWebHostDefaults(host =>
// {
//     host.UseKestrel(k =>
//     {
//         k.ListenAnyIP(1883, l => l.UseMqtt());
//         k.ListenAnyIP(5000);
//     });
// });

ProgramStartup.RegisterServices(builder.Services);

var app = builder.Build();

ProgramStartup.ConfigureApplication(app);

app.MapGet("/", () => "Hello World!");

app.Run();