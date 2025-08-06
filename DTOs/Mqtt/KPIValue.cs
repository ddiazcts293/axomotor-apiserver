using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Mqtt;

public class KPIValue<T>
{
    public KPIStatus Status { get; set; }
    
    public required T Value { get; set; }
}
