using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Controllers;

[Route("/api/testing")]
[ApiController]
public class TestingController
(
    VehicleService vehicleService,
    TripService tripService,
    AxoMotorContext context) : ControllerBase
{
    private readonly VehicleService _vehicleService = vehicleService;
    private readonly TripService _tripService = tripService;
    private readonly AxoMotorContext _context = context;

    [HttpPost("device/{vehicleId}/event")]
    public async Task<IActionResult> PostDeviceEvent(string vehicleId, PostDeviceEventRequest request)
    {
        try
        {
            // obtiene el vehículo
            var vehicle = await _vehicleService.GetAsync(vehicleId);
            // verifica si el código es de un evento de dispositivo válido
            var eventInfo = await _context.DeviceEventCatalog.SingleOrDefaultAsync(
                x => x.Code == request.Code
            );

            // verifica si el vehículo no existe
            if (vehicle is null || vehicle.IsOutOfService || eventInfo is null)
            {
                return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
            }

            DeviceEvent deviceEvent = new()
            {
                Code = request.Code,
                Severity = eventInfo.Severity,
                Type = eventInfo.Type,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(request.Timestamp)
            };

            var result = await _vehicleService.PushEventAsync(vehicleId, deviceEvent);
            if (!result)
            {
                return Ok(Responses.MinimalResponse(ApiResultCode.Failed));
            }

            return Ok(Responses.MinimalResponse(ApiResultCode.Success));
        }
        catch (FormatException)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.SystemException));
        }
    }


    [HttpPost("trip/{tripId}/positions")]
    public async Task<IActionResult> PostPosition(string tripId, TripPositionDto positionDto)
    {
        try
        {
            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(tripId);
            if (trip is null || trip.IsFinished)
            {
                return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
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
                return Ok(Responses.MinimalResponse(ApiResultCode.Failed));
            }

            return Ok(Responses.MinimalResponse(ApiResultCode.Success));
        }
        catch (FormatException)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.SystemException));
        }
    }
}
