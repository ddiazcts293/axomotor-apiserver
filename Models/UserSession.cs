namespace AxoMotor.ApiServer.Models;

public class UserSession
{
    public required string AuthToken { get; set; }

    public DateTimeOffset LogInDate { get; set; }

    public DateTimeOffset? LogOutDate { get; set; }

    public bool Active { get; set; }
}
