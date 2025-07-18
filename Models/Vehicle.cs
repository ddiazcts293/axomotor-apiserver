using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa un vehículo registrado.
/// </summary>
public class Vehicle
{
    /// <summary>
    /// Identificador del vehículo.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

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
    public IList<DeviceEvent> DeviceEvents { get; init; } = [];

    /// <summary>
    /// Fecha de registro del vehículo.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Fecha de última actualización de la información del vehículo.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? LastUpdateDate { get; set; }

    [BsonIgnore]
    public bool IsOutOfService => Status is not
        VehicleStatus.Operative;
}
