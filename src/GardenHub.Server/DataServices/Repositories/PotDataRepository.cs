using GardenHub.Server.Data;
using GardenHub.Server.Data.Model;
using GardenHub.Server.DataServices.Internal;

namespace GardenHub.Server.DataServices.Repositories;

public class PotDataRepository : DataRepository<Pot>, IPotDataRepository
{
    public PotDataRepository(GardenHubDbContext context) : base(context)
    {
        
    }
}