using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa una ubicación específica del viaje.
/// </summary>
public class TripLocation
{
    /// <summary>
    /// Nombre de la ubicación.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Dirección de la ubicación.
    /// </summary>
    public required string Address { get; set; }

    /// <summary>
    /// Coordenadas de la ubicación.
    /// </summary>
    public required GeoJson2DGeographicCoordinates Coordinates { get; set; }

    /// <summary>
    /// Radio en metros que abarca la ubicación en un mapa.
    /// </summary>
    public required float Ratio { get; set; }

    /// <summary>
    /// Fecha y hora de presencia en la ubicación.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset DateTime { get; set; }

    /// <summary>
    /// Intervalo de tiempo de duración de presencia en la parada.
    /// </summary>
    public TimeSpan? Duration { get; set; }
}
