using System.Text.Json.Serialization;

namespace AxoMotor.ApiServer.DTOs.Mqtt;

public class KPIValueSet
{
    public int VehicleCount { get; set; }

    public int VehiclesInUseCount { get; set; }

    public required KPIValue<double> OperativeVehiclesPercentage { get; set; }

    public required KPIValue<double> OnTimeTripsCompletedPercentage { get; set; }

    public required KPIValue<double> AverageTimeToResolveIncident { get; set; }

    public required KPIValue<long> PanicButtonActivations { get; set; }

    public required KPIValue<long> IncidentsReported { get; set; }

    public required KPIValue<long> HarshDrivingEvents { get; set; }
}
