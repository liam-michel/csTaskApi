using System.Security.Claims;

namespace TaskApi.Middleware;

public class RequestLoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<RequestLoggingMiddleware> _logger;

  public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var requestId = context.TraceIdentifier;
    var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";

    using (_logger.BeginScope(new Dictionary<string, object>
    {
      ["RequestId"] = requestId,
      ["UserId"] = userId
    }))
    {
      await _next(context);
    }
  }
}
