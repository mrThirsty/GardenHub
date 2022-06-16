using GardenHub.Server.Data.Internal;

namespace GardenHub.Server.DataServices.Internal;

public interface IDataRepository<record> where record : EntityBase
{
    public Task<bool> CreateAsync(record item);
    public Task<record?> GetByIdAsync(Guid id);
    public Task<IEnumerable<record>> GetAllAsync();
    public Task<bool> UpdateAsync(record item);
    public Task<bool> DeleteAsync(Guid id);

    public Task<bool> RecordExists(record item);
    public Task<bool> RecordExists(Guid id);
}