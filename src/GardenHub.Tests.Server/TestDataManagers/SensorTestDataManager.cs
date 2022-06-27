using System.Net.Http.Json;
using Bogus;
using GardenHub.Shared.Model;

namespace GardenHub.Tests.Server.TestDataManagers;

public class SensorTestDataManager
{
    private const string _BaseRoute = "/sensor";
    
    public static Sensor GenerateItem()
    {
        var recordFaker = new Faker<Sensor>()
            .RuleFor(p => p.Id, Guid.NewGuid)
            .RuleFor(p => p.SensorName, f => f.Lorem.Text());

        return recordFaker.Generate();
    }

    public async static Task<Sensor> CreateItem(HttpClient httpClient)
    {
        Sensor item = GenerateItem();

        var result = await httpClient.PostAsJsonAsync(_BaseRoute, item);
        var createdItem = await result.Content.ReadFromJsonAsync<Sensor>();

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