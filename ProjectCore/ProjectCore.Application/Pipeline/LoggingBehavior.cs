using MediatR;
using Microsoft.Extensions.Logging;
using ProjectCore.Application.Logging;

namespace ProjectCore.Application.Pipeline;

/// <summary>
/// MediatR pipeline behavior that logs every request/response with EventId and timing.
/// Sits between the controller and the handler — no changes needed in individual handlers.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(LogEvents.RequestStarted,
            "[START] Request={Request}", requestName);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();

            _logger.LogInformation(LogEvents.RequestCompleted,
                "[END] Request={Request} ElapsedMs={ElapsedMs}",
                requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(LogEvents.RequestFailed, ex,
                "[FAIL] Request={Request} ElapsedMs={ElapsedMs} Error={ErrorMessage}",
                requestName, sw.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}
