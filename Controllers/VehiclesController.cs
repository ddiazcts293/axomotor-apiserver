using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Controllers;

[ApiController]
[Route("api/vehicles")]
public class VehiclesController
(
    VehicleService service,
    AxoMotorContext context) : ControllerBase
{
    private readonly VehicleService _vehicleService = service;
    private readonly AxoMotorContext _context = context;

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVehicleRequest request)
    {
        try
        {
            bool isValidCode = await _context.VehicleClasses.AnyAsync(
                x => x.Code == request.Class
            );

            if (!isValidCode)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.InvalidArgs, "Invalid vehicle class code"
                ));
            }

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
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.InvalidArgs, "Invalid vehicle class code"
                    ));
                }
            }

            var vehicles = await _vehicleService.GetAsync(
            status, brand, model, vehicleClass, year, inUse);

            return Ok(vehicles.ToSuccessCollectionResponse());
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

    [HttpGet("{vehicleId}")]
    public async Task<IActionResult> Get(string vehicleId)
    {
        try
        {
            var vehicle = await _vehicleService.GetAsync(vehicleId);
            if (vehicle is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            return Ok(vehicle.ToSuccessResponse());
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

    [HttpPut("{vehicleId}")]
    public async Task<IActionResult> Update(string vehicleId, UpdateVehicleRequest request)
    {
        try
        {
            bool result = await _vehicleService.UpdateAsync(
                vehicleId, request.PlateNumber, request.Status);

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

    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> Delete(string vehicleId)
    {
        try
        {
            if (!await _vehicleService.DeleteAsync(vehicleId))
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

    [HttpPost("{vehicleId}/checkConnection")]
    public async Task<IActionResult> CheckConnection(string vehicleId)
    {
        // TODO: verificar conectividad con el dispositivo
        return Ok(Responses.ErrorResponse(ApiResultCode.NotImplemented));
    }

    [HttpGet("{vehicleid}/events")]
    public async Task<IActionResult> GetEvents(
        string vehicleid,
        int skip = 0,
        int limit = 20)
    {
        try
        {
            var result = await _vehicleService.GetEventsAsync(vehicleid, skip, limit);
            if (result is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Vehicle does not exist"
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
}
