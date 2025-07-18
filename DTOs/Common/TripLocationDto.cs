namespace AxoMotor.ApiServer.DTOs.Common;

public class TripLocationDto
{
    public required string Name { get; set; }

    public required string Address { get; set; }

    public required float Latitude { get; set; }

    public required float Longitude { get; set; }

    public required float Ratio { get; set; }

    public required DateTimeOffset DateTime { get; set; }
}
