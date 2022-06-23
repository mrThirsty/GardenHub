using GardenHub.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace GardenHub.Server.Data;

public class GardenHubDbContext : DbContext
{
    public GardenHubDbContext(DbContextOptions<GardenHubDbContext> options) : base(options)
    {
        
    }

    public DbSet<Plant> Plants { get; set; } = default!;
    public DbSet<Pot> Pots { get; set; } = default!;
}