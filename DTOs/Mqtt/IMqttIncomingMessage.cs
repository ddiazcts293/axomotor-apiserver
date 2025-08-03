namespace AxoMotor.ApiServer.DTOs.Mqtt;

public interface IMqttIncomingMessage<T>
{
    public long? Timestamp { get; set; }

    public T ToValidatedModel();
}
