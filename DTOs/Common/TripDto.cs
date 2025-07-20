using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class TripDto
{
    public required string TripId { get; set; }

    public required string DriverId { get; set; }

    public required string VehicleId { get; set; }

    public required TripStatus Status { get; set; }

    public required TripLocationDto StartingPoint { get; set; }

    public required TripLocationDto FinalDestination { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<TripLocationDto>? PlannedStops { get; set; }

    public required IList<TripPositionDto> RecordedPositions { get; set; }

    public TripStats? Stats { get; set; }

    public required string CreatedById { get; set; }

    public DateTimeOffset CreationDate { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public static TripDto Convert(Trip trip) => new()
    {
        TripId = trip.Id,
        DriverId = trip.Driver,
        VehicleId = trip.Vehicle,
        Status = trip.Status,
        StartingPoint = TripLocationDto.Convert(trip.StartingPoint),
        FinalDestination = TripLocationDto.Convert(trip.FinalDestination),
        PlannedStops = trip.PlannedStops?.Select(TripLocationDto.Convert).ToList(),
        RecordedPositions = trip.RecordedPositions.Select(TripPositionDto.Convert).ToList(),
        Stats = trip.Stats,
        CreatedById = trip.CreatedBy,
        CreationDate = trip.CreationDate,
        LastUpdateDate = trip.LastUpdateDate
    };
}
