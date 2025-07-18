namespace AxoMotor.ApiServer.Models.Catalog;

public class KnownLocation
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Address { get; set; }

    public required decimal Longitude { get; set; }

    public required decimal Latitude { get; set; }

    public required decimal Ratio { get; set; }
}
