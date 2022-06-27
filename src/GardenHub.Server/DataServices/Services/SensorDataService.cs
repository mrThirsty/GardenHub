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
}