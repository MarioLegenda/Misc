using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.TraceIdentifier;

        _logger.LogError(exception,
            "Unhandled exception. CorrelationId: {CorrelationId}, Path: {Path}",
            correlationId,
            httpContext.Request.Path);

        var (statusCode, title) = exception switch
        {
            UnauthorizedAccessException => (401, "Unauthorised."),
            KeyNotFoundException        => (404, "Resource not found."),
            ArgumentException           => (400, "Invalid request."),
            _                           => (500, "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Title  = title,
            Status = statusCode,
            Extensions = { ["traceId"] = correlationId }
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        // Return true = exception is handled, pipeline stops here
        // Return false = pass to the next handler in the chain
        return true;
    }
}