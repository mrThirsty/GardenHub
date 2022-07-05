using FluentValidation;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public class SensorDataService : DataService<Sensor, ISensorDataRepository>, ISensorDataService
{
    public SensorDataService(IValidator<Sensor> validator, ISensorDataRepository repo) : base(validator, repo)
    {
        
    }

    public async Task<Sensor?> GetSensorByName(string name)
    {
        var sensors = await Repository.GetAllAsync();

        return sensors.Where(s => s.SensorName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault();
    }
}