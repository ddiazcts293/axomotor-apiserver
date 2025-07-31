namespace AxoMotor.ApiServer.DTOs.Mqtt;

public interface IMqttIncomingMessage<T>
{
    public string TripId { get; set; }

    public long? Timestamp { get; set; }

    public T ToValidatedModel();
}
