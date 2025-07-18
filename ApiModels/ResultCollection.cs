namespace AxoMotor.ApiServer.ApiModels;

public class ResultCollection<T>
{
    /// <summary>
    /// Elementos devueltos.
    /// </summary>
    public required IEnumerable<T> Items { get; init; }

    /// <summary>
    /// Longitud de la colección de elementos devueltos.
    /// </summary>
    public required int Length { get; init; }
}
