using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DotnetIntegrationTested.Services.Http.Middleware;

public static class ExceptionMiddlewareExtensions
{
  public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder) =>
    builder.UseMiddleware<ExceptionMiddleware>();
}

public sealed class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionMiddleware> _logger;

  public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private async Task HandleExceptionAsync(HttpContext context, Exception ex)
  {
    ErrorResponse response;
    if (ex is OperationCanceledException)
    {
      response = new ErrorResponse(499, "Client request cancelled");
      _logger.LogInformation(ex, "Request {TraceId} was canceled", context.TraceIdentifier);
    }
    else
    {
      response = new ErrorResponse(500, "An unexpected error occurred");
      _logger.LogCritical(ex, "Unhandled exception occurred", context.TraceIdentifier);
    }

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = response.StatusCode;

    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
  }

  private record ErrorResponse([property: JsonIgnore] int StatusCode, string Message);
}
