using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AxoMotor.ApiServer.Services;

public class UserAccountService
{
    private readonly IMongoCollection<UserAccount> _userAccountCollection;

    public UserAccountService(IOptions<MongoDBSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _userAccountCollection = database.GetCollection<UserAccount>(
            settings.Value.Collections.UserAccount
        );
    }

    public async Task RegisterAsync(UserAccount account)
    {
        account.RegistrationDate = DateTimeOffset.UtcNow;
        await _userAccountCollection.InsertOneAsync(account);
    }

    public async Task<UserAccount?> GetAsync(string id) =>
        await _userAccountCollection.Find(x => x.Id == id)
            .FirstOrDefaultAsync();

    public async Task<IList<UserAccount>> GetAsync(
        UserAccountType? type,
        UserAccountStatus? status,
        bool? isLoggedIn
    )
    {
        var builder = Builders<UserAccount>.Filter;
        List<FilterDefinition<UserAccount>> filters = [];

        if (type is not null)
        {
            filters.Add(builder.Eq(x => x.Type, type));
        }
        if (status is not null)
        {
            filters.Add(builder.Eq(x => x.Status, status));
        }
        if (isLoggedIn is not null)
        {
            filters.Add(builder.Eq(x => x.IsLoggedIn, isLoggedIn));
        }
        
        if (filters.Count == 0)
        {
            filters.Add(builder.Empty);
        }

        return await _userAccountCollection.Find(builder.And(filters)).ToListAsync();
    }

    public async Task<bool> UpdateAsync(
        string id,
        string? firstName,
        string? lastName,
        UserAccountStatus? status
    )
    {
        var filter = Builders<UserAccount>.Filter.Eq(x => x.Id, id);
        var update = Builders<UserAccount>.Update.CurrentDate(
            x => x.LastUpdateDate);

        if (firstName is not null)
        {
            update = update.Set(x => x.FirstName, firstName);
        }
        if (lastName is not null)
        {
            update = update.Set(x => x.LastName, lastName);
        }
        if (status is not null)
        {
            update = update.Set(x => x.Status, status);
        }

        var result = await _userAccountCollection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _userAccountCollection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
