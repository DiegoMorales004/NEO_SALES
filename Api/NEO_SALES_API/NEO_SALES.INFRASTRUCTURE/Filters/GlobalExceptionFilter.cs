using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Exceptions;
using NEO_SALES.CORE.Models.Configuration;

namespace NEO_SALES.INFRASTRUCTURE.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly MessageDefaultsConfiguration _messages;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IOptions<MessageDefaultsConfiguration> messages)
    {
        _logger = logger;
        _messages = messages.Value;
    }

    public void OnException(ExceptionContext context)
    {
        var requestId = context.HttpContext.TraceIdentifier.Replace(':', '-');

        _logger.LogError(
            context.Exception,
            "RequestId: {RequestId} | DateTime: {DateTime} | Error no controlado procesando {Path}",
            requestId,
            DateTime.Now,
            context.HttpContext.Request.Path);

        var (statusCode, message) = context.Exception switch
        {
            BadRequestCustomException ex => (StatusCodes.Status400BadRequest, ex.Message),
            NotFoundCustomException ex => (StatusCodes.Status404NotFound, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, _messages.InternalError)
        };

        context.Result = new ObjectResult(new { message }) { StatusCode = statusCode };
        context.ExceptionHandled = true;
    }
}
