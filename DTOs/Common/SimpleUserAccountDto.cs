using AxoMotor.ApiServer.Models.Catalog;
using AxoMotor.ApiServer.Models.Enums;

namespace AxoMotor.ApiServer.DTOs.Common;

public class SimpleUserAccountDto
{
    public required int Id { get; set; }

    public required string FullName { get; set; }

    public required UserAccountRole Role { get; set; }

    public static implicit operator SimpleUserAccountDto?(UserAccount? user)
        => user is null ? null : new()
        {
            Id = user.Id,
            FullName = user.FullName,
            Role = user.Role
        };
}
