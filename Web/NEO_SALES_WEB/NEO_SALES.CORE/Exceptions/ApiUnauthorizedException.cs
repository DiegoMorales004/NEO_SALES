namespace NEO_SALES.CORE.Exceptions;

public class ApiUnauthorizedException : Exception
{
    public ApiUnauthorizedException(string message) : base(message)
    {
    }
}
