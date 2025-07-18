using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Responses;

public class CreateTripResponse
{
    public required string TripId { get; set; }
    
    public TripStatus Status { get; set; }
}
