using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models;

namespace AxoMotor.ApiServer.DTOs.Common;

public class TripPlannedStopDto : TripLocationDto
{
    [DataType(DataType.Duration)]
    public TimeSpan? EstimatedDuration { get; set; }

    [DataType(DataType.Duration)]
    public TimeSpan? ActualDuration { get; set; }

    public bool? IsCompleted { get; set; }

    public static implicit operator TripPlannedStopDto?(TripPlannedStop? stop)
        => stop is null ? null : new()
        {
            Name = stop.Name,
            Address = stop.Address,
            Ratio = stop.Ratio,
            Latitude = stop.Coordinates.Latitude,
            Longitude = stop.Coordinates.Longitude,
            DateTime = stop.DateTime,
            ActualDuration = stop.ActualDuration,
            EstimatedDuration = stop.EstimatedDuration,
            IsCompleted = stop.IsCompleted
        };
}
