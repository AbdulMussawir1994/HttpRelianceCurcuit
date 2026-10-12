using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace OrderServiceAPI.Extensions;

public class DownstreamExceptionHandler(ILogger<DownstreamExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken token)
    {
        int statusCode;
        string title;

        switch (exception)
        {
            case BrokenCircuitException:
                statusCode = StatusCodes.Status503ServiceUnavailable;
                title = "Product service circuit is open";
                break;

            case TimeoutRejectedException:
                statusCode = StatusCodes.Status504GatewayTimeout;
                title = "Product service timed out";
                break;

            case HttpRequestException:
                statusCode = StatusCodes.Status503ServiceUnavailable;
                title = "Product service unavailable";
                break;

            default:
                return false;
        }

        logger.LogWarning(exception, "Downstream service failure. StatusCode: {StatusCode}, TraceId: {TraceId}", statusCode, httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = "The downstream service cannot process the request.",
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        await httpContext.Response.WriteAsJsonAsync(problem, token);

        return true;
    }
}
