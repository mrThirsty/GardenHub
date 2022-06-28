using GardenHub.Shared.Model.Internal;

namespace GardenHub.Web.Services.Internal;

public interface IDataService<T> where T : EntityBase
{
    Task<IEnumerable<T>> Get();
    Task<T> Get(Guid id);
    Task<bool> Add(T item);
    Task<bool> Update(T item);
    Task<bool> Delete(Guid id);
}