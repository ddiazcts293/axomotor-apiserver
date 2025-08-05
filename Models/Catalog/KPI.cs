using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.Models.Catalog;

public class KPI : ModelBase
{
    public required KPIType Type { get; set; }

    public double OptimalValue { get; set; }

    public double? GoodValue { get; set; }

    public double AcceptableValue { get; set; }

    public double? BadValue { get; set; }

    public double CriticalValue { get; set; }

    public bool Inverted { get; set; }
}
