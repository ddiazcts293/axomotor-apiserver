using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

public class KPI : ModelBase
{
    public required KPIType Type { get; set; }

    public decimal OptimalValue { get; set; }

    public decimal? LowValue { get; set; }

    public decimal AcceptableValue { get; set; }

    public decimal? HighValue { get; set; }

    public decimal CriticalValue { get; set; }
}
