using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

public class DeviceEventInfo
{
    public required DeviceEventCode Code { get; set; }

    public required string DisplayName { get; set; }

    public string? Description { get; set; }

    public required DeviceEventSeverity Severity { get; set; }

    public required DeviceEventType Type { get; set; }
}
