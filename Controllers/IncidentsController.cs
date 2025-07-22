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
            // obtiene el código de incidencia es válido
            var info = await _context.IncidentCatalog.SingleOrDefaultAsync(
                x => x.Code == request.Code);
            if (info is null)
                return ApiError(ApiResultCode.InvalidArgs, "Invalid incident code");

            // verifica si el viaje existe
            var trip = await _tripService.GetAsync(request.TripId);
            if (trip is null)
                return ApiError(ApiResultCode.NotFound, "The trip does not exist");

            // verifica si el viaje fue finalizado
            if (trip.IsFinished)
                return ApiError(ApiResultCode.InvalidState, "Trip was finished");

            Incident incident = new()
            {
                // TODO: obtener identificador de usuario que realiza la acción
                RegisteredById = trip.DriverId,
                Code = request.Code,
                TripId = request.TripId,
                Priority = info.Priority,
                Type = info.Type,
                Description = request.Description,
                RelatedIncidentId = request.RelatedIncidentId,
                // TODO: implementar subidad de imagenes
            };

            await _incidentService.CreateAsync(incident);

            CreateIncidentResponse response = new()
            {
                IncidentId = incident.Id,
                Priority = incident.Priority,
                Status = incident.Status,
                Type = incident.Type
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
    [ProducesResponseType<GenericResponse<ResultCollection<IncidentDto>>>(200)]
    public async Task<IActionResult> Get(
        string? incidentCode,
        IncidentType? type,
        IncidentStatus? status,
        IncidentPriority? priority,
        int? registeredBy,
        int? revisedBy,
        int? closedBy
    )
    {
        try
        {
            if (incidentCode is not null)
            {
                bool isValidCode = await _context.IncidentCatalog.AnyAsync(
                    x => x.Code == incidentCode);

                if (!isValidCode) return ApiError(
                    ApiResultCode.InvalidArgs, "Invalid incident code"
                );
            }

            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();
            // obtiene la lista de incidentes en mongodb
            var incidents = await _incidentService.GetAsync(
                incidentCode,
                type,
                status,
                priority,
                registeredBy,
                revisedBy,
                closedBy
            );

            var results = incidents.Select(x => new IncidentDto
            {
                IncidentId = x.Id,
                Number = x.Number,
                TripId = x.TripId,
                Code = x.Code,
                Type = x.Type,
                Status = x.Status,
                Priority = x.Priority,
                LastKnownPosition = x.LastKnownPosition,
                Description = x.Description,
                Comments = x.Comments,
                RelatedIncidentId = x.RelatedIncidentId,
                RegistrationDate = x.RegistrationDate,
                RevisionDate = x.RevisionDate,
                RegisteredBy = users.SingleOrDefault(y => y.Id == x.RegisteredById),
                RevisedBy = users.SingleOrDefault(y => y.Id == x.RevisedById),
                ClosedBy = users.SingleOrDefault(y => y.Id == x.ClosedById)
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

    [HttpGet("{incidentId}")]
    [ProducesResponseType<GenericResponse<IncidentDto>>(200)]
    public async Task<IActionResult> Get(string incidentId)
    {
        try
        {
            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();
            // obtiene el incidente asociado
            var incident = await _incidentService.GetAsync(incidentId);
            if (incident is null)
            {
                return ApiError(ApiResultCode.NotFound);
            }

            var result = new IncidentDto
            {
                IncidentId = incident.Id,
                Number = incident.Number,
                TripId = incident.TripId,
                Code = incident.Code,
                Type = incident.Type,
                Status = incident.Status,
                Priority = incident.Priority,
                LastKnownPosition = incident.LastKnownPosition,
                Description = incident.Description,
                Comments = incident.Comments,
                RelatedIncidentId = incident.RelatedIncidentId,
                RegistrationDate = incident.RegistrationDate,
                RevisionDate = incident.RevisionDate,
                Pictures = incident.Pictures ?? [],
                RegisteredBy = users.SingleOrDefault(y => y.Id == incident.RegisteredById),
                RevisedBy = users.SingleOrDefault(y => y.Id == incident.RevisedById),
                ClosedBy = users.SingleOrDefault(y => y.Id == incident.ClosedById)
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

    [HttpPut("{incidentId}")]
    public async Task<IActionResult> Update(string incidentId, [FromForm] UpdateIncidentRequest request)
    {
        try
        {
            // verifica si la incidencia existe
            var incident = await _incidentService.GetAsync(incidentId);
            if (incident is null)
                return ApiError(ApiResultCode.NotFound);
            
            // verifica si la incidencia está cerrado o descartado
            if (incident.WasClosedOrDiscarded)
                return ApiError(
                    ApiResultCode.InvalidState,
                    "Incident was closed"
                );

            bool result = await _incidentService.UpdateAsync(
                incidentId,
                request.Status,
                request.Priority,
                request.Comments,
                request.RelatedIncident
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

    [HttpDelete("{incidentId}")]
    public async Task<IActionResult> Delete(string incidentId)
    {
        try
        {
            if (!await _incidentService.DeleteAsync(incidentId))
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

    [HttpGet("{incidentId}/pictures/{pictureNumber}")]
    public async Task<IActionResult> GetPicture(string incidentId, int pictureNumber)
    {
        // TODO: obtención de imagen
        return Ok();
    }
}
