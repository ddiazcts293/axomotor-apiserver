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
    public async Task<IActionResult> Create(CreateIncidentRequest request)
    {
        try
        {
            // obtiene la información del tipo de incidente
            var info = await _context.IncidentCatalog
                .SingleAsync(x => x.Code == request.Code);
                
            // Obtiene el estado del viaje
            var tripStatus = await _tripService.GetStatus(request.TripId);
            if (tripStatus is null)
                return ApiError(ApiResultCode.NotFound, "The trip does not exist");

            // verifica si el viaje fue finalizado
            if (tripStatus.Value.IsFinished())
                return ApiError(ApiResultCode.InvalidState, "Trip was finished");

            Incident incident = new()
            {
                // TODO: obtener identificador de usuario que realiza la acción
                RegisteredById = 1,
                Code = request.Code,
                TripId = request.TripId,
                Priority = info.Priority,
                Type = info.Type,
                Description = request.Description,
                RelatedIncidentId = request.RelatedIncidentId,
                Pictures = request.Pictures?.Select(x => (IncidentPicture)x!).ToList()
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
        IncidentCode? code,
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
            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();
            // obtiene la lista de incidentes en mongodb
            var incidents = await _incidentService.GetAsync(
                code,
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
            // obtiene el incidente asociado
            var incident = await _incidentService.GetAsync(incidentId);
            if (incident is null)
                return ApiError(ApiResultCode.NotFound);

            // obtiene la lista de usuarios en mysql
            var users = await _context.UserAccounts.ToListAsync();

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
    public async Task<IActionResult> Update(string incidentId, UpdateIncidentRequest request)
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
                request.RelatedIncident,
                request.PicturesToAdd,
                request.PicturesToDelete
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
}
