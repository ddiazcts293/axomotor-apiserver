using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Responses;

public class RegisterNewKnownLocationResponse
{
    public required int LocationId { get; set; }
}
