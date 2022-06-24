using MudBlazor;

namespace GardenHub.Web.Services;

public interface IMessageHandler
{
    void ShowMessage(string message, Severity level);
    void ShowError(string message);
    void ShowSuccess(string message);
    void ShowInformation(string message);
}