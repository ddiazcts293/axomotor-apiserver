using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Responses;

public class RegisterVehicleResponse
{
    public int VehicleId { get; set; }

    public VehicleStatus Status { get; set; }
}
