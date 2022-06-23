

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using GardenHub.Shared.Model;
using GardenHub.Tests.Server.TestDataManagers;
using Microsoft.AspNetCore.Mvc;

namespace GardenHub.Tests.Server;

public class PlantServerTests: IClassFixture<ServerFactory>, IAsyncLifetime
{
    public PlantServerTests(ServerFactory factory)
    {
        _factory = factory;
    }

    private readonly ServerFactory _factory;
    private string _baseUrl = "/plant";
    private List<Guid> _IdsToDelete = new();
    
    [Fact]
    public async Task CreatePlant_CreatesPlant_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = PlantTestDataManager.GeneratePlant();
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, plant);
        var createdPlant = await result.Content.ReadFromJsonAsync<Plant>();
        _IdsToDelete.Add(createdPlant.Id);
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdPlant.Should().BeEquivalentTo(plant, options => options.Excluding(p => p.Id));
        createdPlant.Id.Should().NotBeEmpty();
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}plant/{createdPlant.Id}");
    }
    
    [Fact]
    public async Task CreatePlant_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = PlantTestDataManager.GeneratePlant();
        plant.PlantName = string.Empty;
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, plant);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("PlantName");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Plant Name' must not be empty.");
    }
    
    [Fact]
    public async Task CreatePlant_Fails_WhenPlantExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = await PlantTestDataManager.CreatePlant(httpClient);
        _IdsToDelete.Add(plant.Id);
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, plant);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors!.Detail.Should().Be("Record already exists");
    }

    [Fact]
    public async Task UpdatePlant_UpdatesPlant_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = await PlantTestDataManager.CreatePlant(httpClient);
        _IdsToDelete.Add(plant.Id);
        plant.PlantName = "Tomato";
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{plant.Id}", plant);
        var updatedPlant = await result.Content.ReadFromJsonAsync<Plant>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedPlant.Should().BeEquivalentTo(plant);
    }
    
    [Fact]
    public async Task UpdatePlant_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var plant = await PlantTestDataManager.CreatePlant(httpClient);
        _IdsToDelete.Add(plant.Id);
        
        plant.PlantName = string.Empty;
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{plant.Id}", plant);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("PlantName");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Plant Name' must not be empty.");
    }

    [Fact]
    public async Task UpdatePlant_Fails_WhenPlantDoesntExist()
    {
        var httpClient = _factory.CreateClient();
        var plant = PlantTestDataManager.GeneratePlant();
        _IdsToDelete.Add(plant.Id);
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{plant.Id}", plant);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors.Detail.Should().Be($"Record with Id of '{plant.Id}' does not exist.");
    }

    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();

        foreach (Guid id in _IdsToDelete)
        {
            await httpClient.DeleteAsync($"{_baseUrl}/{id}");
        }
    }
}