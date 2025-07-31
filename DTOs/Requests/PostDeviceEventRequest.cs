using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class PostDeviceEventRequest
{
    public required DeviceEventCode Code { get; set; }

    [Range(0, long.MaxValue)]
    public required long Timestamp { get; set; }
}
