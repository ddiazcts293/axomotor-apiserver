using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateIncidentRequest
{
    public IncidentStatus? Status { get; set; }

    public IncidentPriority? Priority { get; set; }

    public string? Comments { get; set; }

    public string? RelatedIncident { get; set; }

    public IList<IFormFile>? PicturesToAdd { get; set; }

    public IList<string>? PicturesToDelete { get; set; }
}
