namespace NEO_SALES.CORE.Exceptions;

public class ApiBadRequestException : Exception
{
    public ApiBadRequestException(string message) : base(message)
    {
    }
}
