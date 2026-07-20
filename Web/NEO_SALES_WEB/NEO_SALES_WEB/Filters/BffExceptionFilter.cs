using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Models.Configuration;

namespace NEO_SALES_WEB.Filters;


public class BffExceptionFilter : IExceptionFilter
{
    private readonly MessageDefaultsConfiguration _messages;

    public BffExceptionFilter(IOptions<MessageDefaultsConfiguration> messages)
    {
        _messages = messages.Value;
    }

    public void OnException(ExceptionContext context)
    {
        var (statusCode, message) = context.Exception switch
        {
            ApiBadRequestException ex => (StatusCodes.Status400BadRequest, ex.Message),
            ApiUnauthorizedException ex => (StatusCodes.Status401Unauthorized, ex.Message),
            ApiNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            ApiCommunicationException ex => (StatusCodes.Status502BadGateway, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, _messages.InternalError)
        };

        context.Result = new ObjectResult(new { message }) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}
