using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterUserAccountRequest
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(Constants.MaxUserNameLength)]
    public required string FirstName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(Constants.MaxUserNameLength)]
    public required string LastName { get; set; }

    [EmailAddress]
    [Required(AllowEmptyStrings = false)]
    [MaxLength(Constants.MaxUserEmailLength)]
    public required string Email { get; set; }

    [Phone]
    [Required(AllowEmptyStrings = false)]
    [MaxLength(Constants.MaxUserPhoneNumberLength)]
    public required string PhoneNumber { get; set; }

    public required UserAccountRole Role { get; set; }
}
