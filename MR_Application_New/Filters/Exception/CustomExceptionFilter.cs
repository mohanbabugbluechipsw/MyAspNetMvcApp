

// CustomExceptionFilter.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using System.Net;
using System.Security.Authentication;

namespace MR_Application_New.Filters
{
    public class CustomExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;
        private readonly MrAppDbNewContext _dbContext;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger, MrAppDbNewContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            // Skip if handled elsewhere
            if (context.ExceptionHandled) return;

            // Log business exceptions specifically

            // Handle known exception types
            switch (context.Exception)
            {
                case DbUpdateException dbEx:
                    context.Result = new JsonResult(new { error = "Database update failed" })
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                    break;

                case AuthenticationException authEx:
                    context.Result = new RedirectToActionResult("Index", "Login", null);
                    break;

                default:
                    // Let middleware handle unexpected exceptions
                    return;
            }

            context.ExceptionHandled = true;
        }

       
    }
}