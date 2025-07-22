using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateVehicleRequest
{
    [Length(Constants.MinVehiclePlateNumberLength, Constants.MaxVehiclePlateNumberLength)]
    public string? PlateNumber { get; set; }

    [MaxLength(Constants.MaxVehicleRegNumberLength)]
    public string? RegistrationNumber { get; set; }

    public VehicleStatus? Status { get; set; }
}
