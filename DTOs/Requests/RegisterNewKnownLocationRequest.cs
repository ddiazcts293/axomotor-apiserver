using System.ComponentModel.DataAnnotations;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterNewKnownLocationRequest
{
    [MaxLength(64)]
    public required string Name { get; set; }

    [MaxLength(256)]
    public required string Address { get; set; }

    public required decimal Longitude { get; set; }

    public required decimal Latitude { get; set; }

    public required decimal Ratio { get; set; }
}
