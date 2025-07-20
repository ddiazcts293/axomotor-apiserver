namespace AxoMotor.ApiServer.ApiModels.Enums;

/// <summary>
/// Código de resultado de API.
/// </summary>
public enum ApiResultCode
{
    /// <summary>
    /// Operación llevada a cabo exitosamente.
    /// </summary>
    Success,
    
    Failed,
    /// <summary>
    /// No encontrado.
    /// </summary>
    NotFound,
    /// <summary>
    /// Agumentos no válidos.
    /// </summary>
    InvalidArgs,
    /// <summary>
    /// Estado no válido.
    /// </summary>
    InvalidState,
    /// <summary>
    /// Error de servidor.
    /// </summary>
    ServerException,
    /// <summary>
    /// No implementado.
    /// </summary>
    NotImplemented
}
