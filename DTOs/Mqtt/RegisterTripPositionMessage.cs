using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Mqtt;

public class RegisterTripPositionMessage : IMqttIncomingMessage<TripPosition>
{
    [Required(AllowEmptyStrings = false)]
    [ValidateObjectId]
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string TripId { get; set; } = null!;

    /// <summary>
    /// Fuente de posición.
    /// </summary>
    public required TripPositionSource Source { get; set; }

    /// <summary>
    /// Latitud.
    /// </summary>
    [Range(-90, 90)]
    public required double Latitude { get; set; }

    /// <summary>
    /// Longitud.
    /// </summary>
    [Range(-180, 180)]
    public required double Longitude { get; set; }

    /// <summary>
    /// Velocidad registrada en el instante.
    /// </summary>
    [Range(0, Constants.MaxTripSpeedValue)]
    public required float Speed { get; set; }

    /// <summary>
    /// Marca de tiempo del instante.
    /// </summary>
    [Range(0, long.MaxValue)]
    public long? Timestamp { get; set; }

    public TripPosition ToValidatedModel()
    {
        MqttMessageValidationHelper.Validate(this);

        return new()
        {
            TripId = TripId,
            Source = Source,
            Coordinates = new(Latitude, Longitude),
            Speed = Speed,
            Timestamp = Timestamp.HasValue ?
                DateTimeOffset.FromUnixTimeSeconds(Timestamp.Value) :
                DateTimeOffset.UtcNow
        };
    }
}
