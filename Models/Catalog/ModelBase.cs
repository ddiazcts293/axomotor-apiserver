namespace AxoMotor.ApiServer.Models.Catalog;

public abstract class ModelBase
{
    public required string Code { get; set; }

    public required string DisplayName { get; set; }
}
