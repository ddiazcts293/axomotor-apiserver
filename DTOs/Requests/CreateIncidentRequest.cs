using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.DTOs.Common;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class CreateIncidentRequest
{
    public required string TripId { get; set; }

    [Length(1, 24)]
    public required string Code { get; set; }

    public required TripPositionDto LastKnownPosition { get; set; }

    public string? Description { get; set; }

    public string? RelatedIncidentId { get; set; }

    public IList<IFormFile>? Pictures { get; set; }
}
