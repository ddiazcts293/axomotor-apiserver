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
    /// Identificador de la posición.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Identificador del viaje asociado.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string TripId { get; set; }

    /// <summary>
    /// Coordenadas de la posición.
    /// </summary>
    public required GeoJson2DGeographicCoordinates Coordinates { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    public required float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public required DateTimeOffset Timestamp { get; set; }
}
