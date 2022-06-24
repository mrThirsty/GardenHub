using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using GardenHub.Web.Data;
using System.Linq;
using System.Net;
using GardenHub.Shared.Model.Internal;

namespace GardenHub.Web.Services.Internal;

public class BaseDataService<loggerType> where loggerType: class
{
    public BaseDataService(IHttpClientFactory clientFactory, IMessageHandler msgHandler, ILogger<loggerType> logger)
    {
        Guard.Against.Null(clientFactory, nameof(clientFactory));
        Guard.Against.Null(msgHandler, nameof(msgHandler));
        Guard.Against.Null(logger, nameof(logger));
        
        ClientFactory = clientFactory;
        MsgHandler = msgHandler;
        Logger = logger;
    }

    protected readonly IHttpClientFactory ClientFactory;
    protected readonly IMessageHandler MsgHandler;
    protected readonly ILogger<loggerType> Logger;
    
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
}