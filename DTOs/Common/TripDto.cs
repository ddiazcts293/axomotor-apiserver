using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class TripDto
{
    public required string TripId { get; set; }

    public required int Number { get; set; }

    public SimpleUserAccountDto? Driver { get; set; }

    public SimpleVehicleDto? Vehicle { get; set; }

    public required TripStatus Status { get; set; }

    public required TripLocationDto Origin { get; set; }

    public required TripLocationDto Destination { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IList<TripPlannedStopDto>? PlannedStops { get; set; }

    public TripStats? Stats { get; set; }

    public SimpleUserAccountDto? CreatedBy { get; set; }

    public required DateTimeOffset CreationDate { get; set; }

    public SimpleUserAccountDto? UpdatedBy { get; set; }

    public required DateTimeOffset? LastUpdateDate { get; set; }
}
