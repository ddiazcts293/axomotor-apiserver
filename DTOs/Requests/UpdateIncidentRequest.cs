using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateIncidentRequest
{
    public IncidentStatus? Status { get; set; }

    public IncidentPriority? Priority { get; set; }

    [MaxLength(Constants.MaxIncidentCommentsLength)]
    public string? Comments { get; set; }

    public string? RelatedIncident { get; set; }

    [MaxLength(Constants.MaxIncidentPictureCount)]
    public IList<string>? PicturesToAdd { get; set; }

    [MaxLength(Constants.MaxIncidentPictureCount)]
    public IList<string>? PicturesToDelete { get; set; }
}
