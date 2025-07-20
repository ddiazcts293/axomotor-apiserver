using AxoMotor.ApiServer.Models;

namespace AxoMotor.ApiServer.DTOs.Common;

/// <summary>
/// Representa una posición concreta durante un instante a lo largo del viaje.
/// </summary>
public class TripPositionDto : PositionDtoBase
{
    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    public static TripPositionDto Convert(TripPosition position)
    {
        return new()
        {
            Latitude = position.Coordinates.Latitude,
            Longitude = position.Coordinates.Longitude,
            Speed = position.Speed,
            Timestamp = position.Timestamp
        };
    }
}
