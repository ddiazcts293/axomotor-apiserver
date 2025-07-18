using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AxoMotor.ApiServer.Services;

public class TripService
{
    private readonly IMongoCollection<Trip> _collection;

    public TripService(IOptions<MongoDBSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Trip>(
            settings.Value.Collections.Trips
        );
    }

    public async Task CreateAsync(Trip trip)
    {
        trip.CreationDate = DateTimeOffset.UtcNow;
        trip.Vehicle = trip.Driver;
        trip.CreatedBy = trip.Driver;

        await _collection.InsertOneAsync(trip);
    }

    public async Task<Trip?> GetAsync(string tripId) =>
        await _collection.Find(x => x.Id == tripId).FirstOrDefaultAsync();

    public async Task<IList<Trip>> GetAsync(
        string? driverId = null,
        string? vehicleId = null,
        string? createdById = null,
        string? startingPointName = null,
        string? finalDestinationName = null,
        TripStatus? status = null
    )
    {
        var builder = Builders<Trip>.Filter;
        List<FilterDefinition<Trip>> filters = [];

        if (driverId is not null)
        {
            filters.Add(builder.Eq(x => x.Driver, driverId));
        }
        if (vehicleId is not null)
        {
            filters.Add(builder.Eq(x => x.Vehicle, vehicleId));
        }
        if (createdById is not null)
        {
            filters.Add(builder.Eq(x => x.CreatedBy, createdById));
        }
        if (startingPointName is not null)
        {
            filters.Add(builder.Eq(x => x.StartingPoint.Name, startingPointName));
        }
        if (finalDestinationName is not null)
        {
            filters.Add(builder.Eq(x => x.FinalDestination.Name, finalDestinationName));
        }
        if (status is not null)
        {
            filters.Add(builder.Eq(x => x.Status, status));
        }

        if (filters.Count == 0)
        {
            filters.Add(builder.Empty);
        }

        return await _collection.Find(builder.And(filters)).ToListAsync();
    }

    public async Task<bool> UpdateAsync(
        string tripId,
        string? driver = null,
        string? vehicle = null,
        TripStatus? status = null,
        TripLocation? startingPoint = null,
        TripLocation? finalDestination = null,
        IList<TripLocation>? plannedStops = null
    )
    {
        var filter = Builders<Trip>.Filter.Eq(x => x.Id, tripId);
        var update = Builders<Trip>.Update.CurrentDate(x => x.LastUpdateDate);

        if (driver is not null)
        {
            update = update.Set(x => x.Driver, driver);
        }
        if (vehicle is not null)
        {
            update = update.Set(x => x.Vehicle, vehicle);
        }
        if (status is not null)
        {
            update = update.Set(x => x.Status, status);
        }
        if (startingPoint is not null)
        {
            update = update.Set(x => x.StartingPoint, startingPoint);
        }
        if (finalDestination is not null)
        {
            update = update.Set(x => x.FinalDestination, finalDestination);
        }
        if (plannedStops is not null)
        {
            update = update.Set(x => x.PlannedStops, plannedStops);
        }

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<bool> PushPositionAsync(string tripId, TripPosition position)
    {
        var filter = Builders<Trip>.Filter.Eq(x => x.Id, tripId);
        var update = Builders<Trip>.Update.Push(x => x.RecordedPositions, position);
        var result = await _collection.UpdateOneAsync(filter, update);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<IList<TripPosition>?> GetPositionsAsync(
        string tripId,
        int skip,
        int limit
    )
    {
        var filter = Builders<Trip>.Filter.Eq(x => x.Id, tripId);
        var projection = Builders<Trip>.Projection
            .Slice(x => x.RecordedPositions, skip, limit);
        var result = await _collection.Find(filter)
            .Project<Trip>(projection)
            .SingleOrDefaultAsync();

        return result?.RecordedPositions.ToList();
    }
}
