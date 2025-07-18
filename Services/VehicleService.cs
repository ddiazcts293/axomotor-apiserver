using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AxoMotor.ApiServer.Services;

public class VehicleService
{
    private readonly IMongoCollection<Vehicle> _collection;

    public VehicleService(IOptions<MongoDBSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Vehicle>(
            settings.Value.Collections.Vehicles
        );
    }

    public async Task RegisterAsync(Vehicle vehicle)
    {
        vehicle.RegistrationDate = DateTimeOffset.UtcNow;
        await _collection.InsertOneAsync(vehicle);
    }

    public async Task<Vehicle?> GetAsync(string vehicleId) =>
        await _collection.Find(x => x.Id == vehicleId)
            .FirstOrDefaultAsync();

    public async Task<IList<Vehicle>> GetAsync(
        VehicleStatus? status,
        string? brand = null,
        string? model = null,
        string? vehicleClass = null,
        int? year = null,
        bool? inUse = null
    )
    {
        var builder = Builders<Vehicle>.Filter;
        List<FilterDefinition<Vehicle>> filters = [];

        if (status is not null)
        {
            filters.Add(builder.Eq(x => x.Status, status));
        }
        if (!string.IsNullOrWhiteSpace(brand))
        {
            filters.Add(builder.Eq(x => x.Details.Brand, brand));
        }
        if (!string.IsNullOrWhiteSpace(model))
        {
            filters.Add(builder.Eq(x => x.Details.Model, model));
        }
        if (vehicleClass is not null)
        {
            filters.Add(builder.Eq(x => x.Details.Class, vehicleClass));
        }
        if (year is not null)
        {
            filters.Add(builder.Eq(x => x.Details.Year, year));
        }
        if (inUse is not null)
        {
            filters.Add(builder.Eq(x => x.InUse, inUse));
        }

        if (filters.Count == 0)
        {
            filters.Add(builder.Empty);
        }

        return await _collection.Find(builder.And(filters)).ToListAsync();
    }

    public async Task<bool> UpdateAsync(
        string vehicleId,
        string? plateNumber = null,
        VehicleStatus? status = null
    )
    {
        var filter = Builders<Vehicle>.Filter.Eq(x => x.Id, vehicleId);
        var update = Builders<Vehicle>.Update.CurrentDate(x => x.LastUpdateDate);

        if (plateNumber is not null)
        {
            update = update.Set(x => x.PlateNumber, plateNumber);
        }
        if (status is not null)
        {
            update = update.Set(x => x.Status, status);
        }

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string vehicleId)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == vehicleId);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<IList<DeviceEvent>?> GetEventsAsync(
        string vehicleId,
        int skip,
        int limit
    )
    {
        var filter = Builders<Vehicle>.Filter.Eq(x => x.Id, vehicleId);
        var projection = Builders<Vehicle>.Projection
            .Slice(x => x.DeviceEvents, skip, limit);
        var result = await _collection.Find(filter)
            .Project<Vehicle>(projection)
            .SingleOrDefaultAsync();

        return result?.DeviceEvents.ToList();
    }

    public async Task<bool> PushEventAsync(string vehicleId, DeviceEvent deviceEvent)
    {
        var filter = Builders<Vehicle>.Filter.Eq(x => x.Id, vehicleId);
        var update = Builders<Vehicle>.Update.Push(x => x.DeviceEvents, deviceEvent);
        var result = await _collection.UpdateOneAsync(filter, update);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
