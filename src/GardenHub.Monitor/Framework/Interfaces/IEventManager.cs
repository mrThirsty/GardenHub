using GardenHub.Monitor.Framework.Events;

namespace GardenHub.Monitor.Framework.Interfaces;

public interface IEventManager
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
}