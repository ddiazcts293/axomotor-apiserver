using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa un incidente registrado.
/// </summary>
public class Incident
{
    /// <summary>
    /// Identificador del incidente.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Identificador del viaje asociado al incidente.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Trip { get; set; }

    /// <summary>
    /// Razón del incidente.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tipo de incidente.
    /// </summary>
    public IncidentType Type { get; set; }

    /// <summary>
    /// Estado del incidente.
    /// </summary>
    public IncidentStatus Status { get; set; }

    /// <summary>
    /// Prioridad del incidente.
    /// </summary>
    public IncidentPriority Priority { get; set; }

    /// <summary>
    /// Última posición conocida cuando se registro el incidente.
    /// </summary>
    public TripPosition? LastKnownPosition { get; set; }

    /// <summary>
    /// Descripción del incidente dada por el conductor sobre el incidente.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Comentarios adicionales acerca del incidente.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Incidente relacionado.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public string? RelatedIncident { get; set; }

    /// <summary>
    /// Lista de fotografías adjuntas al incidente.
    /// </summary>
    public IList<IncidentPicture> Pictures { get; set; } = [];

    /// <summary>
    /// Fecha y hora de cración del incidente.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset CreationDate { get; set; }

    /// <summary>
    /// Fecha y hora de última actualización del incidente.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? LastUpdateDate { get; set; }

    /// <summary>
    /// Identificador del agente que revisó el incidente.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public string? RevisedBy { get; set; }

    /// <summary>
    /// Identificador del agente que cerró el incidente.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ClosedBy { get; set; }

    [BsonIgnore]
    public bool IsClosedOrDiscarded => Status is
        IncidentStatus.Closed or
        IncidentStatus.Discarded;
}
