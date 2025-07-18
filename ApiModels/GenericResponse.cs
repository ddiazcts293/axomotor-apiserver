using System.Text.Json.Serialization;

namespace AxoMotor.ApiServer.ApiModels;

public class GenericResponse<T> : BasicResponse
{
    /// <summary>
    /// Resultado de la operación.
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required T Result { get; init; }
}
