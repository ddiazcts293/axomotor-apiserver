using AxoMotor.ApiServer.Models;

namespace AxoMotor.ApiServer.DTOs.Common;

public class TripLocationDto : PositionDtoBase
{
    public required string Name { get; set; }

    public required string Address { get; set; }

    public required float Ratio { get; set; }

    public required DateTimeOffset DateTime { get; set; }

    public TimeSpan? Duration { get; set; }

    public static TripLocationDto Convert(TripLocation location)
    {
        return new()
        {
            Name = location.Name,
            Address = location.Name,
            Latitude = location.Coordinates.Latitude,
            Longitude = location.Coordinates.Longitude,
            Ratio = location.Ratio,
            DateTime = location.DateTime,
            Duration = location.Duration
        };
    }
}
