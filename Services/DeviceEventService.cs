using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AxoMotor.ApiServer.Services;

public class DeviceEventService
{
    private readonly IMongoCollection<DeviceEvent> _collection;

    public DeviceEventService(IOptions<MongoDBSettings> settings)
    {
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<DeviceEvent>(
            settings.Value.Collections.DeviceEvents
        );
    }

    /// <summary>
    /// Registra un nuevo evento de dispositivo.
    /// </summary>
    /// <param name="deviceEvent"></param>
    /// <returns></returns>
    public async Task PushOneAsync(DeviceEvent deviceEvent) =>
        await _collection.InsertOneAsync(deviceEvent);

    /// <summary>
    /// Obtiene una lista de eventos de dispositivo.
    /// </summary>
    /// <param name="vehicleId"></param>
    /// <param name="skip"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<IList<DeviceEvent>> GetAsync(
        int vehicleId,
        int skip,
        int limit
    ) => await _collection.Find(x => x.VehicleId == vehicleId)
        .Skip(skip)
        .Limit(limit)
        .ToListAsync();
}
