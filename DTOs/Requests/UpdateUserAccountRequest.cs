using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class UpdateUserAccountRequest
{
    [Length(3, 40)]
    public string? FirstName { get; set; }

    [Length(3, 40)]
    public string? LastName { get; set; }

    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public UserAccountStatus? Status { get; set; }
}
