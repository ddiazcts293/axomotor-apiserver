using System.ComponentModel.DataAnnotations;

namespace AxoMotor.ApiServer.Helpers;

public static class MqttMessageValidationHelper
{
    public static void Validate<T>(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        var context = new ValidationContext(obj);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(obj, context, results, true))
        {
            string message = $"Failed to validate content of {obj.GetType().Name}:\n" +
                string.Join("\n", results.Select(x => $"- {x.ErrorMessage}"));

            throw new ValidationException(message);
        }
    }
}
