using System.ComponentModel.DataAnnotations;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateKnownLocationRequest
{
    [MaxLength(64)]
    public string? Name { get; set; }

    [MaxLength(256)]
    public string? Address { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Ratio { get; set; }
}
