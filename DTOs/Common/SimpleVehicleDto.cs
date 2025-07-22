using AxoMotor.ApiServer.Models.Catalog;

namespace AxoMotor.ApiServer.DTOs.Common;

public class SimpleVehicleDto
{
    /// <summary>
    /// Identificador del vehículo.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Matrícula o código en las placas.
    /// </summary>
    public required string PlateNumber { get; set; }

    /// <summary>
    /// Clase del vehículo.
    /// </summary>
    public required string Class { get; set; }

    /// <summary>
    /// Información detallada del vehículo.
    /// </summary>
    public required string Details { get; set; }

    public static implicit operator SimpleVehicleDto?(Vehicle? vehicle)
        => vehicle is null ? null : new()
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            Class = vehicle.Class,
            Details = vehicle.Details
        };
}
