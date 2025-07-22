using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateUserAccountRequest
{
    [Length(1, Constants.MaxUserNameLength)]
    public string? FirstName { get; set; }

    [Length(1, Constants.MaxUserNameLength)]
    public string? LastName { get; set; }

    [EmailAddress]
    [MaxLength(Constants.MaxUserEmailLength)]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(Constants.MaxUserPhoneNumberLength)]
    public string? PhoneNumber { get; set; }

    public UserAccountStatus? Status { get; set; }
}
