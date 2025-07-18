using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa una posición concreta durante un instante a lo largo del viaje.
/// </summary>
public class TripPosition
{
    /// <summary>
    /// Coordenadas de la posición.
    /// </summary>
    public required GeoJson2DGeographicCoordinates Coordinates { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset Timestamp { get; set; }
}
