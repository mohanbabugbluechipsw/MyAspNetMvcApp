

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MR_Application_New.Filters.Result
{
    public class CustomResultFilter : IResultFilter
    {
        private readonly ILogger<CustomResultFilter> _logger;

        public CustomResultFilter(ILogger<CustomResultFilter> logger)
        {
            _logger = logger;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // Add security headers
            context.HttpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.HttpContext.Response.Headers.Append("X-Frame-Options", "DENY");

            if (context.Result is ViewResult viewResult)
            {
                _logger.LogDebug("Preparing view {ViewName}", viewResult.ViewName);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            _logger.LogInformation("Completed execution of {Action}", context.ActionDescriptor.DisplayName);
        }
    }
}
