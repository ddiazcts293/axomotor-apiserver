using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AxoMotor.ApiServer.Controllers;

[Route("api/incidents")]
[ProducesResponseType<BasicResponse>(200)]
[ProducesResponseType<ErrorResponse>(400)]
[ProducesResponseType<ErrorResponse>(500)]
public class IncidentsController
(
    IncidentService incidentService,
    TripService tripService,
    AxoMotorContext context
) : ApiControllerBase
{
    private readonly IncidentService _incidentService = incidentService;
    private readonly TripService _tripService = tripService;
    private readonly AxoMotorContext _context = context;

    [HttpPost]
    [ProducesResponseType<GenericResponse<CreateIncidentResponse>>(200)]
    public async Task<IActionResult> Create([FromForm] CreateIncidentRequest request)
    {
        try
        {
            // obtiene el código de incidente es válido
            var info = await _context.IncidentCatalog.SingleOrDefaultAsync(
                x => x.Code == request.Code);
            if (info is null)
            {
                return ApiError(ApiResultCode.InvalidArgs, "Invalid incident code");
            }

            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(request.TripId);
            if (trip is null)
            {
                return ApiError(ApiResultCode.NotFound, "The trip does not exist");
            }

            // verifica si el viaje fue finalizado
            if (trip.IsFinished)
            {
                return ApiError(ApiResultCode.InvalidState, "Trip was finished");
            }

            Incident incident = new()
            {
                Code = request.Code,
                Trip = request.TripId,
                Priority = info.Priority,
                Type = info.Type,
                LastKnownPosition = new()
                {
                    Coordinates = new(
                        request.LastKnownPosition.Longitude,
                        request.LastKnownPosition.Latitude
                    ),
                    Speed = request.LastKnownPosition.Speed,
                    Timestamp = request.LastKnownPosition.Timestamp
                },
                Description = request.Description,
                RelatedIncident = request.RelatedIncidentId,
                // por ahora se omite la subida de imagenes
            };

            await _incidentService.CreateAsync(incident);

            CreateIncidentResponse response = new()
            {
                IncidentId = incident.Id,
                Priority = incident.Priority,
                Status = incident.Status,
                Type = incident.Type
            };

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

    [HttpGet]
    [ProducesResponseType<GenericResponse<ResultCollection<IncidentDto>>>(200)]
    public async Task<IActionResult> Get(
        string? incidentCode,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        string? revisedBy,
        string? closedBy,
        DateTimeOffset? periodStart,
        DateTimeOffset? periodEnd
    )
    {
        try
        {
            if (incidentCode is not null)
            {
                bool isValidCode = await _context.IncidentCatalog.AnyAsync(
                    x => x.Code == incidentCode);

                if (!isValidCode)
                {
                    return ApiError(
                        ApiResultCode.InvalidArgs, "Invalid incident code"
                    );
                }
            }

            var vehicles = await _incidentService.GetAsync(
                incidentCode,
                type,
                status,
                priority,
                revisedBy,
                closedBy,
                periodStart,
                periodEnd
            );

            return ApiSuccessCollection(vehicles.Select(IncidentDto.Convert));
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

    [HttpGet("{incidentId}")]
    [ProducesResponseType<GenericResponse<IncidentDto>>(200)]
    public async Task<IActionResult> Get(string incidentId)
    {
        try
        {
            var incident = await _incidentService.GetAsync(incidentId);
            if (incident is null)
            {
                return ApiError(ApiResultCode.NotFound);
            }

            return ApiSuccess(IncidentDto.Convert(incident));
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

    [HttpPut("{incidentId}")]
    public async Task<IActionResult> Update(string incidentId, [FromForm] UpdateIncidentRequest request)
    {
        try
        {
            var incident = await _incidentService.GetAsync(incidentId);
            // verifica si el incidente no existe
            if (incident is null)
            {
                return ApiError(ApiResultCode.NotFound);
            }

            // verifica si el incidente está cerrado o descartado
            if (incident.IsClosedOrDiscarded)
            {
                return ApiError(
                    ApiResultCode.InvalidState,
                    "Incident was closed"
                );
            }

            bool result = await _incidentService.UpdateAsync(
                incidentId,
                request.Status,
                request.Priority,
                request.Comments,
                request.RelatedIncident
            );

            if (!result)
            {
                return ApiError(ApiResultCode.Failed);
            }

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

    [HttpDelete("{incidentId}")]
    public async Task<IActionResult> Delete(string incidentId)
    {
        try
        {
            if (!await _incidentService.DeleteAsync(incidentId))
            {
                return ApiError(ApiResultCode.NotFound);
            }

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

    [HttpGet("{incidentId}/pictures/{pictureNumber}")]
    public async Task<IActionResult> GetPicture(string incidentId, int pictureNumber)
    {
        // TODO: obtención de imagen
        return Ok();
    }
}
