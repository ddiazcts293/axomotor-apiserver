using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

/// <summary>
/// Representa un vehículo registrado.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Identificador del vehículo.
    /// </summary>
    public int Id { get; set; }

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
    /// Determina si el vehículo está en uso en el momento actual.
    /// </summary>
    public bool InUse { get; set; }

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
    public int Year { get; set; }

    /// <summary>
    /// Fecha de registro del vehículo.
    /// </summary>
    public DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Fecha de última actualización de la información del vehículo.
    /// </summary>
    public DateTimeOffset? LastUpdateDate { get; set; }

    #region JSON ignore

    /// <summary>
    /// Determina si el vehículo está fuera de servicio.
    /// </summary>
    [JsonIgnore]
    public bool IsOutOfService => Status is not VehicleStatus.Operative;

    [JsonIgnore]
    public string Details => $"{Brand} {Model} {Year}";

    #endregion
}
