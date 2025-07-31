namespace AxoMotor.ApiServer.Exceptions;

[Serializable]
public class AxomotorMqttReceiverException : Exception
{
    public AxomotorMqttReceiverException() { }

    public AxomotorMqttReceiverException(string message) : base(message) { }

    public AxomotorMqttReceiverException(string message, Exception inner) : base(message, inner) { }

    public static void ThrowIfNull<T>(T? obj)
    {
        if (obj is null) throw new AxomotorMqttReceiverException("Invalid message format");
    }
    
    public static void ThrowIfNull<T>(T? obj, string message)
    {
        if (obj is null) throw new AxomotorMqttReceiverException(message);
    }
}
