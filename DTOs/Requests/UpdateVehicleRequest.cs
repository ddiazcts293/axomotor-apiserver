using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateVehicleRequest
{
    [Length(8, 10)]
    public string? PlateNumber { get; set; }

    [Length(16, 22)]
    public string? RegistrationNumber { get; set; }

    public VehicleStatus? Status { get; set; }
}
