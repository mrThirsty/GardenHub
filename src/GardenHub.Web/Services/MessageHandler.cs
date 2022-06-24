using Ardalis.GuardClauses;
using MudBlazor;

namespace GardenHub.Web.Services;

public class MessageHandler : IMessageHandler
{
    public MessageHandler(ISnackbar snackBar)
    {
        Guard.Against.Null(snackBar, nameof(snackBar));
        
        _snackbar = snackBar;
    }

    private readonly ISnackbar _snackbar;
    
    public void ShowMessage(string message, Severity level)
    {
        _snackbar.Add(message, level);
    }

    public void ShowError(string message)
    {
        ShowMessage(message, Severity.Error);
    }

    public void ShowSuccess(string message)
    {
        ShowMessage(message, Severity.Success);
    }

    public void ShowInformation(string message)
    {
        ShowMessage(message, Severity.Info);
    }
}