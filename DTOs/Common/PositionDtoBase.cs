namespace AxoMotor.ApiServer.DTOs.Common;

/// <summary>
/// Represeta la base de una clase que registra una posición GPS.
/// </summary>
public abstract class PositionDtoBase
{
    /// <summary>
    /// Latitud.
    /// </summary>
    public required double Latitude { get; set; }

    /// <summary>
    /// Longitud.
    /// </summary>
    public required double Longitude { get; set; }
}
