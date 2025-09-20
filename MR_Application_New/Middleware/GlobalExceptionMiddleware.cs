

using BLL;
using System.Net;
using System.Text.Json;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

   

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);

            if ((context.Response.StatusCode == (int)HttpStatusCode.NotFound ||
                 context.Response.StatusCode == (int)HttpStatusCode.Forbidden) &&
                 !context.Response.HasStarted)
            {
                var statusCode = context.Response.StatusCode;
                var requestPath = context.Request.Path;
                var errorMessage = statusCode == 404 ? "Page not found" : "Access denied";

                _logger.LogWarning("Status code {StatusCode} for {Path}", statusCode, requestPath);

                if (IsApiRequest(context))
                {
                    context.Response.ContentType = "application/json";
                    var errorResponse = new
                    {
                        success = false,
                        statusCode,
                        path = requestPath,
                        message = errorMessage
                    };

                    var json = JsonSerializer.Serialize(errorResponse);
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    context.Response.Redirect($"/Home/Error?statusCode={statusCode}");
                }
            }
        }
        catch (OperationCanceledException ex) when (context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning("Request was canceled by the client.");
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
        }
       

        catch (Exception ex)
        {
            try
            {
                var errorService = context.RequestServices.GetRequiredService<IErrorService>();
                var errorId = await errorService.HandleExceptionAsync(ex, context);

                if (!context.Response.HasStarted)
                {
                    if (IsApiRequest(context))
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var errorResponse = new
                        {
                            success = false,
                            errorId,
                            message = "An unexpected error occurred. Please contact support with the error ID."
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                    else
                    {
                        // 🚀 pass details for Development
                        var query = $"?errorId={errorId}&statusCode=500&path={Uri.EscapeDataString(context.Request.Path)}";
                        if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                        {
                            query += $"&message={Uri.EscapeDataString(ex.Message)}&stackTrace={Uri.EscapeDataString(ex.StackTrace ?? "")}";
                        }

                        context.Response.Redirect($"/Home/Error{query}");
                    }
                }
                else
                {
                    _logger.LogWarning("Response already started. Cannot modify response for exception.");
                }
            }
            catch (Exception handlerEx)
            {
                _logger.LogError(handlerEx, "Error in GlobalExceptionHandlerMiddleware while handling an exception.");
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("Critical error. Please contact support.");
                }
            }
        }

    }


    private bool IsApiRequest(HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/api") ||
               context.Request.Headers["Accept"].Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase));
    }
}


