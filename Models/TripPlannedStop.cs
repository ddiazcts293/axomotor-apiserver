using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa una parada planeada durante un viaje.
/// </summary>
public class TripPlannedStop : TripLocation
{
    /// <summary>
    /// Duración estimada de la parada.
    /// </summary>
    [BsonTimeSpanOptions(BsonType.Int32, Units = MongoDB.Bson.Serialization.Options.TimeSpanUnits.Minutes)]
    public TimeSpan? EstimatedDuration { get; set; }

    /// <summary>
    /// Duración real de la parada planeada.
    /// </summary>
    [BsonTimeSpanOptions(BsonType.Int32, MongoDB.Bson.Serialization.Options.TimeSpanUnits.Minutes)]
    public TimeSpan? ActualDuration { get; set; }

    /// <summary>
    /// Indica si la parada se completó.
    /// </summary>
    public bool? IsCompleted { get; set; }
}
