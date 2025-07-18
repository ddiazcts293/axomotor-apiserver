using System.Text.Json.Serialization;
using AxoMotor.ApiServer.DTOs.Common;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class CreateTripRequest
{
    /// <summary>
    /// Identificador del conductor asociado.
    /// </summary>
    public required string DriverId { get; set; }

    /// <summary>
    /// Identificador del vehículo asociado.
    /// </summary>
    public required string VehicleId { get; set; }

    /// <summary>
    /// Punto de partida del viaje.
    /// </summary>
    public required TripLocationDto StartingPoint { get; set; }

    /// <summary>
    /// Destino final del viaje.
    /// </summary>
    public required TripLocationDto FinalDestination { get; set; }

    /// <summary>
    /// Lista de paradas planeadas durante el viaje.
    /// </summary>
    public IList<TripLocationDto>? PlannedStops { get; set; }
}
