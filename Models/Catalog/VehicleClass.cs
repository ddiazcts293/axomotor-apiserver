namespace AxoMotor.ApiServer.Models.Catalog;

public class VehicleClass : ModelBase
{
    public static implicit operator string(VehicleClass @class)
        => @class.Code;
}
