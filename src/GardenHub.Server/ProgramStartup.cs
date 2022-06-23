using FluentValidation;
using GardenHub.Server;
using GardenHub.Server.Data;
using GardenHub.Server.Endpoints.Internal;
using GardenHub.Shared.Model.Internal;
using Microsoft.EntityFrameworkCore;

namespace GardenHub.Server;

public static class ProgramStartup
{
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        // services.AddHostedMqttServer(server => server.WithoutDefaultEndpoint())
        //     .AddMqttConnectionHandler()
        //     .AddConnections();

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
        
        builder.Services.AddValidatorsFromAssemblyContaining<IApiMaker>();

        builder.Services.AddEndpoints<Program>(builder.Configuration);
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
        
        // app.UseEndpoints(endpoint =>
        // {
        //     endpoint.MapConnectionHandler<MqttConnectionHandler>("/mqtt",
        //         options => options.WebSockets.SubProtocolSelector =
        //             protocolList => protocolList.FirstOrDefault() ?? string.Empty);
        // });
        //
        // app.UseMqttServer(server =>
        // {
        //     app.ApplicationServices.GetRequiredService<MqttService>().ConfigureServer(server);
        // });
    }
}