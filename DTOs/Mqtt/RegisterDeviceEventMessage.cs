using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Helpers;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Mqtt;

public class RegisterDeviceEventMessage : IMqttIncomingMessage<DeviceEvent>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    [ValidateObjectId]
    [Required(AllowEmptyStrings = false)]
    public string TripId { get; set; } = null!;

    public required int VehicleId { get; set; }

    public required DeviceEventCode Code { get; set; }

    [Range(0, long.MaxValue)]
    public long? Timestamp { get; set; }

    public DeviceEvent ToValidatedModel()
    {
        MqttMessageValidationHelper.Validate(this);

        return new()
        {
            Code = Code,
            VehicleId = VehicleId,
            Timestamp = Timestamp.HasValue ?
                DateTimeOffset.FromUnixTimeSeconds(Timestamp.Value) :
                DateTimeOffset.UtcNow
        };
    }
}
