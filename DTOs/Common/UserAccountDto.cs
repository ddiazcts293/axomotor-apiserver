using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class UserAccountDto
{
    public string Id { get; set; } = null!;

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public UserAccountType Type { get; set; }

    public UserAccountStatus Status { get; set; }

    public bool IsLoggedIn { get; set; }

    public DateTimeOffset RegistrationDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? LastUpdateDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? LastLogInDate { get; set; }

    public static UserAccountDto Convert(UserAccount account)
    {
        return new()
        {
            Id = account.Id,
            FirstName = account.FirstName,
            LastName = account.LastName,
            Type = account.Type,
            Status = account.Status,
            IsLoggedIn = account.IsLoggedIn,
            RegistrationDate = account.RegistrationDate,
            LastUpdateDate = account.LastUpdateDate,
            LastLogInDate = account.LastLogInDate,
        };
    }
}
