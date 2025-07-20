using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class VehicleDto
{
    /// <summary>
    /// Identificador del vehículo.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Matrícula o código en las placas.
    /// </summary>
    public required string PlateNumber { get; set; }

    /// <summary>
    /// Número de registro en tarjeta de ciculación vehicular.
    /// </summary>
    public required string RegistrationNumber { get; set; }

    /// <summary>
    /// Estado del vehículo.
    /// </summary>
    public VehicleStatus Status { get; set; }

    /// <summary>
    /// Determina si el vehículo está en uso en el momento.
    /// </summary>
    public bool InUse { get; set; }

    /// <summary>
    /// Información detallada del vehículo.
    /// </summary>
    public required VehicleDetails Details { get; set; }

    /// <summary>
    /// Lista de eventos del dispositivo.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IList<DeviceEvent>? DeviceEvents { get; set; }

    /// <summary>
    /// Fecha de registro del vehículo.
    /// </summary>
    public DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Fecha de última actualización de la información del vehículo.
    /// </summary>
    public DateTimeOffset? LastUpdateDate { get; set; }

    public static VehicleDto Convert(Vehicle vehicle)
    {
        return new()
        {
            Id = vehicle.Id,
            PlateNumber = vehicle.PlateNumber,
            RegistrationNumber = vehicle.RegistrationNumber,
            Status = vehicle.Status,
            InUse = vehicle.InUse,
            Details = vehicle.Details,
            DeviceEvents = vehicle.DeviceEvents,
            RegistrationDate = vehicle.RegistrationDate,
            LastUpdateDate = vehicle.LastUpdateDate,
        };
    }
}
