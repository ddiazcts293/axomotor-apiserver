using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateTripRequest
{
    /// <summary>
    /// Identificador del conductor asociado.
    /// </summary>
    public int? DriverId { get; set; }

    /// <summary>
    /// Identificador del vehículo asociado.
    /// </summary>
    public int? VehicleId { get; set; }
    
    /// <summary>
    /// Estado del viaje.
    /// </summary>
    public TripStatus? Status { get; set; }
    
    /// <summary>
    /// Punto de partida del viaje.
    /// </summary>
    public TripLocationDto? Origin { get; set; }

    /// <summary>
    /// Destino final del viaje.
    /// </summary>
    public TripLocationDto? Destination { get; set; }

    /// <summary>
    /// Lista de paradas planeadas durante el viaje.
    /// </summary>
    public IList<TripLocationDto>? PlannedStops { get; set; }
}
