using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models;

/// <summary>
/// Representa una incidencia registrada.
/// </summary>
public class Incident
{
    /// <summary>
    /// Identificador de la incidencia.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    /// <summary>
    /// Número de incidencia.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Identificador del viaje asociado a la incidencia.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public required string TripId { get; set; }

    /// <summary>
    /// Razón de la incidencia.
    /// </summary>
    public required string Code { get; set; }

    /// <summary>
    /// Tipo de incidencia.
    /// </summary>
    public IncidentType Type { get; set; }

    /// <summary>
    /// Estado de la incidencia.
    /// </summary>
    public IncidentStatus Status { get; set; }

    /// <summary>
    /// Prioridad de la incidencia.
    /// </summary>
    public IncidentPriority Priority { get; set; }

    /// <summary>
    /// Última posición conocida cuando se registro la incidencia.
    /// </summary>
    public TripPosition? LastKnownPosition { get; set; }

    /// <summary>
    /// Descripción de la incidencia dada por el conductor.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Comentarios adicionales acerca de la incidencia.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Incidente relacionado.
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public string? RelatedIncidentId { get; set; }

    /// <summary>
    /// Lista de fotografías adjuntas a la incidencia.
    /// </summary>
    public IList<IncidentPicture>? Pictures { get; set; }

    /// <summary>
    /// Fecha y hora de registro de la incidencia.
    /// </summary>
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Fecha y hora de revisión de la incidencia.
    /// </summary>
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset? RevisionDate { get; set; }

    /// <summary>
    /// Identificador de usuario que registro la incidencia.
    /// </summary>
    public int RegisteredById { get; set; }

    /// <summary>
    /// Identificador del agente que revisó la incidencia.
    /// </summary>
    public int? RevisedById { get; set; }

    /// <summary>
    /// Identificador del agente que cerró la incidencia.
    /// </summary>
    public int? ClosedById { get; set; }

    #region BSON ignore

    /// <summary>
    /// Indica si la incidencia fue cerrada o descartada.
    /// </summary>
    [BsonIgnore]
    public bool WasClosedOrDiscarded => Status is
        IncidentStatus.Closed or
        IncidentStatus.Discarded;

    #endregion
}
