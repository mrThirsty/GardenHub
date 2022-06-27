using System.Net;
using System.Net.Http.Json;
using GardenHub.Shared.Model;
using GardenHub.Tests.Server.TestDataManagers;
using Microsoft.AspNetCore.Mvc;

namespace GardenHub.Tests.Server;

public class SensorServerTests : IClassFixture<ServerFactory>, IAsyncLifetime
{
    public SensorServerTests(ServerFactory factory)
    {
        _factory = factory;
    }

    private readonly ServerFactory _factory;
    private string _baseUrl = "/sensor";
    private List<Guid> _IdsToDelete = new();
    
    [Fact]
    public async Task GetSensor_ReturnsSensor_WhenSensorExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await SensorTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        
        //Act
        var result = await httpClient.GetAsync($"{_baseUrl}/{item.Id}");
        var foundItem = await result.Content.ReadFromJsonAsync<Sensor>();

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundItem.Should().BeEquivalentTo(item);
    }
    
    [Fact]
    public async Task GetSensor_ReturnsNotFound_WhenSensorDoesNotExist()
    {
        //Arrange
        var httpClient = _factory.CreateClient();

        //Act
        var result = await httpClient.GetAsync($"{_baseUrl}/{Guid.NewGuid()}");

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task GetAllSensors_ReturnsAllSensors_WhenSensorsExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await SensorTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);

        //Act
        var result = await httpClient.GetAsync(_baseUrl);
        var foundItems = await result.Content.ReadFromJsonAsync<List<Sensor>>();

        //Arrange
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        foundItems.Count.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task CreateSensor_CreatesSensor_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = SensorTestDataManager.GenerateItem();
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var createdItem = await result.Content.ReadFromJsonAsync<Sensor>();
        _IdsToDelete.Add(createdItem.Id);
        
        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        createdItem.Should().BeEquivalentTo(item, options => options.Excluding(p => p.Id));
        createdItem.Id.Should().NotBeEmpty();
        result.Headers.Location.Should().Be($"{httpClient.BaseAddress}sensor/{createdItem.Id}");
    }
    
    [Fact]
    public async Task CreateSensor_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = SensorTestDataManager.GenerateItem();
        item.SensorName = string.Empty;
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("SensorName");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Sensor Name' must not be empty.");
    }
    
    [Fact]
    public async Task CreateSensor_Fails_WhenSensorExists()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await SensorTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        
        //Act
        var result = await httpClient.PostAsJsonAsync(_baseUrl, item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        errors!.Detail.Should().Be("Record already exists");
    }

    [Fact]
    public async Task UpdateSensor_UpdatesSensor_WhenDataIsCorrect()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var Sensor = await SensorTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(Sensor.Id);
        Sensor.SensorName = "Sensor AAAA";
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{Sensor.Id}", Sensor);
        var updatedSensor = await result.Content.ReadFromJsonAsync<Sensor>();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedSensor.Should().BeEquivalentTo(Sensor);
    }
    
    [Fact]
    public async Task UpdateSensor_Fails_WhenDataIsInvalid()
    {
        //Arrange
        var httpClient = _factory.CreateClient();
        var item = await SensorTestDataManager.CreateItem(httpClient);
        _IdsToDelete.Add(item.Id);
        
        item.SensorName = string.Empty;
        
        //Act
        var result = await httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item);
        var errors = await result.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        var error = errors!.Errors!.Single();

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error.Key.Should().Be("SensorName");
        error.Value.Length.Should().BeGreaterThan(0);
        error.Value[0].Should().Be("'Sensor Name' must not be empty.");
    }

    [Fact]
    public async Task UpdateSensor_Fails_WhenSensorDoesntExist()
    {
        var httpClient = _factory.CreateClient();
        var item = SensorTestDataManager.GenerateItem();
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

        await SensorTestDataManager.DeleteItems(_IdsToDelete, httpClient);

        // foreach (Guid id in _IdsToDelete)
        // {
        //     await httpClient.DeleteAsync($"{_baseUrl}/{id}");
        // }
    }
}