using System.Net;
using System.Net.Http.Json;
using GardenHub.Shared.Model;
using GardenHub.Tests.Server.TestDataManagers;
using Microsoft.AspNetCore.Mvc;

namespace GardenHub.Tests.Server;

public class PotServerTests: IClassFixture<ServerFactory>, IAsyncLifetime
{
    public PotServerTests(ServerFactory factory)
    {
        _factory = factory;
    }

    private readonly ServerFactory _factory;
    private string _baseUrl = "/pot";
    private List<Guid> _IdsToDelete = new();
    private List<Guid> _plantsToDelete = new();
    
    [Fact]
    public async Task GetPot_ReturnsPot_WhenPotExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await PotTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _plantsToDelete.Add(item.PlantId);
        
        //Act
        var result = await httpClient.GetAsync($"{_baseUrl}/{item.Id}");
        var foundItem = await result.Content.ReadFromJsonAsync<Pot>();

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundItem.Should().BeEquivalentTo(item);
    }
    
    [Fact]
    public async Task GetPot_ReturnsNotFound_WhenPotDoesNotExist()
    {
        //Arrange
        var httpClient = _factory.CreateClient();

        //Act
        var result = await httpClient.GetAsync($"{_baseUrl}/{Guid.NewGuid()}");

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetAllPots_ReturnsAllPots_WhenPotsExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await PotTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _plantsToDelete.Add(item.PlantId);

        //Act
        var result = await httpClient.GetAsync(_baseUrl);
        var foundItems = await result.Content.ReadFromJsonAsync<List<Pot>>();

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundItems.Count.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task CreatePot_CreatesPot_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = await PlantTestDataManager.CreateItem(httpClient);
        _plantsToDelete.Add(plant.Id);
        
        var item = PotTestDataManager.GenerateItem(plant.Id);
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var createdItem = await result.Content.ReadFromJsonAsync<Pot>();
        _IdsToDelete.Add(createdItem.Id);
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdItem.Should().BeEquivalentTo(item, options => options.Excluding(p => p.Id));
        createdItem.Id.Should().NotBeEmpty();
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}pot/{createdItem.Id}");
    }
    
    [Fact]
    public async Task CreatePot_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = await PlantTestDataManager.CreateItem(httpClient);
        _plantsToDelete.Add(plant.Id);
        
        var item = PotTestDataManager.GenerateItem(plant.Id);
        item.PotName = string.Empty;
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("PotName");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Pot Name' must not be empty.");
    }
    
    [Fact]
    public async Task CreatePot_Fails_WhenPotExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await PotTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _plantsToDelete.Add(item.PlantId);
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors!.Detail.Should().Be("Record already exists");
    }

    [Fact]
    public async Task UpdatePot_UpdatesPot_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await PotTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _plantsToDelete.Add(item.PlantId);
        
        item.PotName = "Pot AAAA";
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var updatedPot = await result.Content.ReadFromJsonAsync<Pot>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedPot.Should().BeEquivalentTo(item);
    }
    
    [Fact]
    public async Task UpdatePot_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await PotTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        _plantsToDelete.Add(item.PlantId);
        
        item.PotName = string.Empty;
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("PotName");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Pot Name' must not be empty.");
    }

    [Fact]
    public async Task UpdatePot_Fails_WhenPotDoesntExist()
    {
        var httpClient = _factory.CreateClient();
        var plant = await PlantTestDataManager.CreateItem(httpClient);
        _plantsToDelete.Add(plant.Id);
        
        var item = PotTestDataManager.GenerateItem(plant.Id);
        _IdsToDelete.Add(item.Id);
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Detail.Should().Be($"Record with Id of '{item.Id}' does not exist.");
    }
    
    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();

        await PotTestDataManager.DeleteItems(_IdsToDelete, httpClient);
        await PlantTestDataManager.DeleteItems(_plantsToDelete, httpClient);
    }
}