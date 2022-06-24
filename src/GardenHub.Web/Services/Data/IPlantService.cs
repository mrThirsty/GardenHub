using GardenHub.Shared.Model;

namespace GardenHub.Web.Services.Data;

public interface IPlantService
{
    Task<IEnumerable<Plant>> GetPlants();
    Task<bool> AddPlant(Plant item);
    Task<bool> UpdatePlant(Plant item);
    Task<bool> DeletePlant(Guid id);
}