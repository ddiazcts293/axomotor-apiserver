using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa una parada planeada durante un viaje.
/// </summary>
public class TripPlannedStop : TripLocation
{
    /// <summary>
    /// Fecha y hora real de presencia en la ubicación.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? ActualDateTime { get; set; }

    /// <summary>
    /// Duración estimada de la parada.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonTimeSpanOptions(BsonType.Int32, Units = MongoDB.Bson.Serialization.Options.TimeSpanUnits.Minutes)]
    public TimeSpan? EstimatedDuration { get; set; }

    /// <summary>
    /// Duración real de la parada planeada.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonTimeSpanOptions(BsonType.Int32, MongoDB.Bson.Serialization.Options.TimeSpanUnits.Minutes)]
    public TimeSpan? ActualDuration { get; set; }

    /// <summary>
    /// Indica si la parada se completó.
    /// </summary>
    [BsonIgnoreIfNull]
    public bool? IsCompleted { get; set; }
}
