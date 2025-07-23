using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxoMotor.ApiServer.Controllers;

[Route("/api/testing")]
[ApiController]
[ProducesResponseType<MinimalResponse>(200)]
[ProducesResponseType<MinimalResponse>(400)]
[ProducesResponseType<MinimalResponse>(500)]
public class TestingController
(
    TripService tripService,
    PositionService positionService,
    DeviceEventService deviceEventService,
    AxoMotorContext context
) : ControllerBase
{
    private readonly TripService _tripService = tripService;
    private readonly PositionService _positionService = positionService;
    private readonly DeviceEventService _deviceEventService = deviceEventService;
    private readonly AxoMotorContext _context = context;

    [HttpPost("device/{vehicleId}/event")]
    public async Task<IActionResult> PostDeviceEvent(int vehicleId, PostDeviceEventRequest request)
    {
        try
        {
            // obtiene el vehículo
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            // verifica si el código es de un evento de dispositivo válido
            var eventInfo = await _context.DeviceEventCatalog.FindAsync(request.Code);

            // verifica si el vehículo no existe
            if (vehicle is null)
                return Ok(Responses.MinimalResponse(ApiResultCode.NotFound));
            else if (eventInfo is null)
                return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
            else if (vehicle.IsOutOfService)
                return Ok(Responses.MinimalResponse(ApiResultCode.InvalidState));

            DeviceEvent deviceEvent = new()
            {
                VehicleId = vehicleId,
                Code = request.Code,
                Severity = eventInfo.Severity,
                Type = eventInfo.Type,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(request.Timestamp)
            };

            await _deviceEventService.PushOneAsync(deviceEvent);
            return Ok(Responses.MinimalResponse(ApiResultCode.Success));
        }
        catch (FormatException)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.ServerException));
        }
    }

    [HttpPost("trip/{tripId}/positions")]
    public async Task<IActionResult> PostPosition(string tripId, PostTripPositionRequest request)
    {
        try
        {
            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(tripId.ToString());
            if (trip is null)
                return Ok(Responses.MinimalResponse(ApiResultCode.NotFound));
            else if (trip.IsFinished)
                return Ok(Responses.MinimalResponse(ApiResultCode.InvalidState));

            var tripPosition = new TripPosition()
            {
                TripId = tripId,
                Source = request.Source,
                Speed = request.Speed,
                Timestamp = DateTimeOffset.FromUnixTimeSeconds(request.Timestamp),
                Coordinates = new(request.Longitude, request.Latitude)
            };

            await _positionService.PushOneAsync(tripPosition);
            return Ok(Responses.MinimalResponse(ApiResultCode.Success));
        }
        catch (FormatException)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.InvalidArgs));
        }
        catch (Exception)
        {
            return Ok(Responses.MinimalResponse(ApiResultCode.ServerException));
        }
    }
}
