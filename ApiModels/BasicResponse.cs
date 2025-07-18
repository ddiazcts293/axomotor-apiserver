using System.Text.Json.Serialization;
using AxoMotor.ApiServer.ApiModels.Enums;

namespace AxoMotor.ApiServer.ApiModels;

public class BasicResponse
{
    /// <summary>
    /// Estatus de la operación.
    /// </summary>
    [JsonPropertyOrder(-1)]
    public required ApiResultCode Code { get; init; }

    /// <summary>
    /// Marca de tiempo de la operación.
    /// </summary>
    [JsonPropertyOrder(999)]
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
