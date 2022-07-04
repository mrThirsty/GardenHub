using System.Text;
using FluentValidation;
using GardenHub.Server;
using GardenHub.Server.Data;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Server.MQTT;
using GardenHub.Shared.Model.Internal;
using Microsoft.EntityFrameworkCore;
using MQTTnet.AspNetCore;

namespace GardenHub.Server;

public static class ProgramStartup
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSqlite<GardenHubDbContext>(connectionString: "Data Source=gardenhub.server.db;");
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AnyOrigin", x =>
            {
                x.AllowAnyOrigin();
                x.AllowAnyHeader();
            });
        });
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddValidatorsFromAssemblyContaining<EntityBase>();

        builder.Services.AddEndpoints<Program>(builder.Configuration);

        builder.Services.AddSingleton<MqttService>();

        builder.Services.AddHostedMqttServerWithServices(mqttOptions =>
        {
            var mqttSvc = mqttOptions.ServiceProvider.GetRequiredService<MqttService>();
            mqttSvc.ConfigureMqttServerOptions(mqttOptions);
            mqttOptions.Build();
        }).AddMqttConnectionHandler().AddConnections();
        
        // builder.Services.AddHostedMqttServer(ms => ms.WithoutDefaultEndpoint())
        //     .AddMqttConnectionHandler()
        //     .AddConnections();
    }

    public static void ConfigureApplication(this WebApplication app)
    {
        app.Logger.LogInformation("Getting DB Context.");
        var db = app.Services.CreateScope().ServiceProvider.GetRequiredService<GardenHubDbContext>();
        app.Logger.LogInformation("Ensuring DB is created");

        var pendingMigrations = db.Database.GetPendingMigrations();

        if (pendingMigrations.Any())
        {
            db.Database.Migrate();

            var lastApplied = (db.Database.GetAppliedMigrations()).Last();
        }
        
        app.UseRouting();
        app.UseCors("AnyOrigin");

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseEndpoints<Program>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapConnectionHandler<MqttConnectionHandler>("/mqtt", options => options.WebSockets.SubProtocolSelector = protocolList => protocolList.FirstOrDefault() ?? string.Empty);
        });

        app.UseMqttServer(server => app.Services.GetRequiredService<MqttService>().ConfigureMqttServer(server));
        // app.UseMqttServer(server =>
        // {
        //     server.ClientConnectedAsync += async (args) =>
        //     {
        //         Console.WriteLine($"MQTT: Client connected: {args.ClientId}");
        //     };
        //     server.ClientDisconnectedAsync += async (args) =>
        //     {
        //         Console.WriteLine($"MQTT: Client disconnected: {args.ClientId}");
        //     };
        //
        //     server.InterceptingPublishAsync += async (args) =>
        //     {
        //         var msg = args.ApplicationMessage;
        //
        //         var topic = msg.Topic;
        //         var payload = Encoding.UTF8.GetString(msg.Payload);
        //         
        //         Console.WriteLine($"New MQTT message: {topic}");
        //         Console.WriteLine($"Payload: {payload}");
        //     };
        // });
    }
}