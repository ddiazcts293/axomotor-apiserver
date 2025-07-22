using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa una fotografía de una incidencia.
/// </summary>
public class IncidentPicture
{
    /// <summary>
    /// Identificador del archivo.
    /// </summary>
    public required string FileId { get; set; }

    /// <summary>
    /// Marca de tiempo del archivo.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset Timestamp { get; set; }
}
