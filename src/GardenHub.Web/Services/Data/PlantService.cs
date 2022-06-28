using System.Net;
using Ardalis.GuardClauses;
using GardenHub.Shared.Model;
using GardenHub.Web.Data;
using GardenHub.Web.Pages;
using GardenHub.Web.Services.Internal;

namespace GardenHub.Web.Services.Data;

public class PlantService : BaseDataService<PlantService, Plant>, IPlantService
{
    public PlantService(IHttpClientFactory clientFactory, IMessageHandler msgHandler, ILogger<PlantService> logger) : base("plant","PlantService",clientFactory, msgHandler, logger)
    {
    }
    
    // public async Task<IEnumerable<Plant>> GetPlants()
    // {
    //     try
    //     {
    //         HttpRequestMessage request = new(HttpMethod.Get, "plant");
    //         var client = ClientFactory.CreateClient("GardenHub");
    //         var response = await client.SendAsync(request);
    //
    //         if (response.IsSuccessStatusCode)
    //         {
    //             var items = await response.Content.ReadFromJsonAsync<IEnumerable<Plant>>();
    //
    //             return items;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleError(ex, "Error in PlantService.GetPlants",
    //             "Something went wrong trying to get all the plants. Please try again.");
    //     }
    //     
    //     return new List<Plant>();
    // }
    //
    // public async Task<bool> AddPlant(Plant item)
    // {
    //     try
    //     {
    //         HttpRequestMessage request = CreatePostMessage<Plant>("plant", item);
    //
    //         bool success = await HandleSave(request);
    //
    //         return success;
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleError(ex, "Error in PlantService.AddPlant", "Unable to save the new plant, please try again.");
    //     }
    //
    //     return false;
    // }
    //
    // public async Task<bool> UpdatePlant(Plant item)
    // {
    //     try
    //     {
    //         HttpRequestMessage request = CreatePutMessage<Plant>("plant", item);
    //
    //         bool success = await HandleSave(request);
    //
    //         return success;
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleError(ex, "Error in PlantService.UpdatePlant", "Unable to update the plant, please try again.");
    //     }
    //
    //     return false;
    // }
    //
    // public async Task<bool> DeletePlant(Guid id)
    // {
    //     try
    //     {
    //         HttpRequestMessage request = new(HttpMethod.Delete, $"plant/{id}");
    //         
    //         HttpClient client = ClientFactory.CreateClient("GardenHub");
    //         HttpResponseMessage response = await client.SendAsync(request);
    //
    //         if (response.IsSuccessStatusCode)
    //         {
    //             return true;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         HandleError(ex, "Error in PlantService.DeletePlant", "Unable to delete the selected plant, please try again.");
    //     }
    //
    //     return false;
    // }
}