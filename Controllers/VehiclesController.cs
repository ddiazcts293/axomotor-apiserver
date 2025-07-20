using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Controllers;

[Route("api/vehicles")]
[ProducesResponseType<BasicResponse>(200)]
[ProducesResponseType<ErrorResponse>(400)]
[ProducesResponseType<ErrorResponse>(500)]
public class VehiclesController
(
    VehicleService service,
    AxoMotorContext context) : ApiControllerBase
{
    private readonly VehicleService _vehicleService = service;
    private readonly AxoMotorContext _context = context;

    [HttpPost]
    [ProducesResponseType<GenericResponse<RegisterVehicleResponse>>(200)]
    public async Task<IActionResult> Register(RegisterVehicleRequest request)
    {
        try
        {
            bool isValidCode = await _context.VehicleClasses.AnyAsync(
                x => x.Code == request.Class
            );

            if (!isValidCode)
                return ApiError(ApiResultCode.InvalidArgs, "Invalid vehicle class code");

            Vehicle vehicle = new()
            {
                PlateNumber = request.PlateNumber,
                RegistrationNumber = request.RegistrationNumber,
                Details = new()
                {
                    Brand = request.Brand,
                    Model = request.Model,
                    Class = request.Class,
                    Year = request.Year
                }
            };

            await _vehicleService.RegisterAsync(vehicle);

            RegisterVehicleResponse response = new()
            {
                VehicleId = vehicle.Id,
                Status = vehicle.Status
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
    [ProducesResponseType<GenericResponse<ResultCollection<VehicleDto>>>(200)]
    public async Task<IActionResult> Get(
        VehicleStatus? status,
        string? brand,
        string? model,
        string? vehicleClass,
        int? year,
        bool? inUse
    )
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(vehicleClass))
            {
                bool isValidCode = await _context.VehicleClasses.AnyAsync(
                    x => x.Code == vehicleClass
                );

                if (!isValidCode)
                {
                    return ApiError(
                        ApiResultCode.InvalidArgs,
                        "Invalid vehicle class code");
                }
            }

            var vehicles = await _vehicleService.GetAsync(
            status, brand, model, vehicleClass, year, inUse);

            return ApiSuccess(vehicles.Select(VehicleDto.Convert));
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

    [HttpGet("{vehicleId}")]
    [ProducesResponseType<GenericResponse<VehicleDto>>(200)]
    public async Task<IActionResult> Get(string vehicleId)
    {
        try
        {
            var vehicle = await _vehicleService.GetAsync(vehicleId);
            if (vehicle is null)
                return ApiError(ApiResultCode.NotFound);
            
            return ApiSuccess(VehicleDto.Convert(vehicle));
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

    [HttpPut("{vehicleId}")]
    public async Task<IActionResult> Update(string vehicleId, UpdateVehicleRequest request)
    {
        try
        {
            bool result = await _vehicleService.UpdateAsync(
                vehicleId, request.PlateNumber, request.Status);

            if (!result)
                return ApiError(ApiResultCode.NotFound);
            
            return Ok(Responses.SuccessResponse());
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

    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> Delete(string vehicleId)
    {
        try
        {
            if (!await _vehicleService.DeleteAsync(vehicleId))
                return ApiError(ApiResultCode.NotFound);
            
            return Ok(Responses.SuccessResponse());
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

    [HttpPost("{vehicleId}/checkConnection")]
    public async Task<IActionResult> CheckConnection(string vehicleId)
    {
        // TODO: verificar conectividad con el dispositivo
        return Ok(Responses.ErrorResponse(ApiResultCode.NotImplemented));
    }

    [HttpGet("{vehicleid}/events")]
    [ProducesResponseType<GenericResponse<ResultCollection<DeviceEvent>>>(200)]
    public async Task<IActionResult> GetEvents(
        string vehicleid,
        DateTimeOffset? periodStart,
        DateTimeOffset? periodEnd,
        int skip = 0,
        int limit = 20
    )
    {
        try
        {
            var result = await _vehicleService.GetEventsAsync(vehicleid, skip, limit);
            if (result is null)
                return ApiError(ApiResultCode.Failed, "Vehicle does not exist");

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
