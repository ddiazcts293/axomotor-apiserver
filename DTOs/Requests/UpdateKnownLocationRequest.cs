using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateKnownLocationRequest
{
    [Length(1, Constants.MaxKnownLocationNameLength)]
    public string? Name { get; set; }

    [Length(1, Constants.MaxKnownLocationAddressLength)]
    public string? Address { get; set; }

    [Range(-180, 180)]
    public decimal? Longitude { get; set; }

    [Range(-90, 90)]
    public decimal? Latitude { get; set; }

    [Range(0, Constants.MaxRatioValue)]
    public decimal? Ratio { get; set; }
}
