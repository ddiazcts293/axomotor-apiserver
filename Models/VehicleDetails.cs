namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa la información detallada del vehículo.
/// </summary>
public class VehicleDetails
{
    /// <summary>
    /// Marca del vehículo.
    /// </summary>
    public required string Brand { get; set; }

    /// <summary>
    /// Modelo del vehículo.
    /// </summary>
    public required string Model { get; set; }

    /// <summary>
    /// Clase del vehículo.
    /// </summary>
    public required string Class { get; set; }

    /// <summary>
    /// Año del vehículo.
    /// </summary>
    public required int Year { get; set; }
}
