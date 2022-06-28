using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using GardenHub.Web.Data;
using System.Linq;
using System.Net;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Web.Services.Internal;

public class BaseDataService<loggerType, recordType> : IDataService<recordType> where loggerType: class where recordType : EntityBase
{
    public BaseDataService(string baseRoute, string categoryLabel, IHttpClientFactory clientFactory, IMessageHandler msgHandler, ILogger<loggerType> logger)
    {
        Guard.Against.Null(clientFactory, nameof(clientFactory));
        Guard.Against.Null(msgHandler, nameof(msgHandler));
        Guard.Against.Null(logger, nameof(logger));
        Guard.Against.NullOrWhiteSpace(baseRoute, nameof(baseRoute));
        Guard.Against.NullOrWhiteSpace(categoryLabel, nameof(categoryLabel));
        
        ClientFactory = clientFactory;
        MsgHandler = msgHandler;
        Logger = logger;
        _baseUrl = baseRoute;
        _categoryLabel = categoryLabel;
    }

    protected readonly IHttpClientFactory ClientFactory;
    protected readonly IMessageHandler MsgHandler;
    protected readonly ILogger<loggerType> Logger;
    
    private readonly string _baseUrl = default!;
    private readonly string _categoryLabel = default!;
    
    #region CRUD methods
    public async Task<IEnumerable<recordType>> Get()
    {
        try
        {
            HttpRequestMessage request = new(HttpMethod.Get, _baseUrl);
            var client = ClientFactory.CreateClient("GardenHub");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<IEnumerable<recordType>>();

                return items;
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, $"Error in {_categoryLabel}.Get",
                "Something went wrong trying to get all the records. Please try again.");
        }
        
        return new List<recordType>();
    }
    
    public async Task<recordType> Get(Guid id)
    {
        try
        {
            HttpRequestMessage request = new(HttpMethod.Get, $"{_baseUrl}/{id}");
            var client = ClientFactory.CreateClient("GardenHub");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<recordType>();

                return item;
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, $"Error in {_categoryLabel}.Get(id)",
                "Something went wrong trying to get the specified record. Please try again.");
        }
        
        return null;
    }

    public async Task<bool> Add(recordType item)
    {
        try
        {
            HttpRequestMessage request = CreatePostMessage<recordType>(_baseUrl, item);

            bool success = await HandleSave(request);

            return success;
        }
        catch (Exception ex)
        {
            HandleError(ex, $"Error in {_categoryLabel}.Add", "Unable to save the new record, please try again.");
        }

        return false;
    }
    
    public async Task<bool> Update(recordType item)
    {
        try
        {
            HttpRequestMessage request = CreatePutMessage<recordType>(_baseUrl, item);

            bool success = await HandleSave(request);

            return success;
        }
        catch (Exception ex)
        {
            HandleError(ex, $"Error in {_categoryLabel}.Update", "Unable to update the record, please try again.");
        }

        return false;
    }

    public async Task<bool> Delete(Guid id)
    {
        try
        {
            HttpRequestMessage request = new(HttpMethod.Delete, $"{_baseUrl}/{id}");
            
            HttpClient client = ClientFactory.CreateClient("GardenHub");
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            HandleError(ex, $"Error in {_categoryLabel}.Delete", "Unable to delete the selected record, please try again.");
        }

        return false;
    }
    #endregion
    
    #region Supporting Methods
    protected void HandleError(Exception ex, string logTitle, string userMsg)
    {
        MsgHandler.ShowError(message: userMsg);
        Logger.LogError(exception: ex, message: logTitle);
    }

    protected void HandleValidationError(ValidationProblemDetails details)
    {
        StringBuilder msgBuilder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(details.Detail)) msgBuilder.Append($"{details.Detail}<br/>");

        if (details.Errors.Count > 0)
        {
            msgBuilder.Append("<ul>");
            
            foreach (var errorPair in details.Errors)
            {
                foreach (var msg in errorPair.Value)
                {
                    msgBuilder.Append($"<li>{msg}</li>");
                }
            }

            msgBuilder.Append("</u>");
        }

        MsgHandler.ShowError(msgBuilder.ToString());
    }

    protected HttpRequestMessage CreatePostMessage<T>(string route, T item)
    {
        return CreateRequestWithContent<T>(route, HttpMethod.Post, item);
    }

    protected HttpRequestMessage CreatePutMessage<T>(string route, T item) where T : EntityBase
    {
        return CreateRequestWithContent<T>($"{route}/{item.Id}", HttpMethod.Put, item);
    }

    protected HttpRequestMessage CreateRequestWithContent<T>(string route, HttpMethod method, T item)
    {
        Guard.Against.Null(item, nameof(item));

        string json = JsonSerializer.Serialize(item);

        return new HttpRequestMessage(method, route)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    protected async Task<bool> HandleSave(HttpRequestMessage request)
    {
        HttpClient client = ClientFactory.CreateClient("GardenHub");
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            ValidationProblemDetails errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            HandleValidationError(errors);
        }
        else
        {
            MsgHandler.ShowError("Was unable to save the plant, please try again later.");
        }

        return false;
    }
    #endregion
}