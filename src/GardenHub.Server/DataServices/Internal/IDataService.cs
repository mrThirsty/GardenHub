using GardenHub.Server.Data.Internal;
using LanguageExt.Common;

namespace GardenHub.Server.DataServices.Internal;

public interface IDataService<record> where record : EntityBase
{
    public Task<Result<record>> CreateAsync(record item);
    public Task<record?> GetByIdAsync(Guid id);
    public Task<IEnumerable<record>> GetAllAsync();
    public Task<Result<record>> UpdateAsync(record item);
    public Task<bool> DeleteAsync(Guid id);

    public Task<bool> RecordExists(record item);
}