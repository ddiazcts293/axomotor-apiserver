using AxoMotor.ApiServer.ApiModels.Enums;

namespace AxoMotor.ApiServer.ApiModels;

public class MinimalResponse
{
    public ApiResultCode Code { get; init; }

    public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
