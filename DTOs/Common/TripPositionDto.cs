namespace AxoMotor.ApiServer.DTOs.Common;

/// <summary>
/// Representa una posición concreta durante un instante a lo largo del viaje.
/// </summary>
public class TripPositionDto
{
    /// <summary>
    /// Latitud.
    /// </summary>
    public required float Latitude { get; set; }

    /// <summary>
    /// Longitud.
    /// </summary>
    public required float Longitude { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }
}
