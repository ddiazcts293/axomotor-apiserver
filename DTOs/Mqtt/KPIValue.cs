using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Mqtt;

public class KPIValue<T>
{
    public KPILevel Level { get; set; }
    
    public required T Value { get; set; }
}
