namespace AxoMotor.ApiServer.DTOs.Requests;

public class PostDeviceEventRequest
{
    public required string Code { get; set; }

    public required long Timestamp { get; set; }
}
