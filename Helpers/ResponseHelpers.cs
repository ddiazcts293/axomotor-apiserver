using AxoMotor.ApiServer.ApiModels;
using AxoMotor.ApiServer.ApiModels.Enums;

namespace AxoMotor.ApiServer.Helpers;

public static class Responses
{
    public static MinimalResponse MinimalResponse(ApiResultCode code) =>
        new() { Code = code };

    public static BasicResponse SuccessResponse() =>
        new() { Code = ApiResultCode.Success };

    public static BasicResponse ErrorResponse(ApiResultCode code) =>
        new() { Code = code };

    public static ErrorResponse ErrorResponse(ApiResultCode code, string? reason = null) =>
        new() { Code = code, Reason = reason };
}
