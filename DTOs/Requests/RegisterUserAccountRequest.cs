using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterUserAccountRequest
{
    [Length(3, 40)]
    public required string FirstName { get; set; }

    [Length(3, 40)]
    public required string LastName { get; set; }

    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    public required UserAccountType Type { get; set; }
}
