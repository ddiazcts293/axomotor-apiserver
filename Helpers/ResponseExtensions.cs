using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;

namespace AxoMotor.ApiServer.Helpers;

public static class ResponseExtensions
{
    [Obsolete]
    public static GenericResponse<T> ToSuccessResponse<T>(this T result)
    {
        return new()
        {
            Code = ApiResultCode.Success,
            Result = result
        };
    }

    [Obsolete]
    public static GenericResponse<ResultCollection<T>> ToSuccessCollectionResponse<T>(
        this IEnumerable<T> items
    )
    {
        return new()
        {
            Code = ApiResultCode.Success,
            Result = new()
            {
                Items = items,
                Length = items.Count()
            }
        };
    }

    [Obsolete]
    public static ErrorResponse ToErrorResponse<T>(
        this T exception,
        string? format = null
    ) where T : Exception
    {
        string reason = (format is not null && format.Contains("{0}")) ?
            string.Format(format) :
            exception.Message;

        return new ErrorResponse()
        {
            Code = ApiResultCode.Failed,
            Reason = reason
        };
    }

    [Obsolete]
    public static ErrorResponse ToErrorResponse<T>(
        this T exception,
        ApiResultCode statusCode,
        string? format = null
    ) where T : Exception
    {
        string reason = (format is not null && format.Contains("{0}")) ?
            string.Format(format) :
            exception.Message;

        return new ErrorResponse()
        {
            Code = statusCode,
            Reason = reason
        };
    }


}
