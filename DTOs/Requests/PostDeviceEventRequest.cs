using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class PostDeviceEventRequest
{
    [MaxLength(Constants.MaxDeviceEventCodeLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Code { get; set; }

    [Range(0, long.MaxValue)]
    public required long Timestamp { get; set; }
}
