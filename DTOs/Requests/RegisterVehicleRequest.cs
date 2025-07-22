using System.ComponentModel.DataAnnotations;
using AxoMotor.ApiServer.Data;

namespace AxoMotor.ApiServer.DTOs.Requests;

public class RegisterVehicleRequest
{
    [Required]
    [Length(Constants.MinVehiclePlateNumberLength, Constants.MaxVehiclePlateNumberLength)]
    public required string PlateNumber { get; set; }

    [MaxLength(Constants.MaxVehicleRegNumberLength)]
    [Required(AllowEmptyStrings = false)]
    public required string RegistrationNumber { get; set; }

    [MaxLength(Constants.MaxVehicleBrandLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Brand { get; set; }

    [MaxLength(Constants.MaxVehicleModelLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Model { get; set; }

    [MaxLength(Constants.MaxVehicleClassLength)]
    [Required(AllowEmptyStrings = false)]
    public required string Class { get; set; }

    public required int Year { get; set; }
}
