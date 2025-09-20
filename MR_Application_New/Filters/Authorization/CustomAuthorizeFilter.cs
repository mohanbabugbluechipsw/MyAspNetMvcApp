using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MR_Application_New.Filters.Authorization
{
   

    public class CustomAuthorizeFilter : IAuthorizationFilter
    {
        private readonly ILogger<CustomAuthorizeFilter> _logger;

        public CustomAuthorizeFilter(ILogger<CustomAuthorizeFilter> logger)
        {
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var path = context.HttpContext.Request.Path;
            if (path.StartsWithSegments("/Login") ||
                path.StartsWithSegments("/Home") ||
                path.StartsWithSegments("/health"))
            {
                return;
            }

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Unauthorized access attempt to {Path}", path);
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }
}
