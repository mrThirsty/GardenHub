using GardenHub.Server.Data;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Repositories;

public class PotDataRepository : DataRepository<Pot>, IPotDataRepository
{
    public PotDataRepository(GardenHubDbContext context) : base(context)
    {
        
    }
}