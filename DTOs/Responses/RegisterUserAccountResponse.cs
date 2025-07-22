using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Responses;

public class RegisterUserAccountResponse
{
    public int UserAccountId { get; set; }

    public UserAccountStatus Status { get; set; }
}
