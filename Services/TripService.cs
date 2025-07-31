using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AxoMotor.ApiServer.Services;

public class TripService
{
    private readonly MongoDBCollections _dbCollectionNames;
    private readonly IMongoCollection<Trip> _collection;

    /// <summary>
    /// Constructor principal.
    /// </summary>
    /// <param name="settings">Configuración de MongoDB.</param>
    public TripService(IOptions<MongoDBSettings> settings)
    {
        _dbCollectionNames = settings.Value.Collections;
        MongoClient client = new(settings.Value.ConnectionUri);
        IMongoDatabase database = client.GetDatabase(settings.Value.DatabaseName);

        _collection = database.GetCollection<Trip>(
            settings.Value.Collections.Trips
        );
    }

    /// <summary>
    /// Verifica si un viaje con el identificador dado existe.
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    public async Task<bool> Exists(string tripId) =>
        await _collection.Find(x => x.Id == tripId).AnyAsync();

    /// <summary>
    /// Obtiene el estado de un viaje.
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    public async Task<TripStatus?> GetStatus(string tripId)
    {
        var builder = Builders<Trip>.Filter;
        var projection = new ProjectionDefinitionBuilder<Trip>()
            .Include(x => x.Status);

        var trip = await _collection.Find(x => x.Id == tripId)
            .Project<Trip>(projection)
            .FirstOrDefaultAsync();

        return trip?.Status;
    }

    /// <summary>
    /// Crea un nuevo viaje.
    /// </summary>
    /// <param name="trip">Instancia de clase con la información del viaje.</param>
    /// <returns></returns>
    public async Task CreateAsync(Trip trip)
    {
        // asigna el tiempo de creación
        trip.CreationDate = DateTimeOffset.UtcNow;
        await _collection.InsertOneAsync(trip);
    }

    /// <summary>
    /// Obtiene la información del viaje.
    /// </summary>
    /// <param name="tripId">Identificador del viaje.</param>
    /// <returns></returns>
    public async Task<Trip?> GetAsync(string tripId) =>
        await _collection.Find(x => x.Id == tripId).FirstOrDefaultAsync();

    /// <summary>
    /// Obtiene un lista de viajes que complan con los criterios dados.
    /// </summary>
    /// <param name="driverId"></param>
    /// <param name="vehicleId"></param>
    /// <param name="createdById"></param>
    /// <param name="updatedById"></param>
    /// <param name="originName"></param>
    /// <param name="destinationName"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<IList<Trip>> GetAsync(
        int? driverId = null,
        int? vehicleId = null,
        int? createdById = null,
        int? updatedById = null,
        string? originName = null,
        string? destinationName = null,
        TripStatus? status = null
    )
    {
        var builder = Builders<Trip>.Filter;
        var projection = new ProjectionDefinitionBuilder<Trip>()
            .Exclude(x => x.PlannedStops);

        List<FilterDefinition<Trip>> filters = [];

        if (driverId is not null)
            filters.Add(builder.Eq(x => x.DriverId, driverId));
        if (vehicleId is not null)
            filters.Add(builder.Eq(x => x.VehicleId, vehicleId));
        if (createdById is not null)
            filters.Add(builder.Eq(x => x.CreatedById, createdById));
        if (updatedById is not null)
            filters.Add(builder.Eq(x => x.UpdatedById, updatedById));
        if (!string.IsNullOrWhiteSpace(originName))
            filters.Add(builder.Eq(x => x.Origin.Name, originName));
        if (!string.IsNullOrWhiteSpace(destinationName))
            filters.Add(builder.Eq(x => x.Destination.Name, destinationName));
        if (status is not null)
            filters.Add(builder.Eq(x => x.Status, status));

        if (filters.Count == 0)
            filters.Add(builder.Empty);

        return await _collection
            .Find(builder.And(filters))
            .SortByDescending(x => x.CreationDate)
            .Project<Trip>(projection)
            .ToListAsync();
    }

    public async Task<Trip?> GetPendingAsync(int driverId)
    {
        var builder = Builders<Trip>.Filter;
        var filter = builder.And(
            builder.Eq(x => x.DriverId, driverId),
            builder.In(x => x.Status,
            [
                TripStatus.Planned,
                TripStatus.Delayed,
                TripStatus.OnRoute,
                TripStatus.AtBase,
                TripStatus.Stopped
            ])
        );

        return await _collection.Find(filter)
            .SortBy(x => x.CreationDate)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Actualiza la información de un viaje.
    /// </summary>
    /// <param name="tripId"></param>
    /// <param name="driverId"></param>
    /// <param name="vehicleId"></param>
    /// <param name="status"></param>
    /// <param name="origin"></param>
    /// <param name="destination"></param>
    /// <param name="plannedStops"></param>
    /// <param name="stats"></param>
    /// <param name="updatedById"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(
        string tripId,
        int? driverId = null,
        int? vehicleId = null,
        TripStatus? status = null,
        TripLocation? origin = null,
        TripLocation? destination = null,
        IList<TripPlannedStop>? plannedStops = null,
        int? updatedById = null,
        TripStats? stats = null
    )
    {
        // crea el filtro para seleccionar el viaje a actualizar
        var filter = Builders<Trip>.Filter.Eq(x => x.Id, tripId);
        var update = Builders<Trip>.Update.CurrentDate(x => x.LastUpdateDate);

        if (driverId is not null)
            update = update.Set(x => x.DriverId, driverId);
        if (vehicleId is not null)
            update = update.Set(x => x.VehicleId, vehicleId);
        if (status is not null)
            update = update.Set(x => x.Status, status.Value);
        if (origin is not null)
            update = update.Set(x => x.Origin, origin);
        if (destination is not null)
            update = update.Set(x => x.Destination, destination);
        if (plannedStops is not null)
            update = update.Set(x => x.PlannedStops, plannedStops);
        if (stats is not null)
            update = update.Set(x => x.Stats, stats);
        if (updatedById is not null)
            update = update.Set(x => x.UpdatedById, updatedById);

        var result = await _collection.UpdateOneAsync(filter, update);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    /// <summary>
    /// Elimina un viaje.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
}
