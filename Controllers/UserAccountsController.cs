using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.DTOs.Common;
using AxoMotor.ApiServer.DTOs.Requests;
using AxoMotor.ApiServer.DTOs.Responses;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models.Catalog;
using AxoMotor.ApiServer.Models.Enums;
using AxoMotor.ApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue.Exceptions;

namespace AxoMotor.ApiServer.Controllers;

[Route("api/userAccounts")]
[ProducesResponseType<BasicResponse>(200)]
[ProducesResponseType<ErrorResponse>(400)]
[ProducesResponseType<ErrorResponse>(500)]
//public class UserAccountsController(AxoMotorContext context, Supabase.Client client) : ApiControllerBase
public class UserAccountsController(AxoMotorContext context) : ApiControllerBase
{
    /*
        TODO: agregar autenticación
        
        - Conectar con API de Supabase para administrar autenticación
        - Implementar endpoint para restablecer la contraseña
    */

    private readonly AxoMotorContext _context = context;
    //private readonly Supabase.Client _client = client;

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<GenericResponse<UserAccount>>(200)]
    public async Task<IActionResult> GetMe()
    {
        string email = User.FindFirst("email")?.Value ?? string.Empty;
        
        try
        {
            var userAccount = await _context.UserAccounts.
                FirstOrDefaultAsync(x => x.Email == email);

            if (userAccount is null)
                return ApiError(ApiResultCode.NotFound);

            return ApiSuccess(userAccount);
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
                Role = request.Role,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            await _context.UserAccounts.AddAsync(account);
            await _context.SaveChangesAsync();

            try
            {
                string email = request.Email;
                string phone = request.PhoneNumber;
                string phoneLastDigits = phone.Substring(phone.Length - 4);
                string password = $"{request.Role}_{phoneLastDigits}";
                //await _client.Auth.SignUp(request.Email, password);
            }
            catch (GotrueException ex)
            {
                // borra el usuario recientemente creado de la base de datos
                _context.UserAccounts.Remove(account);
                await _context.SaveChangesAsync();
                return ApiError(ApiResultCode.Failed, ex.Reason.ToString());
            }

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
    [ProducesResponseType<GenericResponse<ResultCollection<UserAccount>>>(200)]
    public async Task<IActionResult> Get(
        UserAccountRole? role,
        UserAccountStatus? status,
        bool? isLoggedIn
    )
    {
        try
        {
            var query = _context.UserAccounts.AsQueryable();

            if (role is not null)
                query = query.Where(x => x.Role == role);
            if (status is not null)
                query = query.Where(x => x.Status == status);
            if (isLoggedIn is not null)
                query = query.Where(x => x.IsLoggedIn == isLoggedIn);

            var userAccounts = await query.ToListAsync();
            return ApiSuccess(userAccounts);
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

    [HttpGet("{userId}")]
    [ProducesResponseType<GenericResponse<UserAccount>>(200)]
    public async Task<IActionResult> Get(int userId)
    {
        try
        {
            var userAccount = await _context.UserAccounts.FindAsync(userId);
            if (userAccount is null)
                return ApiError(ApiResultCode.NotFound);

            return ApiSuccess(userAccount);
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

    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(int userId, UpdateUserAccountRequest request)
    {
        try
        {
            var userAccount = await _context.UserAccounts.FindAsync(userId);
            if (userAccount is null)
                return ApiError(ApiResultCode.NotFound);

            if (request.FirstName is not null)
                userAccount.FirstName = request.FirstName;
            if (request.LastName is not null)
                userAccount.LastName = request.LastName;
            if (request.Email is not null)
                userAccount.Email = request.Email;
            if (request.PhoneNumber is not null)
                userAccount.PhoneNumber = request.PhoneNumber;
            if (request.Status is not null)
                userAccount.Status = request.Status.Value;

            _context.UserAccounts.Update(userAccount);
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

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        try
        {
            var userAccount = await _context.UserAccounts.FindAsync(userId);
            if (userAccount is null)
                return ApiError(ApiResultCode.NotFound);

            _context.UserAccounts.Remove(userAccount);
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

    [HttpPut("{userId}/resetPassword")]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        try
        {
            var userAccount = await _context.UserAccounts.FindAsync(userId);
            if (userAccount is null)
                return ApiError(ApiResultCode.NotFound);

            string email = userAccount.Email;
            return ApiSuccess();
            /* if (await _client.Auth.ResetPasswordForEmail(email))
            {
            }
            else
            {
                return ApiError(ApiResultCode.Failed, "Auth provider error");
            } */
        }
        catch (Exception ex)
        {
            return ApiServerError(ex);
        }
    }
}
