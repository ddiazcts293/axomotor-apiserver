using System.Text.Json.Serialization;
using AxoMotor.ApiServer.Models.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace AxoMotor.ApiServer.Models.Catalog;

/// <summary>
/// Representa una cuenta de usuario en el sistema.
/// </summary>
public class UserAccount
{
    /// <summary>
    /// Identificador del usuario.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre de pila.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Apellidos.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Correo electrónico.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Número de teléfono.
    /// </summary>
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// Tipo de cuenta de usuario.
    /// </summary>
    public UserAccountRole Role { get; set; }

    /// <summary>
    /// Estado de la cuenta.
    /// </summary>
    public UserAccountStatus Status { get; set; }

    /// <summary>
    /// Indica si se ha iniciado sesión en la cuenta.
    /// </summary>
    public bool IsLoggedIn { get; set; }

    /// <summary>
    /// Fecha de registro.
    /// </summary>
    public DateTimeOffset RegistrationDate { get; set; }

    /// <summary>
    /// Fecha de última actualización.
    /// </summary>
    public DateTimeOffset? LastUpdateDate { get; set; }

    /// <summary>
    /// Fecha de último inicio de sesión.
    /// </summary>
    public DateTimeOffset? LastLogInDate { get; set; }

    #region JSON ignore

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}".Trim();

    #endregion
}
