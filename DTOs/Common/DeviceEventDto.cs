using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class DeviceEventDto
{
    /// <summary>
    /// Código del evento.
    /// </summary>
    public required DeviceEventCode Code { get; set; }

    /// <summary>
    /// Tipo de evento.
    /// </summary>
    public required DeviceEventType Type { get; set; }

    /// <summary>
    /// Severidad del evento.
    /// </summary>
    public DeviceEventSeverity Severity { get; set; }

    /// <summary>
    /// Marca de tiempo de cuando se produjo el evento.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    public static implicit operator DeviceEventDto?(DeviceEvent? @event)
        => @event is null ? null : new()
        {
            Code = @event.Code,
            Type = @event.Type,
            Severity = @event.Severity,
            Timestamp = @event.Timestamp,
        };
}
