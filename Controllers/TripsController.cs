using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Catalog;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace AxoMotor.ApiServer.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController
(
    TripService tripService,
    UserAccountService userAccountService,
    VehicleService vehicleService,
    AxoMotorContext context) : ControllerBase
{
    private readonly TripService _tripService = tripService;
    private readonly VehicleService _vehicleService = vehicleService;
    private readonly UserAccountService _userAccountService = userAccountService;
    private readonly AxoMotorContext _context = context;

    [HttpPost]
    public async Task<IActionResult> Create(CreateTripRequest request)
    {
        try
        {
            // obtiene la cuenta asociada al identificador de conductor especificado
            var account = await _userAccountService.GetAsync(request.DriverId);
            if (account is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.NotFound,
                    "Driver not found")
                );
            }

            if (account.Type != UserAccountType.Driver)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "User is not a driver")
                );
            }

            if (account.Status != UserAccountStatus.Enabled)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "User account disabled")
                );
            }

            // obtiene la cuenta asociada al identificador de vehículo especificado
            var vehicle = await _vehicleService.GetAsync(request.VehicleId);
            if (vehicle is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.NotFound,
                    "Vehicle not found")
                );
            }

            if (vehicle.Status is VehicleStatus.HeldByAuthorities)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Vehicle is held by authorities")
                );
            }

            if (vehicle.Status is VehicleStatus.OutOfService)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Vehicle is out of service")
                );
            }

            // TODO: verificar si el conductor o vehiculo están asignados a 
            // otro viaje en la misma fecha

            Trip trip = new()
            {
                Vehicle = request.VehicleId,
                Driver = request.DriverId,
                StartingPoint = new()
                {
                    Name = request.StartingPoint.Name,
                    Address = request.StartingPoint.Address,
                    DateTime = request.StartingPoint.DateTime,
                    Ratio = request.StartingPoint.Ratio,
                    Coordinates = new
                    (
                        request.StartingPoint.Longitude,
                        request.StartingPoint.Latitude
                    )
                },
                FinalDestination = new()
                {
                    Name = request.FinalDestination.Name,
                    Address = request.FinalDestination.Address,
                    DateTime = request.FinalDestination.DateTime,
                    Ratio = request.FinalDestination.Ratio,
                    Coordinates = new
                    (
                        request.FinalDestination.Longitude,
                        request.FinalDestination.Latitude
                    )
                },
                CreatedBy = request.DriverId
            };

            if (request.PlannedStops is not null)
            {
                List<TripLocation> plannedStops = [];
                foreach (var item in request.PlannedStops)
                {
                    plannedStops.Add(new()
                    {
                        Name = item.Name,
                        Address = item.Address,
                        DateTime = item.DateTime,
                        Ratio = item.Ratio,
                        Coordinates = new(item.Longitude, item.Latitude)
                    });
                }

                trip.PlannedStops = plannedStops;
            }

            await _tripService.CreateAsync(trip);
            var response = new CreateTripResponse()
            {
                TripId = trip.Id,
                Status = trip.Status
            };

            return Ok(response.ToSuccessResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        string? driverId,
        string? vehicleId,
        string? agentId,
        string? startingPointName,
        string? finalDestinationName,
        TripStatus? status)
    {
        try
        {
            var result = await _tripService.GetAsync(
                driverId,
                vehicleId,
                agentId,
                startingPointName,
                finalDestinationName,
                status
            );

            return Ok(result.ToSuccessCollectionResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpGet("{tripId}")]
    public async Task<IActionResult> Get(string tripId)
    {
        try
        {
            var result = await _tripService.GetAsync(tripId);
            if (result is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            return Ok(result.ToSuccessResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
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
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            // verifica si el viaje está finalizado
            if (trip.IsFinished)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Trip was finished"
                ));
            }

            if (request.DriverId is not null)
            {
                // obtiene la cuenta asociada al identificador de conductor especificado
                var account = await _userAccountService.GetAsync(request.DriverId);
                if (account is null)
                {
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.NotFound,
                        "Driver not found")
                    );
                }

                if (account.Type != UserAccountType.Driver)
                {
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.Failed,
                        "User is not a driver")
                    );
                }

                if (account.Status != UserAccountStatus.Enabled)
                {
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.Failed,
                        "User account disabled")
                    );
                }

                // TODO: verificar que el conductor no esté asignado a otro viaje
                // (excluyendo el actual)
            }

            if (request.VehicleId is not null)
            {
                // obtiene la cuenta asociada al identificador de vehículo especificado
                var vehicle = await _vehicleService.GetAsync(request.VehicleId);
                if (vehicle is null)
                {
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.NotFound,
                        "Vehicle not found")
                    );
                }

                if (vehicle.IsOutOfService)
                {
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.Failed,
                        "Vehicle is out of service")
                    );
                }

                // TODO: verificar si el vehiculo están asignado a otro viaje
            }

            List<TripLocation>? plannedStops = null;
            if (request.PlannedStops is not null)
            {
                plannedStops = [];

                foreach (var item in request.PlannedStops)
                {
                    plannedStops.Add(new()
                    {
                        Name = item.Name,
                        Address = item.Address,
                        DateTime = item.DateTime,
                        Ratio = item.Ratio,
                        Coordinates = new(item.Longitude, item.Latitude)
                    });
                }
            }

            TripLocation? startingPoint = null;
            if (request.StartingPoint is not null)
            {
                startingPoint = new()
                {
                    Coordinates = new(
                        request.StartingPoint.Longitude,
                        request.StartingPoint.Latitude
                    ),
                    Ratio = request.StartingPoint.Ratio,
                    Name = request.StartingPoint.Name,
                    Address = request.StartingPoint.Address,
                    DateTime = request.StartingPoint.DateTime
                };
            }

            TripLocation? finalDestination = null;
            if (request.FinalDestination is not null)
            {
                finalDestination = new()
                {
                    Coordinates = new(
                        request.FinalDestination.Longitude,
                        request.FinalDestination.Latitude
                    ),
                    Ratio = request.FinalDestination.Ratio,
                    Name = request.FinalDestination.Name,
                    Address = request.FinalDestination.Address,
                    DateTime = request.FinalDestination.DateTime
                };
            }

            bool result = await _tripService.UpdateAsync(
                tripId,
                request.DriverId,
                request.VehicleId,
                request.Status,
                startingPoint,
                finalDestination,
                plannedStops
            );

            if (!result)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            return Ok(Responses.SuccessResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpDelete("{tripId}")]
    public async Task<IActionResult> Delete(string tripId)
    {
        try
        {
            if (!await _tripService.DeleteAsync(tripId))
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            return Ok(Responses.SuccessResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

#if false
    [HttpPost("{tripId}/positions")]
    public async Task<IActionResult> PostPosition(string tripId, TripPositionDto positionDto)
    {
        try
        {
            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(tripId);
            if (trip is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Trip does not exist"
                ));
            }

            // verifica si el viaje está finalizado
            if (trip.IsFinished)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Trip was finished"
                ));
            }

            var tripPosition = new TripPosition()
            {
                Speed = positionDto.Speed,
                Timestamp = positionDto.Timestamp,
                Coordinates = new(positionDto.Longitude, positionDto.Latitude)
            };

            var result = await _tripService.PushPositionAsync(tripId, tripPosition);
            if (!result)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Failed to record position"
                ));
            }

            return Ok(Responses.SuccessResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }
#endif

    [HttpGet("{tripId}/positions")]
    public async Task<IActionResult> GetPositions(
        string tripId,
        int skip = 0,
        int limit = 20
    )
    {
        try
        {
            var result = await _tripService.GetPositionsAsync(tripId, skip, limit);
            if (result is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Trip does not exist"
                ));
            }

            return Ok(result.ToSuccessCollectionResponse());
        }
        catch (FormatException ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpGet("/knownLocations")]
    public async Task<IActionResult> GetKnownLocations()
    {
        try
        {
            var list = await _context.KnownLocations.ToListAsync();
            return Ok(list.ToSuccessCollectionResponse());
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpPost("/knownLocations")]
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

            return Ok(response.ToSuccessResponse());
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpPut("/knownLocations/{id}")]
    public async Task<IActionResult> UpdateKnownLocations(int id, UpdateKnownLocationRequest request)
    {
        try
        {
            var location = await _context.KnownLocations.SingleOrDefaultAsync(
                x => x.Id == id
            );

            if (location is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

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
            return Ok(Responses.SuccessResponse());
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }

    [HttpDelete("/knownLocations/{id}")]
    public async Task<IActionResult> DeleteKnownLocations(int id)
    {
        try
        {
            var location = await _context.KnownLocations.SingleOrDefaultAsync(
                x => x.Id == id
            );

            if (location is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            _context.Remove(location);
            await _context.SaveChangesAsync();
            return Ok(Responses.SuccessResponse());
        }
        catch (Exception ex)
        {
            return Ok(ex.ToErrorResponse(ApiResultCode.SystemException));
        }
    }
}
