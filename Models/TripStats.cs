using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa las estadísticas de un viaje.
/// </summary>
public class TripStats
{
    /// <summary>
    /// Duración total del viaje.
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Distancia recorrida.
    /// </summary>
    public float DistanceTraveled { get; set; }

    /// <summary>
    /// Recuento de paradas realizadas.
    /// </summary>
    public int StopCount { get; set; }

    /// <summary>
    /// Velocidad promedio.
    /// </summary>
    public float AverageSpeed { get; set; }

    /// <summary>
    /// Tiempo total que el vehículo permaneció no localizable.
    /// </summary>
    [BsonTimeSpanOptions(BsonType.Int32, Units = MongoDB.Bson.Serialization.Options.TimeSpanUnits.Minutes)]
    public TimeSpan GpsSignalLostDuration { get; set; }
}
