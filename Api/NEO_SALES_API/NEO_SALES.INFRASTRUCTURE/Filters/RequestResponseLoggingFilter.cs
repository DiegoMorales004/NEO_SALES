using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace NEO_SALES.INFRASTRUCTURE.Filters;

public class RequestResponseLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestResponseLoggingFilter> _logger;

    public RequestResponseLoggingFilter(ILogger<RequestResponseLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var requestId = context.HttpContext.TraceIdentifier.Replace(':', '-');

        _logger.LogInformation(
            "RequestId: {RequestId} | DateTime: {DateTime} | Request: {Path} {Arguments}",
            requestId,
            DateTime.Now,
            context.HttpContext.Request.Path,
            context.ActionArguments);

        var executedContext = await next();

        _logger.LogInformation(
            "RequestId: {RequestId} | DateTime: {DateTime} | Response: {Result}",
            requestId,
            DateTime.Now,
            executedContext.Result);
    }
}
