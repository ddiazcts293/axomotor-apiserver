using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AxoMotor.ApiServer.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected OkObjectResult ApiSuccess()
    {
        return Ok(new BasicResponse()
        {
            Code = ApiResultCode.Success
        });
    }

    protected OkObjectResult ApiSuccess<T>(T result)
    {
        return Ok(new GenericResponse<T>()
        {
            Code = ApiResultCode.Success,
            Result = result
        });
    }

    protected OkObjectResult ApiSuccessCollection<T>(IEnumerable<T> items)
    {
        return Ok(new GenericResponse<ResultCollection<T>>()
        {
            Code = ApiResultCode.Success,
            Result = new()
            {
                Items = items,
                Length = items.Count()
            }
        });
    }

    protected ActionResult ApiError(ApiResultCode statusCode, string? reason = null)
    {
        return BadRequest(new ErrorResponse()
        {
            Code = statusCode,
            Reason = reason
        });
    }

    protected ActionResult ApiServerError<T>(
        T exception,
        string? format = null
    ) where T : Exception
    {
        string reason = (format is not null && format.Contains("{0}")) ?
            string.Format(format) :
            exception.Message;

        return StatusCode(500, new ErrorResponse()
        {
            Code = ApiResultCode.ServerException,
            Reason = reason
        });
    }
}
