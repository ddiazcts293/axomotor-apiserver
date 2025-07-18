using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateVehicleRequest
{
    [Length(8, 10)]
    public string? PlateNumber { get; set; }

    public VehicleStatus? Status { get; set; }
}
