using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Responses;

public class RegisterVehicleResponse
{
    public required string VehicleId { get; set; }

    public VehicleStatus Status { get; set; }
}
