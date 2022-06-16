using GardenHub.Server.Data;
using GardenHub.Server.Endpoints.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GardenHub.Tests.Server;

public class ServerFactory : WebApplicationFactory<IApiMaker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(collection =>
        {
            collection.RemoveAll(typeof(GardenHubDbContext));

            DbContextOptionsBuilder<GardenHubDbContext> builder =
                new DbContextOptionsBuilder<GardenHubDbContext>().UseSqlite("DataSource=GardenHub.Server.Test.db"); //file:inmem?mode=memory&cache=shared");

            GardenHubDbContext context = new GardenHubDbContext(builder.Options);
            
            var pendingMigrations = context.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }

            collection.AddDbContext<GardenHubDbContext>((_, options) => options.UseSqlite("DataSource=GardenHub.Server.Test.db"));
        });
    }
}