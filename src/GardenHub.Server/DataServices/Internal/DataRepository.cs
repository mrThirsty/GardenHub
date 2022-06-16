using GardenHub.Server.Data.Internal;
using Microsoft.EntityFrameworkCore;

namespace GardenHub.Server.DataServices.Internal;

public class DataRepository<record> : IDataRepository<record> where record : EntityBase
{
    public DataRepository(DbContext dbContext)
    {
        Context = dbContext;
    }

    protected readonly DbContext Context;

    public async Task<bool> CreateAsync(record item)
    {
        item.Id = Guid.NewGuid();

        await Context.Set<record>().AddAsync(item);
        var result = await Context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<record?> GetByIdAsync(Guid id)
    {
        return await Context.Set<record>().FindAsync(id);
    }

    public async Task<IEnumerable<record>> GetAllAsync()
    {
        return await Context.Set<record>().ToListAsync();
    }

    public async Task<bool> UpdateAsync(record item)
    {
        
        record record = Context.Set<record>().AsNoTracking().Where(i => i.Id == item.Id).FirstOrDefault();

        if (record != null)
        {
            Context.Set<record>().Attach(item);
            Context.Entry<record>(item).State = EntityState.Modified;
            var result = await Context.SaveChangesAsync();

            return result > 0;
        }

        return false;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        record record = (record)Context.Find(typeof(record), id);

        if (record != null)
        {
            Context.Set<record>().Remove(record);
            var result = await Context.SaveChangesAsync();

            return result > 0;
        }

        return false;
    }

    public virtual async Task<bool> RecordExists(record item)
    {
        return await RecordExists(item.Id);
    }
    
    public virtual async Task<bool> RecordExists(Guid id)
    {
        var recordByid = await GetByIdAsync(id);

        if (recordByid is not null) return true;

        return false;
    }
}