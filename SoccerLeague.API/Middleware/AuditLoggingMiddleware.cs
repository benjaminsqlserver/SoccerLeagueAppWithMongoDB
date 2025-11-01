using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;
using System.Text;

namespace SoccerLeague.API.Middleware
{
    /// <summary>
    /// Middleware to automatically log all HTTP requests to the audit trail.
    /// </summary>
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditLogRepository)
        {
            // Skip audit logging for certain paths (optional)
            if (ShouldSkipAuditLogging(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var request = context.Request;
            var userId = context.User?.FindFirst("sub")?.Value; // Adjust based on your auth claims
            var username = context.User?.Identity?.Name;

            // Capture request details
            var requestBody = await ReadRequestBodyAsync(request);

            // Store original response body stream
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                // Log the request after completion
                await LogRequestAsync(
                    auditLogRepository,
                    userId,
                    username,
                    request,
                    context.Response.StatusCode,
                    requestBody);
            }
            catch (Exception ex)
            {
                // Log failed requests
                await LogRequestAsync(
                    auditLogRepository,
                    userId,
                    username,
                    request,
                    500,
                    requestBody,
                    ex.Message);

                throw;
            }
            finally
            {
                // Copy response back to original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private bool ShouldSkipAuditLogging(PathString path)
        {
            // Skip logging for health checks, swagger, static files, etc.
            var pathsToSkip = new[] { "/health", "/swagger", "/api/auditlogs" };
            return pathsToSkip.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<string?> ReadRequestBodyAsync(HttpRequest request)
        {
            if (request.ContentLength > 0)
            {
                request.EnableBuffering();

                // Use StreamReader for reliable reading
                using var reader = new StreamReader(
                    request.Body,
                    encoding: Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: -1,
                    leaveOpen: true);

                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                return body;
            }
            return null;
        }

        private async Task LogRequestAsync(
            IAuditLogRepository repository,
            string? userId,
            string? username,
            HttpRequest request,
            int statusCode,
            string? requestBody,
            string? errorMessage = null)
        {
            try
            {
                var actionType = DetermineActionType(request.Method, request.Path);
                var success = statusCode >= 200 && statusCode < 300;

                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Username = username,
                    ActionType = actionType,
                    EntityType = "HttpRequest",
                    Description = $"{request.Method} {request.Path}",
                    IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = request.Headers["User-Agent"].ToString(),
                    ActionDate = DateTime.UtcNow,
                    Success = success,
                    ErrorMessage = errorMessage,
                    AdditionalData = $"{{\"StatusCode\": {statusCode}, \"RequestBody\": \"{requestBody}\"}}",
                    CreatedDate = DateTime.UtcNow
                };

                await repository.AddAsync(auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit entry");
            }
        }

        private AuditActionType DetermineActionType(string method, PathString path)
        {
            return method.ToUpper() switch
            {
                "POST" => AuditActionType.Create,
                "PUT" => AuditActionType.Update,
                "PATCH" => AuditActionType.Update,
                "DELETE" => AuditActionType.Delete,
                "GET" => AuditActionType.View,
                _ => AuditActionType.SystemEvent
            };
        }
    }

    /// <summary>
    /// Extension method to register the audit logging middleware.
    /// </summary>
    public static class AuditLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditLoggingMiddleware>();
        }
    }
}