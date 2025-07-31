using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class IncidentDto
{
    /// <summary>
    /// Identificador de la incidencia.
    /// </summary>
    public required string IncidentId { get; set; }

    /// <summary>
    /// Número de incidencia.
    /// </summary>
    public required int Number { get; set; }

    /// <summary>
    /// Identificador del viaje asociado a la incidencia.
    /// </summary>
    public required string TripId { get; set; }

    /// <summary>
    /// Razón de la incidencia.
    /// </summary>
    public required IncidentCode Code { get; set; }

    /// <summary>
    /// Tipo de incidencia.
    /// </summary>
    public required IncidentType Type { get; set; }

    /// <summary>
    /// Estado de la incidencia.
    /// </summary>
    public required IncidentStatus Status { get; set; }

    /// <summary>
    /// Prioridad de la incidencia.
    /// </summary>
    public required IncidentPriority Priority { get; set; }

    /// <summary>
    /// Última posición conocida cuando se registro la incidencia.
    /// </summary>
    public TripPositionDto? LastKnownPosition { get; set; }

    /// <summary>
    /// Descripción de la incidencia dada por el conductor sobre la incidencia.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Comentarios adicionales acerca de la incidencia.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Incidente relacionado.
    /// </summary>
    public string? RelatedIncidentId { get; set; }

    /// <summary>
    /// Lista de fotografías adjuntas a la incidencia.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<IncidentPicture>? Pictures { get; set; }

    /// <summary>
    /// Fecha y hora de cración de la incidencia.
    /// </summary>
    public required DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Fecha y hora de revisión de la incidencia.
    /// </summary>
    public DateTimeOffset? RevisionDate { get; set; }

    /// <summary>
    /// Identificador de usuario que registro la incidencia.
    /// </summary>
    public SimpleUserAccountDto? RegisteredBy { get; set; }

    /// <summary>
    /// Identificador del agente que revisó la incidencia.
    /// </summary>
    public SimpleUserAccountDto? RevisedBy { get; set; }

    /// <summary>
    /// Identificador del agente que cerró la incidencia.
    /// </summary>
    public SimpleUserAccountDto? ClosedBy { get; set; }
}
