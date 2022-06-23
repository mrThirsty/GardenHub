using FluentValidation;
using GardenHub.Server.Exceptions;
using GardenHub.Shared.Model.Internal;
using LanguageExt.Common;

namespace GardenHub.Server.DataServices.Internal;

public class DataService<record, repoType> : IDataService<record> where record : EntityBase where repoType : IDataRepository<record>
{
    public DataService(IValidator<record> validator, repoType repo)
    {
        Validator = validator;
        Repository = repo;
    }
    
    protected readonly IValidator<record> Validator;
    protected readonly repoType Repository;

    public async Task<Result<record>> CreateAsync(record item)
    {
        var validationResult = await Validator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            var validationException = new ValidationException(validationResult.Errors);

            return new Result<record>(validationException);
        }
        
        var existingRecipe = await RecordExists(item);
        if (existingRecipe)
        {
            return new Result<record>(new ValidationException("Record already exists"));
        }

        var createResult = await Repository.CreateAsync(item);

        if (!createResult)
        {
            return new Result<record>(new OperationException(message: "Unable to create the record",operation: "CREATE"));
        }

        return item;
    }

    public async Task<record?> GetByIdAsync(Guid id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<record>> GetAllAsync()
    {
        return await Repository.GetAllAsync();
    }

    public async Task<Result<record>> UpdateAsync(record item)
    {
        var validationResult = await Validator.ValidateAsync(item);

        if (!validationResult.IsValid)
        {
            var validationException = new ValidationException(validationResult.Errors);

            return new Result<record>(validationException);
        }

        var createResult = await Repository.UpdateAsync(item);

        if (!createResult)
        {
            return new Result<record>(new ValidationException(message: $"Record with Id of '{item.Id}' does not exist."));
        }

        return item;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await Repository.DeleteAsync(id);
    }

    public virtual async Task<bool> RecordExists(record item)
    {
        return await Repository.RecordExists(item);
    }
}