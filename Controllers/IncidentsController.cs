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
[Route("api/incidents")]
public class IncidentsController
(
    IncidentService incidentService,
    TripService tripService,
    AxoMotorContext context
) : ControllerBase
{
    private readonly IncidentService _incidentService = incidentService;
    private readonly TripService _tripService = tripService;
    private readonly AxoMotorContext _context = context;

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateIncidentRequest request)
    {
        try
        {
            // obtiene el código de incidente es válido
            var info = await _context.IncidentCatalog.SingleOrDefaultAsync(
                x => x.Code == request.Code);
            if (info is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.InvalidArgs, "Invalid incident code"
                ));
            }

            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(request.TripId);
            if (trip is null)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed, "The trip does not exist"
                ));
            }

            // verifica si el viaje fue finalizado
            if (trip.IsFinished)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Trip was finished"
                ));
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
                    return Ok(Responses.ErrorResponse(
                        ApiResultCode.InvalidArgs, "Invalid incident code"
                    ));
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

    [HttpGet("{incidentId}")]
    public async Task<IActionResult> Get(string incidentId)
    {
        try
        {
            var incident = await _incidentService.GetAsync(incidentId);
            if (incident is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            return Ok(incident.ToSuccessResponse());
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

    [HttpPut("{incidentId}")]
    public async Task<IActionResult> Update(string incidentId, [FromForm] UpdateIncidentRequest request)
    {
        try
        {
            var incident = await _incidentService.GetAsync(incidentId);
            // verifica si el incidente no existe
            if (incident is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            // verifica si el incidente está cerrado o descartado
            if (incident.IsClosedOrDiscarded)
            {
                return Ok(Responses.ErrorResponse(
                    ApiResultCode.Failed,
                    "Incident was closed"
                ));
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
                return Ok(Responses.ErrorResponse(ApiResultCode.Failed));
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

    [HttpDelete("{incidentId}")]
    public async Task<IActionResult> Delete(string incidentId)
    {
        try
        {
            if (!await _incidentService.DeleteAsync(incidentId))
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

    [HttpGet("{incidentId}/pictures/{pictureNumber}")]
    public async Task<IActionResult> GetPicture(string incidentId, int pictureNumber)
    {
        // TODO: obtención de imagen
        return Ok();
    }
}
