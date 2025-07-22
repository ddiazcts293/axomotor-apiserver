using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AxoMotor.ApiServer.Services;

public class PositionService
{
    private readonly IMongoCollection<TripPosition> _collection;

    public PositionService(IOptions<MongoDBSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<TripPosition>(
            settings.Value.Collections.Positions
        );
    }

    /// <summary>
    /// Registra una nueva posición de un viaje.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public async Task PushOneAsync(TripPosition position) =>
        await _collection.InsertOneAsync(position);

    /// <summary>
    /// Obtiene una lista de posiciones registradas durante un viaje.
    /// </summary>
    /// <param name="tripId"></param>
    /// <param name="skip"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<IList<TripPosition>> GetAsync(
        string tripId,
        int skip,
        int limit
    ) => await _collection.Find(x => x.TripId == tripId)
        .Skip(skip)
        .Limit(limit)
        .ToListAsync();

}
