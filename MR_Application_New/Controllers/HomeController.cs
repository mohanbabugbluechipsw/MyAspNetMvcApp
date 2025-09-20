


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model_New.ViewModels;
using System;


namespace MR_Application_New.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }





        public IActionResult Error(int? statusCode = null, Guid? errorId = null, string path = null, string message = null, string stackTrace = null)
        {
            var model = new ErrorViewModel
            {
                ErrorId = errorId ?? Guid.NewGuid(),
                StatusCode = statusCode ?? 500,
                Path = path ?? "/Unknown",
                Message = message ?? "An unexpected error occurred.",
                StackTrace = stackTrace,
                ShowDetailedError = _env.IsDevelopment()
            };

            Response.StatusCode = model.StatusCode ?? 500;
            return View(model);
        }



    }
}


namespace Model_New.ViewModels
{
    public class ErrorViewModel
    {
        public int? StatusCode { get; set; }
        public Guid ErrorId { get; set; }
        public bool ShowDetailedError { get; set; }

        // extra dynamic details
        public string Path { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}

