using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterUserAccountRequest
{
    [Length(3, 40)]
    public required string FirstName { get; set; }

    [Length(3, 40)]
    public required string LastName { get; set; }

    [MaxLength(40)]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    [MaxLength(12)]
    [DataType(DataType.PhoneNumber)]
    public required string PhoneNumber { get; set; }

    public required UserAccountRole Role { get; set; }
}
