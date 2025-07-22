using System.ComponentModel.DataAnnotations;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterVehicleRequest
{
    [Length(8, 10)]
    public required string PlateNumber { get; set; }

    [Length(16, 22)]
    public required string RegistrationNumber { get; set; }

    [Length(3, 20)]
    public required string Brand { get; set; }

    [Length(3, 20)]
    public required string Model { get; set; }

    [Length(1, 20)]
    public required string Class { get; set; }

    public required int Year { get; set; }
}
