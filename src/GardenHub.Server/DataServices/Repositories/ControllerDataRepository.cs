using GardenHub.Server.Data;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace GardenHub.Server.DataServices.Repositories;

public class ControllerDataRepository : DataRepository<SensorController>, IControllerDataRepository
{
    public ControllerDataRepository(GardenHubDbContext context) : base(context)
    {
        
    }

    public async Task<SensorController?> FindByControllerId(string controllerId)
    {
        var items = Context.Set<SensorController>().Where(c =>
            c.ControllerId.Equals(controllerId));

        return await items.FirstOrDefaultAsync();
    }
}