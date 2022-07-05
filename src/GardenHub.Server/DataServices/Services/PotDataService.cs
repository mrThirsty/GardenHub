using FluentValidation;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public class PotDataService : DataService<Pot, IPotDataRepository>, IPotDataService
{
    public PotDataService(IValidator<Pot> validator, IPotDataRepository repo) : base(validator, repo)
    {
        
    }

    public async Task<Pot?> FindPotBySensorId(Guid sensorId)
    {
        var pots = await Repository.GetAllAsync();

        return pots.Where(p => p.SensorId == sensorId).FirstOrDefault();
    }
}