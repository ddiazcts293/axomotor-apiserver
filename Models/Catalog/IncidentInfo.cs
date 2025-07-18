using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

public class IncidentInfo : ModelBase
{
    public string? Description { get; set; }

    public required IncidentPriority Priority { get; set; }

    public required IncidentType Type { get; set; }
}
