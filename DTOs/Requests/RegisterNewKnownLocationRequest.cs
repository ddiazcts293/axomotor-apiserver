using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterNewKnownLocationRequest
{
    [MaxLength(Constants.MaxKnownLocationNameLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Name { get; set; }

    [MaxLength(Constants.MaxKnownLocationAddressLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Address { get; set; }

    [Range(-180, 180)]
    public required decimal Longitude { get; set; }

    [Range(-90, 90)]
    public required decimal Latitude { get; set; }

    [Range(0, Constants.MaxRatioValue)]
    public required decimal Ratio { get; set; }
}
