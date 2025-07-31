using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

public class IncidentInfo
{
    public required IncidentCode Code { get; set; }

    public required string DisplayName { get; set; }

    public string? Description { get; set; }

    public required IncidentPriority Priority { get; set; }

    public required IncidentType Type { get; set; }
}
