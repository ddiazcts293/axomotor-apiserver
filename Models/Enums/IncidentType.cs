namespace AxoMotor.ApiServer.Models.Enums;

/// <summary>
/// Tipo de incidente.
/// </summary>
public enum IncidentType
{
    /// <summary>
    /// Falla mecánica.
    /// </summary>
    Mechanical,
    /// <summary>
    /// De ruta.
    /// </summary>
    Route,
    /// <summary>
    /// De carga.
    /// </summary>
    Cargo,
    /// <summary>
    /// De conductor.
    /// </summary>
    Driver,
    /// <summary>
    /// De seguridad.
    /// </summary>
    Security,
    /// <summary>
    /// Otro.
    /// </summary>
    Other
}
