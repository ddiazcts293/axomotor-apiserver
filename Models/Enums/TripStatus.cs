namespace AxoMotor.ApiServer.Models.Enums;

/// <summary>
/// Indica el estado de un viaje.
/// </summary>
public enum TripStatus
{
    /// <summary>
    /// Planeado.
    /// </summary>
    Planned,
    /// <summary>
    /// Retrasado.
    /// </summary>
    Delayed,
    /// <summary>
    /// En ruta.
    /// </summary>
    OnRoute,
    /// <summary>
    /// En la base.
    /// </summary>
    AtBase,
    /// <summary>
    /// Detenido.
    /// </summary>
    Stopped,
    /// <summary>
    /// Finalizado.
    /// </summary>
    Finished,
    /// <summary>
    /// Cancelado.
    /// </summary>
    Cancelled,
    /// <summary>
    /// Abortado.
    /// </summary>
    Aborted,
    /// <summary>
    /// Señal de GPS perdida.
    /// </summary>
    GpsSignalLost
}
