using System.Net.Http.Json;
using Bogus;
using GardenHub.Shared.Model;

namespace GardenHub.Tests.Server.TestDataManagers;

public class ReadingTestDataManager
{
    private const string _BaseRoute = "/reading";
    
    public static Reading GenerateItem(Guid potId)
    {
        var recordFaker = new Faker<Reading>()
            .RuleFor(p => p.Id, Guid.NewGuid)
            .RuleFor(p => p.Timestamp, f => f.Date.Past())
            .RuleFor(p => p.PotId, potId);

        return recordFaker.Generate();
    }

    public async static Task<Reading> CreateItem(HttpClient httpClient)
    {
        Pot pot = await PotTestDataManager.CreateItem(httpClient);
        Reading item = GenerateItem(pot.Id);

        var result = await httpClient.PostAsJsonAsync(_BaseRoute, item);
        var createdItem = await result.Content.ReadFromJsonAsync<Reading>();
        createdItem.Pot = pot;
        
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