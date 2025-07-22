using AxoMotor.ApiServer.Models;

namespace AxoMotor.ApiServer.DTOs.Common;

public class TripLocationDto : PositionDtoBase
{
    public required string Name { get; set; }

    public required string Address { get; set; }

    public required float Ratio { get; set; }

    public required DateTimeOffset DateTime { get; set; }

    public static implicit operator TripLocationDto?(TripLocation? location)
        => location is null ? null : new()
        {
            Name = location.Name,
            Address = location.Address,
            Latitude = location.Coordinates.Latitude,
            Longitude = location.Coordinates.Longitude,
            DateTime = location.DateTime,
            Ratio = location.Ratio
        };
 
    public static implicit operator TripLocation?(TripLocationDto? dto)
        => dto is null ? null : new()
    {
        Name = dto.Name,
        Address = dto.Address,
        Ratio = dto.Ratio,
        Coordinates = new(dto.Longitude, dto.Latitude),
        DateTime = dto.DateTime
    };

    public static explicit operator TripPlannedStop?(TripLocationDto? dto)
        => dto is null ? null : new()
    {
        Name = dto.Name,
        Address = dto.Address,
        Ratio = dto.Ratio,
        Coordinates = new(dto.Longitude, dto.Latitude),
        DateTime = dto.DateTime
    };
}
