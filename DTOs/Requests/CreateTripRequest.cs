using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class CreateTripRequest
{
    /// <summary>
    /// Identificador del conductor asociado.
    /// </summary>
    public required int DriverId { get; set; }

    /// <summary>
    /// Identificador del vehículo asociado.
    /// </summary>
    public required int VehicleId { get; set; }

    /// <summary>
    /// Punto de partida del viaje.
    /// </summary>
    public required TripLocationDto Origin { get; set; }

    /// <summary>
    /// Destino final del viaje.
    /// </summary>
    public required TripLocationDto Destination { get; set; }

    /// <summary>
    /// Lista de paradas planeadas durante el viaje.
    /// </summary>
    [Range(0, Constants.MaxTripPlannedStopCount)]
    public IList<TripLocationDto>? PlannedStops { get; set; }
}
