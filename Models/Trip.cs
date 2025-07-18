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
    /// Identificador del conductor asociado.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Driver { get; set; }

    /// <summary>
    /// Identificador del vehículo asociado.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Vehicle { get; set; }

    /// <summary>
    /// Estado del viaje.
    /// </summary>
    public TripStatus Status { get; set; }

    /// <summary>
    /// Punto de partida del viaje.
    /// </summary>
    public required TripLocation StartingPoint { get; set; }

    /// <summary>
    /// Destino final del viaje.
    /// </summary>
    public required TripLocation FinalDestination { get; set; }

    /// <summary>
    /// Lista de paradas planeadas durante el viaje.
    /// </summary>
    public IList<TripLocation>? PlannedStops { get; set; }

    /// <summary>
    /// Lista de posiciones del viaje.
    /// </summary>
    public IList<TripPosition> RecordedPositions { get; set; } = [];

    /// <summary>
    /// Estadísticas del viaje.
    /// </summary>
    [BsonIgnoreIfNull]
    public TripStats? Stats { get; set; }

    /// <summary>
    /// Identificador del agente que creó el viaje.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string CreatedBy { get; set; }

    /// <summary>
    /// Fecha y hora de cración del viaje.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset CreationDate { get; set; }

    /// <summary>
    /// Fecha y hora de última actualización del viaje.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? LastUpdateDate { get; set; }

    [BsonIgnore]
    public bool IsFinished => Status is
        TripStatus.Cancelled or
        TripStatus.Finished or
        TripStatus.Aborted;
}
