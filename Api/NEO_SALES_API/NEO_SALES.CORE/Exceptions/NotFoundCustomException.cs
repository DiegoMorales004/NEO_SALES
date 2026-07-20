namespace NEO_SALES.CORE.Exceptions;

public class NotFoundCustomException : Exception
{
    public string Entity { get; }
    public string Key { get; }

    public NotFoundCustomException(string message, string entity, object key)
        : base($"{message} - Entidad: {entity}, Clave: {key}")
    {
        Entity = entity;
        Key = key.ToString() ?? string.Empty;
    }
}
