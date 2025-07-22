using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace AxoMotor.ApiServer.Helpers;

internal sealed class ValidateObjectIdAttribute : ValidationAttribute
{
    public ValidateObjectIdAttribute()
    {
        ErrorMessage = "Invalid object Id";
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext
    )
    {
        if (value is null || (value is string id && ObjectId.TryParse(id, out _)))
            return ValidationResult.Success;

        return new ValidationResult(ErrorMessage);
    }
}
