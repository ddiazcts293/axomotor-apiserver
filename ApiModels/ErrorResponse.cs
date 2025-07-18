using System.Text.Json.Serialization;

namespace AxoMotor.ApiServer.ApiModels;

public class ErrorResponse : BasicResponse
{
    /// <summary>
    /// Razón del error.
    /// </summary>
    [JsonPropertyOrder(1)]
    public string? Reason { get; init; }
}
