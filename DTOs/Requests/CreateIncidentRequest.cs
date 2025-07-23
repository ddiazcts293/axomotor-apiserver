using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.Helpers;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class CreateIncidentRequest
{
    [Required(AllowEmptyStrings = false)]
    [ValidateObjectId]
    public required string TripId { get; set; }

    [MaxLength(Constants.MaxIncidentCodeLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Code { get; set; }

    [MaxLength(Constants.MaxIncidentDescriptionLength)]
    public string? Description { get; set; }

    [ValidateObjectId]
    public string? RelatedIncidentId { get; set; }

    [MaxLength(Constants.MaxIncidentPictureCount)]
    public IList<IncidentPictureDto>? Pictures { get; set; }
}
