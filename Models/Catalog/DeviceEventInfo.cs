using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

public class DeviceEventInfo : ModelBase
{
    public string? Description { get; set; }

    public required DeviceEventSeverity Severity { get; set; }

    public required DeviceEventType Type { get; set; }
}
