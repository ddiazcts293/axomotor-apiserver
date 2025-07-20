using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class IncidentDto
{
    /// <summary>
    /// Identificador del incidente.
    /// </summary>
    public required string IncidentId { get; set; }

    /// <summary>
    /// Identificador del viaje asociado al incidente.
    /// </summary>
    public required string TripId { get; set; }

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
    public TripPositionDto? LastKnownPosition { get; set; }

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
    public string? RelatedIncidentId { get; set; }

    /// <summary>
    /// Lista de fotografías adjuntas al incidente.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<IncidentPictureDto>? Pictures { get; set; }

    /// <summary>
    /// Fecha y hora de cración del incidente.
    /// </summary>
    public DateTimeOffset CreationDate { get; set; }

    /// <summary>
    /// Fecha y hora de última actualización del incidente.
    /// </summary>
    public DateTimeOffset? LastUpdateDate { get; set; }

    /// <summary>
    /// Identificador del agente que revisó el incidente.
    /// </summary>
    public string? RevisedById { get; set; }

    /// <summary>
    /// Identificador del agente que cerró el incidente.
    /// </summary>
    public string? ClosedById { get; set; }

    public static IncidentDto Convert(Incident incident)
    {
        var dto = new IncidentDto()
        {
            IncidentId = incident.Id,
            TripId = incident.Trip,
            Code = incident.Code,
            Type = incident.Type,
            Status = incident.Status,
            Priority = incident.Priority,
            Description = incident.Description,
            Comments = incident.Comments,
            RelatedIncidentId = incident.RelatedIncident,
            CreationDate = incident.CreationDate,
            LastUpdateDate = incident.LastUpdateDate,
            RevisedById = incident.RevisedBy,
            ClosedById = incident.ClosedBy
        };

        if (incident.LastKnownPosition is not null)
        {
            dto.LastKnownPosition = TripPositionDto.Convert(incident.LastKnownPosition);
        }

        if (incident.Pictures is not null && incident.Pictures.Any())
        {
            dto.Pictures = [.. incident.Pictures
                .Select(x => new IncidentPictureDto()
                {
                    FileId = x.FileId,
                    Position = TripPositionDto.Convert(x.Position)
                })
            ];
        }

        return dto;
    }
}
