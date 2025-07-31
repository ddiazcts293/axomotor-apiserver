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

[Route("api/vehicles")]
[ProducesResponseType<BasicResponse>(200)]
[ProducesResponseType<ErrorResponse>(400)]
[ProducesResponseType<ErrorResponse>(500)]
public class VehiclesController(
    AxoMotorContext context,
    DeviceEventService deviceEventService
) : ApiControllerBase
{
    private readonly AxoMotorContext _context = context;
    private readonly DeviceEventService _deviceEventService = deviceEventService;

    [HttpPost]
    [ProducesResponseType<GenericResponse<RegisterVehicleResponse>>(200)]
    public async Task<IActionResult> Register(RegisterVehicleRequest request)
    {
        try
        {
            var @class = await _context.VehicleClasses.FindAsync(request.Class);
            if (@class is null)
                return ApiError(ApiResultCode.InvalidArgs, "Invalid vehicle class code");

            Vehicle vehicle = new()
            {
                PlateNumber = request.PlateNumber,
                RegistrationNumber = request.RegistrationNumber,
                Brand = request.Brand,
                Model = request.Model,
                Class = @class,
                Year = request.Year
            };

            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();

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
    [ProducesResponseType<GenericResponse<ResultCollection<Vehicle>>>(200)]
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

            var query = _context.Vehicles.AsQueryable();

            if (status is not null)
                query = query.Where(x => x.Status == status);
            if (brand is not null)
                query = query.Where(x => x.Brand == brand);
            if (model is not null)
                query = query.Where(x => x.Model == model);
            if (vehicleClass is not null)
                query = query.Where(x => x.Class == vehicleClass);
            if (year is not null)
                query = query.Where(x => x.Year == year);
            if (inUse is not null)
                query = query.Where(x => x.InUse == inUse);

            var vehicles = await query.ToListAsync();
            return ApiSuccess(vehicles);
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
    [ProducesResponseType<GenericResponse<Vehicle>>(200)]
    public async Task<IActionResult> Get(int vehicleId)
    {
        try
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle is null)
                return ApiError(ApiResultCode.NotFound);
            
            return ApiSuccess(vehicle);
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
    public async Task<IActionResult> Update(int vehicleId, UpdateVehicleRequest request)
    {
        try
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle is null)
                return ApiError(ApiResultCode.NotFound);

            if (request.PlateNumber is not null)
                vehicle.PlateNumber = request.PlateNumber;
            if (request.RegistrationNumber is not null)
                vehicle.RegistrationNumber = request.RegistrationNumber;
            if (request.Status is not null)
                vehicle.Status = request.Status.Value;

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
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

    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> Delete(int vehicleId)
    {
        try
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle is null)
                return ApiError(ApiResultCode.NotFound);

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
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

    [HttpPost("{vehicleId}/checkConnection")]
    public async Task<IActionResult> CheckConnection(int vehicleId)
    {
        // TODO: verificar conectividad con el dispositivo
        return ApiError(ApiResultCode.NotImplemented);
    }

    [HttpGet("{vehicleId}/events")]
    [ProducesResponseType<GenericResponse<ResultCollection<DeviceEventDto>>>(200)]
    public async Task<IActionResult> GetEvents(
        int vehicleId,
        DeviceEventCode? code,
        DeviceEventType? type,
        DeviceEventSeverity? severity,
        DateTimeOffset? periodStart,
        DateTimeOffset? periodEnd,
        int skip = 0,
        int limit = 20
    )
    {
        try
        {
            bool vehicleExists = await _context.Vehicles.AnyAsync(x => x.Id == vehicleId);
            if (!vehicleExists)
                return ApiError(ApiResultCode.Failed, "Vehicle does not exist");

            var list = await _deviceEventService.GetAsync(
                vehicleId,
                skip,
                limit,
                code,
                type,
                severity
            );

            return ApiSuccessCollection(list.Select(x => (DeviceEventDto)x!));
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
