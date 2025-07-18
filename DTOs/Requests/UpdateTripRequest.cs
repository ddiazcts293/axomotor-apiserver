using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateTripRequest
{
    public string? DriverId { get; set; }

    public string? VehicleId { get; set; }

    public TripStatus? Status { get; set; }

    public TripLocationDto? StartingPoint { get; set; }

    public TripLocationDto? FinalDestination { get; set; }

    public IList<TripLocationDto>? PlannedStops { get; set; }
}
