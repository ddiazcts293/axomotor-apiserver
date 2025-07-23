using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models;

namespace AxoMotor.ApiServer.DTOs.Common;

/// <summary>
/// Representa una fotografía de una incidencia.
/// </summary>
public class IncidentPictureDto
{
    /// <summary>
    /// Identificador del archivo.
    /// </summary>
    [Required]
    public required string FileId { get; set; }

    /// <summary>
    /// Marca de tiempo del archivo.
    /// </summary>
    [Required]
    public DateTimeOffset Timestamp { get; set; }

    public static implicit operator IncidentPicture?(IncidentPictureDto? dto)
        => dto is null ? null : new()
        {
            FileId = dto.FileId,
            Timestamp = dto.Timestamp
        };
}
