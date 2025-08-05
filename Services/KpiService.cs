using AxoMotor.ApiServer.Config;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Mqtt;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Catalog;
using AxoMotor.ApiServer.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
//using MongoDB.Driver.Linq;

namespace AxoMotor.ApiServer.Services;

public class KpiService
{
    private readonly AxoMotorContext _dbContext;
    private readonly IMongoCollection<Trip> _trips;
    private readonly IMongoCollection<Incident> _incidents;
    private readonly IMongoCollection<DeviceEvent> _events;

    public KpiService(IOptions<MongoDBSettings> options, AxoMotorContext context)
    {
        var names = options.Value.Collections;
        var client = new MongoClient(options.Value.ConnectionUri);
        var database = client.GetDatabase(options.Value.DatabaseName);

        _trips = database.GetCollection<Trip>(names.Trips);
        _incidents = database.GetCollection<Incident>(names.Incidents);
        _events = database.GetCollection<DeviceEvent>(names.DeviceEvents);
        _dbContext = context;
    }

    public async Task<KPIValueSet> GetKpiValues(CancellationToken cancellationToken)
    {
        var kpiCatalog = await _dbContext.KPICatalog.ToListAsync(cancellationToken);
        DateTimeOffset startUtc = DateTime.Today.ToUniversalTime();
        DateTimeOffset endUtc = DateTime.Now.ToUniversalTime();

        // filtro de viajes completados el día de hoy
        var tripsFilter = Builders<Trip>.Filter.And(
            Builders<Trip>.Filter.Eq(x => x.Status, TripStatus.Finished),
            Builders<Trip>.Filter.Gte(x => x.LastUpdateDate, startUtc),
            Builders<Trip>.Filter.Lte(x => x.LastUpdateDate, endUtc)
        );

        // filtro de incidencias iniciadas hoy
        var incidentsFilter = Builders<Incident>.Filter.And(
            Builders<Incident>.Filter.Gte(x => x.RegistrationDate, startUtc),
            Builders<Incident>.Filter.Lte(x => x.RegistrationDate, endUtc)
        );

        // filtro de incidencias cerradas hoy
        var incidentsClosedFilter = Builders<Incident>.Filter.And(
            Builders<Incident>.Filter.Gte(x => x.RevisionDate, startUtc),
            Builders<Incident>.Filter.Lte(x => x.RevisionDate, endUtc),
            Builders<Incident>.Filter.Eq(x => x.Status, IncidentStatus.Closed)
        );

        // filtro de eventos de activaciones del botón de pánico producidas hoy
        var panicBtnActivationsFilter = Builders<DeviceEvent>.Filter.And(
            Builders<DeviceEvent>.Filter.Gte(x => x.Timestamp, startUtc),
            Builders<DeviceEvent>.Filter.Lte(x => x.Timestamp, endUtc),
            Builders<DeviceEvent>.Filter.Eq(x => x.Code, DeviceEventCode.PanicButtonPressed)
        );

        // filtro de eventos de eventos de conducción brusca producidos hoy
        var harshDrivingEventsFilter = Builders<DeviceEvent>.Filter.And(
            Builders<DeviceEvent>.Filter.Gte(x => x.Timestamp, startUtc),
            Builders<DeviceEvent>.Filter.Lte(x => x.Timestamp, endUtc),
            Builders<DeviceEvent>.Filter.In(x => x.Code, [
                DeviceEventCode.HarshAcceleration,
                DeviceEventCode.HarshBraking,
                DeviceEventCode.HarshCornering,
                DeviceEventCode.ImpactDetected
            ])
        );

        var incidentProjection = new ProjectionDefinitionBuilder<Incident>()
            .Include(x => x.RegistrationDate)
            .Include(x => x.RevisionDate);
        var incidentsClosed = await _incidents
            .Find(incidentsClosedFilter)
            .Project<Incident>(incidentProjection)
            .ToListAsync(cancellationToken);
        var tripsFinished = await _trips.Find(tripsFilter)
            .ToListAsync(cancellationToken);

        int vehicleCount = await _dbContext.Vehicles
            .CountAsync(cancellationToken);
        int vehicleInUseCount = await _dbContext.Vehicles
            .CountAsync(x => x.InUse, cancellationToken);
        double operativeVehicleCount = await _dbContext.Vehicles
            .CountAsync(x => x.Status == VehicleStatus.Operative, cancellationToken);

        // calcula el porcentaje de vehiculos operativos
        double operativeVehiclesPercentage = vehicleCount > 0 ?
            (operativeVehicleCount / vehicleCount) : 0;

        // calcula la cantidad de viajes completados a tiempo
        int tripFinishedCount = tripsFinished.Count;
        double onTimeTripFinishedCount = tripsFinished.
            Count(x => x.LastUpdateDate <= x.Destination.DateTime);

        // calcula el porcentaje de viajes completados a tiempo
        double onTimeTripsFinishedPercentage = tripFinishedCount > 0 ?
            (onTimeTripFinishedCount / tripFinishedCount) : 0;

        double averageTimeToResolveIncident = incidentsClosed.Count > 0 ?
            incidentsClosed
                .Select(x => x.RevisionDate - x.RegistrationDate)
                .Average(x => x?.Minutes ?? 0) : 0;

        // calcula el número de veces que se activó el botón de pánico
        long panicBtnActivationsCount = await _events
            .Find(panicBtnActivationsFilter)
            .CountDocumentsAsync(cancellationToken);

        // calcula la cantidad de incidentes reportados
        long incidentReportedCount = await _incidents
            .Find(incidentsFilter)
            .CountDocumentsAsync(cancellationToken);

        // calcula el número de eventos bruscos de conducción 
        long harshDrivingCount = await _events
            .Find(harshDrivingEventsFilter)
            .CountDocumentsAsync(cancellationToken);

        return new()
        {
            VehicleCount = vehicleCount,
            VehiclesInUseCount = vehicleInUseCount,
            OperativeVehiclesPercentage = new()
            {
                Value = operativeVehiclesPercentage,
                Level = GetKPILevel("OperativeVehiclesPercentage", operativeVehiclesPercentage)
            },
            OnTimeTripsCompletedPercentage = new()
            {
                Value = onTimeTripsFinishedPercentage,
                Level = GetKPILevel("OnTimeTripsCompletedPercentage", onTimeTripsFinishedPercentage)
            },
            AverageTimeToResolveIncident = new()
            {
                Value = averageTimeToResolveIncident,
                Level = GetKPILevel("AverageTimeToResolveIncident", averageTimeToResolveIncident)
            },
            PanicButtonActivations = new()
            {
                Value = panicBtnActivationsCount,
                Level = GetKPILevel("PanicButtonActivations", panicBtnActivationsCount)
            },
            IncidentsReported = new()
            {
                Value = incidentReportedCount,
                Level = GetKPILevel("IncidentsReported", incidentReportedCount)
            },
            HarshDrivingEvents = new()
            {
                Value = harshDrivingCount,
                Level = GetKPILevel("HarshDrivingEvents", harshDrivingCount)
            }
        };

        KPILevel GetKPILevel(string name, double value)
        {
            KPI kpi = kpiCatalog.Single(x => x.Code == name);

            if (kpi.Type != KPIType.Value)
            {
                if (kpi.Type == KPIType.Percent) value *= 100.0;

                if (kpi.Inverted)
                {
                    if (value <= kpi.OptimalValue && value > kpi.GoodValue)
                        return KPILevel.Optimal;
                    else if (value <= kpi.GoodValue && value > kpi.AcceptableValue)
                        return KPILevel.Good;
                    else if (value <= kpi.AcceptableValue && value > kpi.BadValue)
                        return KPILevel.Acceptable;
                    else if (value <= kpi.BadValue && value > kpi.CriticalValue)
                        return KPILevel.Bad;
                    else if (value <= kpi.CriticalValue)
                        return KPILevel.Critical;
                    else
                        return KPILevel.NotData;
                }
                else
                {
                    if (value >= kpi.OptimalValue && value < kpi.GoodValue)
                        return KPILevel.Optimal;
                    else if (value >= kpi.GoodValue && value < kpi.AcceptableValue)
                        return KPILevel.Good;
                    else if (value >= kpi.AcceptableValue && value < kpi.BadValue)
                        return KPILevel.Acceptable;
                    else if (value >= kpi.BadValue && value < kpi.CriticalValue)
                        return KPILevel.Bad;
                    else if (value >= kpi.CriticalValue)
                        return KPILevel.Critical;
                    else
                        return KPILevel.NotData;
                }
            }
            else
            {
                if (kpi.Inverted)
                {
                    if (value <= kpi.OptimalValue && value > kpi.AcceptableValue)
                        return KPILevel.Optimal;
                    else if (value <= kpi.AcceptableValue && value > kpi.CriticalValue)
                        return KPILevel.Acceptable;
                    else if (value <= kpi.CriticalValue)
                        return KPILevel.Critical;
                    else
                        return KPILevel.NotData;
                }
                else
                {
                    if (value >= kpi.OptimalValue && value < kpi.AcceptableValue)
                        return KPILevel.Optimal;
                    else if (value >= kpi.AcceptableValue && value < kpi.CriticalValue)
                        return KPILevel.Acceptable;
                    else if (value >= kpi.CriticalValue)
                        return KPILevel.Critical;
                    else
                        return KPILevel.NotData;
                }
            }
        }
    }
}
