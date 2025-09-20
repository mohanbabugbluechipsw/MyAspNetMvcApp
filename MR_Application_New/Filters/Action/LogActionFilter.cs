using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Model_New.ViewModels;

namespace MR_Application_New.Filters.Action
{
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation("Executing {ActionName} in {ControllerName}", actionName, controllerName);

            // Example validation - ensure image is provided for login
            if (context.ActionArguments.ContainsKey("model") &&
                context.ActionArguments["model"] is LoginViewModel model)
            {
                if (model.PhotoUrl == null)
                {
                    context.Result = new BadRequestObjectResult("Image is required");
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.Controller.GetType().Name;
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation("Executed {ActionName} in {ControllerName}", actionName, controllerName);
        }
    }
}
