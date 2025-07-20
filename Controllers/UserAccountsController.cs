using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxoMotor.ApiServer.Controllers;

[Route("api/userAccounts")]
[ProducesResponseType<BasicResponse>(200)]
[ProducesResponseType<ErrorResponse>(400)]
[ProducesResponseType<ErrorResponse>(500)]
public class UserAccountsController(UserAccountService service) : ApiControllerBase
{
    /*
        TODO: agregar autenticación
        
        - Conectar con API de Supabase para administrar autenticación
        - Implementar endpoint para restablecer la contraseña
    */

    private readonly UserAccountService _userAccountService = service;

    [HttpGet("me")]
    [ProducesResponseType<GenericResponse<UserAccountDto>>(200)]
    public async Task<IActionResult> GetMe()
    {
        // TODO: agregar endpoint para obtener información del mismo usuario
        return Ok(Responses.ErrorResponse(ApiResultCode.NotImplemented));
    }

    [HttpPost]
    [ProducesResponseType<RegisterUserAccountResponse>(200)]
    public async Task<IActionResult> Register(RegisterUserAccountRequest request)
    {
        try
        {
            UserAccount account = new()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Type = request.Type,
            };

            await _userAccountService.RegisterAsync(account);

            var response = new RegisterUserAccountResponse()
            {
                UserAccountId = account.Id,
                Status = account.Status
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
    [ProducesResponseType<GenericResponse<ResultCollection<UserAccountDto>>>(200)]
    public async Task<IActionResult> Get(
        UserAccountType? type,
        UserAccountStatus? status,
        bool? isLoggedIn
    )
    {
        try
        {
            var userAccounts = await _userAccountService.GetAsync(
                type, status, isLoggedIn);

            return ApiSuccess(userAccounts.Select(UserAccountDto.Convert));
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

    [HttpGet("{id}")]
    [ProducesResponseType<GenericResponse<UserAccountDto>>(200)]
    public async Task<IActionResult> Get(string id)
    {
        try
        {
            var userAccount = await _userAccountService.GetAsync(id);
            if (userAccount is null)
                return ApiError(ApiResultCode.NotFound);

            return ApiSuccess(UserAccountDto.Convert(userAccount));
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateUserAccountRequest request)
    {
        try
        {
            bool result = await _userAccountService.UpdateAsync(
                id, request.FirstName, request.LastName, request.Status);

            if (!result)
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

    [HttpPut("{id}/resetPassword")]
    public async Task<IActionResult> ResetPassword(string id)
    {
        // TODO: agregar restablecimiento de contraseña
        return Ok(Responses.ErrorResponse(ApiResultCode.NotImplemented));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            if (!await _userAccountService.DeleteAsync(id))
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
