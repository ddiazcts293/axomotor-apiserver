using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa un viaje registrado.
/// </summary>
public class Trip
{
    /// <summary>
    /// Identificador del viaje.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Número de viaje.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Identificador del conductor asociado.
    /// </summary>
    public required int DriverId { get; set; }

    /// <summary>
    /// Identificador del vehículo asociado.
    /// </summary>
    public required int VehicleId { get; set; }

    /// <summary>
    /// Estado del viaje.
    /// </summary>
    public TripStatus Status { get; set; }

    /// <summary>
    /// Origen del viaje.
    /// </summary>
    public required TripLocation Origin { get; set; }

    /// <summary>
    /// Destino del viaje.
    /// </summary>
    public required TripLocation Destination { get; set; }

    /// <summary>
    /// Lista de paradas planeadas durante el viaje.
    /// </summary>
    [BsonIgnoreIfNull]
    public IList<TripPlannedStop>? PlannedStops { get; set; }

    /// <summary>
    /// Estadísticas del viaje.
    /// </summary>
    [BsonIgnoreIfNull]
    public TripStats? Stats { get; set; }

    /// <summary>
    /// Identificador del agente que creó el viaje.
    /// </summary>
    public required int CreatedById { get; set; }

    /// <summary>
    /// Fecha y hora de cración del viaje.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset CreationDate { get; set; }

    /// <summary>
    /// Identificado del agente que actualizó el viaje.
    /// </summary>
    public int? UpdatedById { get; set; }

    /// <summary>
    /// Fecha y hora de última actualización del viaje.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? LastUpdateDate { get; set; }

    #region BSON Ignore

    [BsonIgnore]
    public bool IsFinished => Status is
        TripStatus.Cancelled or
        TripStatus.Finished or
        TripStatus.Aborted;

    #endregion
}
