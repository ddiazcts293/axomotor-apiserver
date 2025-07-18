using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Responses;

public class CreateIncidentResponse
{
    public required string IncidentId { get; set; }

    public IncidentType Type { get; set; }

    public IncidentPriority Priority { get; set; }

    public IncidentStatus Status { get; set; }
}
