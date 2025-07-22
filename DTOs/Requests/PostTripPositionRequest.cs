using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class PostTripPositionRequest
{
    /// <summary>
    /// Latitud.
    /// </summary>
    [Range(-90, 90)]
    public required double Latitude { get; set; }

    /// <summary>
    /// Longitud.
    /// </summary>
    [Range(-180, 180)]
    public required double Longitude { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    [Range(0, Constants.MaxTripSpeedValue)]
    public required float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    [Range(0, long.MaxValue)]
    public required long Timestamp { get; set; }
}
