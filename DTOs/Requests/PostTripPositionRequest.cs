namespace AxoMotor.ApiServer.DTOs.Requests;

public class PostTripPositionRequest
{
    /// <summary>
    /// Latitud.
    /// </summary>
    public required double Latitude { get; set; }

    /// <summary>
    /// Longitud.
    /// </summary>
    public required double Longitude { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    public long Timestamp { get; set; }
}
