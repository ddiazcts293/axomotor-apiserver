namespace AxoMotor.ApiServer.Exceptions;

[Serializable]
public class AxomotorApiException : Exception
{
    public AxomotorApiException() { }

    public AxomotorApiException(string message) : base(message) { }

    public AxomotorApiException(string message, Exception inner) : base(message, inner) { }
}
