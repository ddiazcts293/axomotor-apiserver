using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Catalog;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Controllers;

[Route("api/trips")]
[ProducesResponseType<BasicResponse>(200)]
[ProducesResponseType<ErrorResponse>(400)]
[ProducesResponseType<ErrorResponse>(500)]
public class TripsController
(
    TripService tripService,
    PositionService positionService,
    AxoMotorContext context
) : ApiControllerBase
{
    private readonly TripService _tripService = tripService;
    private readonly PositionService _positionService = positionService;
    private readonly AxoMotorContext _context = context;

    [HttpPost]
    [ProducesResponseType<GenericResponse<CreateTripResponse>>(200)]
    public async Task<IActionResult> Create(CreateTripRequest request)
    {
        try
        {
            // inicia una transacción
            await using var transaction = await _context.Database
                .BeginTransactionAsync();

            // obtiene la cuenta asociada al identificador de conductor especificado
            var account = await _context.UserAccounts.FindAsync(request.DriverId);
            // obtiene la cuenta asociada al identificador de vehículo especificado
            var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);

            // confirma la transacción
            await transaction.CommitAsync();

            if (account is null)
                return ApiError(ApiResultCode.NotFound, "Driver was not found");
            if (account.Role != UserAccountRole.Driver)
                return ApiError(ApiResultCode.InvalidState, "User is not a driver");
            if (account.Status != UserAccountStatus.Enabled)
                return ApiError(ApiResultCode.InvalidState, "User account is disabled");
            if (vehicle is null)
                return ApiError(ApiResultCode.NotFound, "Vehicle was not found");
            if (vehicle.Status is VehicleStatus.HeldByAuthorities)
                return ApiError(ApiResultCode.InvalidState, "Vehicle is held by authorities");
            if (vehicle.Status is VehicleStatus.OutOfService)
                return ApiError(ApiResultCode.InvalidState, "Vehicle is out of service");

            // TODO: verificar si el conductor o vehiculo están asignados a 
            // otro viaje en la misma fecha

            Trip trip = new()
            {
                VehicleId = request.VehicleId,
                DriverId = request.DriverId,
                Origin = request.Origin!,
                Destination = request.Destination!,
                CreatedById = request.DriverId, // TODO: obtener Id de quien realiza la acción
                PlannedStops = request.PlannedStops?
                    .Select(x => (TripPlannedStop)x!)
                    .ToList(),
            };

            await _tripService.CreateAsync(trip);

            var response = new CreateTripResponse()
            {
                TripId = trip.Id,
                Status = trip.Status
            };

            return ApiSuccess(response);
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpGet]
    [ProducesResponseType<GenericResponse<ResultCollection<TripDto>>>(200)]
    public async Task<IActionResult> Get(
        int? driverId,
        int? vehicleId,
        int? cratedById,
        int? updateById,
        string? originName,
        string? destinationName,
        TripStatus? status)
    {
        try
        {
            // obtiene la lista de usuarios en mysql
            var vehicles = await _context.Vehicles.ToListAsync();
            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();
            // obtiene la lista de viajes en mongodb
            var trips = await _tripService.GetAsync(
                driverId,
                vehicleId,
                cratedById,
                updateById,
                originName,
                destinationName,
                status
            );

            var results = trips.Select(x => new TripDto
            {
                TripId = x.Id,
                Number = x.Number,
                Status = x.Status,
                Origin = x.Origin!,
                Destination = x.Destination!,
                CreationDate = x.CreationDate,
                LastUpdateDate = x.LastUpdateDate,
                Stats = x.Stats,
                Vehicle = vehicles.SingleOrDefault(v => v.Id == x.VehicleId),
                Driver = users.SingleOrDefault(u => u.Id == x.DriverId),
                CreatedBy = users.SingleOrDefault(u => u.Id == x.CreatedById),
                UpdatedBy = users.SingleOrDefault(u => u.Id == x.UpdatedById),
            });

            return ApiSuccessCollection(results);
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpGet("{tripId}")]
    [ProducesResponseType<GenericResponse<TripDto>>(200)]
    public async Task<IActionResult> Get(string tripId)
    {
        try
        {
            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();
            // obtiene el viaje asociado
            var trip = await _tripService.GetAsync(tripId);
            if (trip is null)
                return ApiError(ApiResultCode.NotFound);

            var result = new TripDto
            {
                TripId = trip.Id,
                Number = trip.Number,
                Status = trip.Status,
                Origin = trip.Origin!,
                Destination = trip.Destination!,
                CreationDate = trip.CreationDate,
                LastUpdateDate = trip.LastUpdateDate,
                Stats = trip.Stats,
                Driver = users.SingleOrDefault(u => u.Id == trip.DriverId),
                CreatedBy = users.SingleOrDefault(u => u.Id == trip.CreatedById),
                UpdatedBy = users.SingleOrDefault(u => u.Id == trip.UpdatedById),
                Vehicle = await _context.Vehicles
                    .SingleOrDefaultAsync(v => v.Id == trip.VehicleId),
                PlannedStops = trip.PlannedStops?
                    .Select(p => (TripPlannedStopDto)p!)
                    .ToList() ?? []
            };

            return ApiSuccess(result);
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpPut("{tripId}")]
    public async Task<IActionResult> Update(string tripId, UpdateTripRequest request)
    {
        try
        {
            // TODO: el coductor solo puede cambiar el estado del viaje
            // para iniciar, terminar, abortar, detener, pausar
            // el agente puede establecer los demás

            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(tripId);
            if (trip is null)
                return ApiError(ApiResultCode.NotFound, "Trip not found");

            // verifica si el viaje está finalizado
            if (trip.IsFinished)
                return ApiError(ApiResultCode.InvalidState, "Trip was finished");

            if (request.DriverId is not null)
            {
                // obtiene la cuenta asociada al identificador de conductor especificado
                var account = await _context.UserAccounts.FindAsync(request.DriverId);
                if (account is null)
                    return ApiError(ApiResultCode.NotFound, "Driver not found");
                if (account.Role != UserAccountRole.Driver)
                    return ApiError(ApiResultCode.InvalidState, "User is not a driver");
                if (account.Status != UserAccountStatus.Enabled)
                    return ApiError(ApiResultCode.InvalidState, "User account disabled");

                // TODO: verificar que el conductor no esté asignado a otro viaje
                // (excluyendo el actual)
            }

            if (request.VehicleId is not null)
            {
                // obtiene la cuenta asociada al identificador de vehículo especificado
                var vehicle = await _context.Vehicles.FindAsync(request.VehicleId);
                if (vehicle is null)
                    return ApiError(ApiResultCode.NotFound, "Vehicle not found");
                if (vehicle.IsOutOfService)
                    return ApiError(ApiResultCode.InvalidState, "Vehicle is out of service");

                // TODO: verificar si el vehiculo están asignado a otro viaje
            }

            List<TripPlannedStop>? plannedStops = request.PlannedStops?
                .Select(x => (TripPlannedStop)x!)
                .ToList();

            // actualiza la información del viaje
            bool result = await _tripService.UpdateAsync(
                tripId,
                request.DriverId,
                request.VehicleId,
                request.Status,
                request.Origin,
                request.Destination,
                plannedStops
            // TODO: obtener Id de quien realiza la acción
            );

            if (!result)
                return ApiError(ApiResultCode.Failed);

            return ApiSuccess();
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpDelete("{tripId}")]
    public async Task<IActionResult> Delete(string tripId)
    {
        try
        {
            if (!await _tripService.DeleteAsync(tripId))
                return ApiError(ApiResultCode.NotFound);

            return ApiSuccess();
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpPost("{tripId}/positions")]
    public async Task<IActionResult> PostPosition(string tripId, TripPositionDto position)
    {
        try
        {
            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(tripId.ToString());
            if (trip is null)
                return ApiError(ApiResultCode.NotFound, "Trip does not exist");

            // verifica si el viaje está finalizado
            if (trip.IsFinished)
                return ApiError(ApiResultCode.Failed, "Trip was finished");

            var tripPosition = new TripPosition()
            {
                TripId = tripId,
                Speed = position.Speed,
                Source = position.Source,
                Timestamp = position.Timestamp,
                Coordinates = new(position.Longitude, position.Latitude)
            };

            await _positionService.PushOneAsync(tripPosition);
            return ApiSuccess();
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpGet("{tripId}/positions")]
    [ProducesResponseType<GenericResponse<ResultCollection<TripPositionDto>>>(200)]
    public async Task<IActionResult> GetPositions(
        string tripId,
        DateTimeOffset? periodStart,
        DateTimeOffset? periodEnd,
        int skip = 0,
        int limit = 20
    )
    {
        try
        {
            var list = await _positionService.GetAsync(tripId, skip, limit);
            if (list is null)
                return ApiError(ApiResultCode.NotFound, "Trip does not exist");

            return ApiSuccessCollection(list.Select(x => (TripPositionDto)x!));
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpPost("knownLocations")]
    [ProducesResponseType<GenericResponse<RegisterNewKnownLocationResponse>>(200)]
    public async Task<IActionResult> RegisterKnownLocations(RegisterNewKnownLocationRequest request)
    {
        try
        {
            KnownLocation location = new()
            {
                Name = request.Name,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Ratio = request.Ratio
            };

            await _context.KnownLocations.AddAsync(location);
            await _context.SaveChangesAsync();

            RegisterNewKnownLocationResponse response = new()
            {
                LocationId = location.Id
            };

            return ApiSuccess(response);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpGet("knownLocations")]
    [ProducesResponseType<GenericResponse<ResultCollection<KnownLocation>>>(200)]
    public async Task<IActionResult> GetKnownLocations()
    {
        try
        {
            var list = await _context.KnownLocations.ToListAsync();
            return ApiSuccessCollection(list);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpPut("knownLocations/{id}")]
    public async Task<IActionResult> UpdateKnownLocations(int id, UpdateKnownLocationRequest request)
    {
        try
        {
            var location = await _context.KnownLocations.SingleOrDefaultAsync(
                x => x.Id == id
            );

            if (location is null)
                return ApiError(ApiResultCode.NotFound);

            if (request.Name is not null)
            {
                location.Name = request.Name;
            }
            if (request.Address is not null)
            {
                location.Address = request.Address;
            }
            if (request.Latitude is not null)
            {
                location.Latitude = request.Latitude.Value;
            }
            if (request.Longitude is not null)
            {
                location.Longitude = request.Longitude.Value;
            }
            if (request.Ratio is not null)
            {
                location.Ratio = request.Ratio.Value;
            }

            _context.KnownLocations.Update(location);
            await _context.SaveChangesAsync();
            return ApiSuccess();
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpDelete("knownLocations/{id}")]
    public async Task<IActionResult> DeleteKnownLocations(int id)
    {
        try
        {
            var location = await _context.KnownLocations.SingleOrDefaultAsync(
                x => x.Id == id
            );

            if (location is null)
                return ApiError(ApiResultCode.NotFound);

            _context.Remove(location);
            await _context.SaveChangesAsync();
            return ApiSuccess();
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }

    [HttpGet("pending/{driverId}")]
    public async Task<IActionResult> GetPendingTrip(int driverId)
    {
        try
        {
            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();
            // obtiene la información del conductor    
            var driver = users.FirstOrDefault(x => x.Id == driverId);
            if (driver is null)
                return ApiError(ApiResultCode.NotFound, "Driver was not found");
            if (driver.Role != UserAccountRole.Driver)
                return ApiError(ApiResultCode.InvalidState, "User is not a driver");
            if (driver.Status != UserAccountStatus.Enabled)
                return ApiError(ApiResultCode.InvalidState, "User account is disabled");
            
            // obtiene el viaje pendiente
            var trip = await _tripService.GetPendingAsync(driverId);
            if (trip is null)
                return ApiSuccess();

            var result = new TripDto
            {
                TripId = trip.Id,
                Number = trip.Number,
                Status = trip.Status,
                Origin = trip.Origin!,
                Destination = trip.Destination!,
                CreationDate = trip.CreationDate,
                LastUpdateDate = trip.LastUpdateDate,
                Stats = trip.Stats,
                Driver = users.SingleOrDefault(u => u.Id == trip.DriverId),
                CreatedBy = users.SingleOrDefault(u => u.Id == trip.CreatedById),
                UpdatedBy = users.SingleOrDefault(u => u.Id == trip.UpdatedById),
                Vehicle = await _context.Vehicles
                    .SingleOrDefaultAsync(v => v.Id == trip.VehicleId),
                PlannedStops = trip.PlannedStops?
                    .Select(p => (TripPlannedStopDto)p!)
                    .ToList() ?? []
            };

            return ApiSuccess(result);
        }
        catch (FormatException ex)
        {
            return ApiError(ApiResultCode.InvalidArgs, ex.Message);
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }
}
