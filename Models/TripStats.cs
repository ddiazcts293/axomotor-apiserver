namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa las estadísticas de un viaje.
/// </summary>
public class TripStats
{
    /// <summary>
    /// Distancia recorrida.
    /// </summary>
    public float DistanceTraveled { get; set; }

    /// <summary>
    /// Recuento de paradas realizadas.
    /// </summary>
    public int StopCount { get; set; }

    /// <summary>
    /// Velocidad promedio.
    /// </summary>
    public float AverageSpeed { get; set; }
}
