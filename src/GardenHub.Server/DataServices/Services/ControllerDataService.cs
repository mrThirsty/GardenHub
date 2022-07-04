using FluentValidation;
using GardenHub.Server.DataServices.Internal;
using GardenHub.Server.DataServices.Repositories;
using GardenHub.Shared.Model;

namespace GardenHub.Server.DataServices.Services;

public class ControllerDataService : DataService<SensorController, IControllerDataRepository>, IControllerDataService
{
    public ControllerDataService(IValidator<SensorController> validator, IControllerDataRepository repo) : base(validator, repo)
    {
        
    }

    public async Task<SensorController?> FindByControllerId(string controllerId)
    {
        var controller = await Repository.FindByControllerId(controllerId);

        return controller;
    }
}