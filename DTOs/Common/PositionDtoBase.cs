using System.ComponentModel.DataAnnotations;

namespace AxoMotor.ApiServer.DTOs.Common;

/// <summary>
/// Represeta la base de una clase que registra una posición GPS.
/// </summary>
public abstract class PositionDtoBase
{
    /// <summary>
    /// Latitud.
    /// </summary>
    [Required]
    [Range(-90, 90)]
    public required double Latitude { get; set; }

    /// <summary>
    /// Longitud.
    /// </summary>
    [Required]
    [Range(-180, 180)]
    public required double Longitude { get; set; }
}
