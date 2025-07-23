using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

/// <summary>
/// Representa una posición concreta durante un instante a lo largo del viaje.
/// </summary>
public class TripPositionDto : PositionDtoBase
{
    /// <summary>
    /// Fuente de posición.
    /// </summary>
    public TripPositionSource Source { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    public static implicit operator TripPositionDto?(TripPosition? position)
        => position is null ? null : new()
    {
        Latitude = position.Coordinates.Latitude,
        Longitude = position.Coordinates.Longitude,
        Speed = position.Speed,
        Source = position.Source,
        Timestamp = position.Timestamp
    };
}
