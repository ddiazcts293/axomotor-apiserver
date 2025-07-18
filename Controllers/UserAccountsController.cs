using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AxoMotor.ApiServer.Controllers;

[ApiController]
[Route("api/userAccounts")]
public class UserAccountsController(UserAccountService service) : ControllerBase
{
    /*
        TODO: agregar autenticación
        
        - Conectar con API de Supabase para administrar autenticación
        - Implementar endpoint para restablecer la contraseña
    */

    private readonly UserAccountService _userAccountService = service;

    [HttpPost]
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
        UserAccountType? type,
        UserAccountStatus? status,
        bool? isLoggedIn
    )
    {
        try
        {
            var userAccounts = await _userAccountService.GetAsync(
                type, status, isLoggedIn);

            return Ok(userAccounts.ToSuccessCollectionResponse());
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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {        
        try
        {
            var userAccount = await _userAccountService.GetAsync(id);
            if (userAccount is null)
            {
                return Ok(Responses.ErrorResponse(ApiResultCode.NotFound));
            }

            return Ok(userAccount.ToSuccessResponse());
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateUserAccountRequest request)
    {
        try
        {
            bool result = await _userAccountService.UpdateAsync(
                id, request.FirstName, request.LastName, request.Status);

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
}
