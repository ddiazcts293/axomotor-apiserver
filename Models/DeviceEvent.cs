using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa un evento de dispositivo.
/// </summary>
public class DeviceEvent
{
    /// <summary>
    /// Código del evento.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tipo de evento.
    /// </summary>
    public DeviceEventType Type { get; set; }

    /// <summary>
    /// Severidad del evento.
    /// </summary>
    public DeviceEventSeverity Severity { get; set; }

    /// <summary>
    /// Marca de tiempo de cuando se produjo el evento.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset Timestamp { get; set; }
}
