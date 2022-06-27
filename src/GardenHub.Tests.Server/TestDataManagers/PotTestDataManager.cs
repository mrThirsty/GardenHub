using System.Net.Http.Json;
using Bogus;
using GardenHub.Shared.Model;

namespace GardenHub.Tests.Server.TestDataManagers;

public class PotTestDataManager
{
    private const string _BaseRoute = "/pot";
    
    public static Pot GenerateItem(Guid plantId)
    {
        var recordFaker = new Faker<Pot>()
            .RuleFor(p => p.Id, Guid.NewGuid)
            .RuleFor(p => p.PotName, f => f.Lorem.Word())
            .RuleFor(p => p.DatePlanted, DateTime.Now)
            .RuleFor(p => p.PlantId, plantId);

        return recordFaker.Generate();
    }

    public async static Task<Pot> CreateItem(HttpClient httpClient)
    {
        Plant plant = await PlantTestDataManager.CreateItem(httpClient);
        Pot item = GenerateItem(plant.Id);

        var result = await httpClient.PostAsJsonAsync(_BaseRoute, item);
        var createdItem = await result.Content.ReadFromJsonAsync<Pot>();

        return createdItem;
    }
    
    public async static Task DeleteItems(IEnumerable<Guid> idsToDelete, HttpClient client)
    {
        foreach (var id in idsToDelete)
        {
            await DeleteItem(id, client);
        }
    }

    public async static Task DeleteItem(Guid id, HttpClient client)
    {
        await client.DeleteAsync($"{_BaseRoute}/{id}");
    }
}