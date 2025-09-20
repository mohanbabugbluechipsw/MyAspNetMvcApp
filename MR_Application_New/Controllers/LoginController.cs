


using BLL.Services;
using DAL.IRepositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using Model_New.ViewModels;
using MR_Application_New.Filters;
using MR_Application_New.Filters.Action;
using MR_Application_New.Filters.Authorization;
using MR_Application_New.Filters.Result;
using System.Security.Claims;
using System.Security.Principal;
using Model_New.Models;
using Microsoft.EntityFrameworkCore;


namespace MR_Application_New.Controllers
{

   


   [Authorize]
    [AllowAnonymous]

    public class LoginController : Controller
    {
        private readonly IGenericRepository<TblUser, MrAppDbNewContext> _loginService;
        private readonly ILogger<LoginController> _logger;
        private readonly MrAppDbNewContext _locationContext;
        private CommonService commonService = new CommonService();
        private readonly IGenericRepository<TblSystemUser, MrAppDbNewContext> generic;

        public LoginController(
            IGenericRepository<TblUser, MrAppDbNewContext> loginService,
            IGenericRepository<TblSystemUser, MrAppDbNewContext> _generic,
            ILogger<LoginController> logger,
            MrAppDbNewContext mylocationContext)
        {
            _loginService = loginService;
            _locationContext = mylocationContext;
            generic = _generic;
            _logger = logger;
        }

        
        [HttpGet]
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                // Get all roles from claims
                var roles = User.Claims
                                .Where(c => c.Type == ClaimTypes.Role)
                                .Select(c => c.Value)
                                .ToList();

                if (roles.Contains("Administrator"))
                    return RedirectToAction("Index", "Admin");

                if (roles.Contains("Merchandiser User"))
                    return RedirectToAction("Index", "ReviewPlane");

                if (roles.Contains("Account Manager"))
                    return RedirectToAction("Dashboard", "AccountManager");

                // Add more roles here if needed
                return RedirectToAction("Index", "Home"); // fallback
            }

            // Not authenticated, show login
            return View();
        }




        //change1
        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> Index(LoginViewModel model)
        //{
        //    try
        //    {
        //        // ✅ Basic validation
        //        if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
        //        {
        //            ViewData["ErrorMessage"] = "Username and password cannot be empty.";
        //            _logger.LogWarning("Login failed: Missing credentials.");
        //            return View(model);
        //        }

        //        if (string.IsNullOrWhiteSpace(model.PhotoUrl))
        //        {
        //            ViewData["ErrorMessage"] = "Please capture your image.";
        //            _logger.LogWarning("Login failed: No photo url captured by user '{UserName}'.", model.UserName);
        //            return View(model);
        //        }

        //        // ✅ Fetch user
        //        var users = commonService.GetUsers(model.UserName, model.Password);
        //        if (users == null || !users.Any())
        //        {
        //            ViewData["ErrorMessage"] = "Invalid username or password.";
        //            _logger.LogWarning("Login failed: Invalid credentials for '{UserName}'.", model.UserName);
        //            return View(model);
        //        }

        //        var currentUser = users.First();

        //        // ✅ Active check
        //        if ((bool)!currentUser.IsActive)
        //        {
        //            _logger.LogWarning("Login blocked: Inactive user '{UserName}'.", model.UserName);
        //            return RedirectToAction("AccessDenied", "Login");
        //        }

        //        // ✅ Role → Redirect map (based on UserTypeName instead of Id)
        //        var roleRedirects = new Dictionary<string, (string Controller, string Action)>(StringComparer.OrdinalIgnoreCase)
        //{
        //    { "SuperAdmin", ("SuperAdmin", "Index") },
        //    { "Administrator", ("Admin", "Index") },
        //    { "Manager", ("Manager", "Dashboard") },
        //    { "Merchandiser User", ("ReviewPlane", "Index") },

        //            {"account manager" ,("AccountManager", "Dashboard") },

        //    { "Reporting User", ("Reports", "Index") }
        //};

        //        if (string.IsNullOrWhiteSpace(currentUser.UserTypeName) || !roleRedirects.ContainsKey(currentUser.UserTypeName))
        //        {
        //            ViewData["ErrorMessage"] = "Your role is not assigned or invalid. Contact admin.";
        //            _logger.LogWarning("Login failed: Missing/invalid role for '{UserName}'.", model.UserName);
        //            return View(model);
        //        }

        //        var (controller, action) = roleRedirects[currentUser.UserTypeName];

        //        // ✅ Claims
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.Name, currentUser.EmpName ?? ""),
        //    new Claim(ClaimTypes.Email, currentUser.EmpEmail ?? ""),
        //    new Claim(ClaimTypes.NameIdentifier, currentUser.EmpNo ?? ""),
        //    new Claim(ClaimTypes.Role, currentUser.UserTypeName ?? ""), // role name here
        //    new Claim("LastActivity", DateTime.Now.ToString())
        //};

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //        await HttpContext.SignInAsync(
        //            CookieAuthenticationDefaults.AuthenticationScheme,
        //            new ClaimsPrincipal(claimsIdentity),
        //            new AuthenticationProperties
        //            {
        //                IsPersistent = true,
        //                ExpiresUtc = DateTime.UtcNow.AddHours(6),
        //                AllowRefresh = true
        //            });

        //        // ✅ Custom cookie (copy default auth cookie)
        //        var authCookieValue = HttpContext.Request.Cookies[".AspNetCore.Cookies"];
        //        if (!string.IsNullOrEmpty(authCookieValue))
        //        {
        //            Response.Cookies.Append(
        //                "MRApp.Auth",
        //                authCookieValue,
        //                new CookieOptions
        //                {
        //                    Expires = DateTimeOffset.Now.AddHours(6),
        //                    HttpOnly = true,
        //                    Secure = true,
        //                    SameSite = SameSiteMode.Lax,
        //                    Path = "/"
        //                });
        //        }

        //        HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString());

        //        // ✅ Save only filename from blob url
        //        var fileNameOnly = Path.GetFileName(new Uri(model.PhotoUrl).AbsolutePath);

        //        var loginCapture = new Tbl_LogindetailsCapture
        //        {
        //            UserId = currentUser.EmpNo,
        //            Username = currentUser.EmpName,
        //            ImageFileName = fileNameOnly,
        //            BlobUrl = model.PhotoUrl,
        //            CaptureDate = TimeZoneInfo.ConvertTimeFromUtc(
        //                DateTime.UtcNow,
        //                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        //            )
        //        };

        //        _locationContext.Tbl_LogindetailsCapture.Add(loginCapture);
        //        await _locationContext.SaveChangesAsync();

        //        _logger.LogInformation("Login successful. Role: {Role}, User: '{UserName}', EmpNo: '{EmpNo}'",
        //            currentUser.UserTypeName, model.UserName, currentUser.EmpNo);

        //        // ✅ Redirect based on role
        //        return RedirectToAction(action, controller);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Login error.");
        //        ViewData["ErrorMessage"] = "We are experiencing technical difficulties. Please try again later.";
        //        return View(model);
        //    }
        //}


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            try
            {
                // ✅ Basic validation
                if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                {
                    ViewData["ErrorMessage"] = "Username and password cannot be empty.";
                    _logger.LogWarning("Login failed: Missing credentials.");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.PhotoUrl))
                {
                    ViewData["ErrorMessage"] = "Please capture your image.";
                    _logger.LogWarning("Login failed: No photo url captured by user '{UserName}'.", model.UserName);
                    return View(model);
                }

                // ✅ Fetch user
                var users = commonService.GetUsers(model.UserName, model.Password);
                if (users == null || !users.Any())
                {
                    ViewData["ErrorMessage"] = "Invalid username or password.";
                    _logger.LogWarning("Login failed: Invalid credentials for '{UserName}'.", model.UserName);
                    return View(model);
                }

                var currentUser = users.First();

                // ✅ Active check
                if ((bool)!currentUser.IsActive)
                {
                    _logger.LogWarning("Login blocked: Inactive user '{UserName}'.", model.UserName);
                    return RedirectToAction("AccessDenied", "Login");
                }

                // ✅ Claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, currentUser.EmpName ?? ""),
            new Claim(ClaimTypes.Email, currentUser.EmpEmail ?? ""),
            new Claim(ClaimTypes.NameIdentifier, currentUser.EmpNo ?? ""),
            new Claim(ClaimTypes.Role, currentUser.UserTypeName ?? ""),
            new Claim("LastActivity", DateTime.Now.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(6),
                        AllowRefresh = true
                    });

                // ✅ Custom cookie
                var authCookieValue = HttpContext.Request.Cookies[".AspNetCore.Cookies"];
                if (!string.IsNullOrEmpty(authCookieValue))
                {
                    Response.Cookies.Append(
                        "MRApp.Auth",
                        authCookieValue,
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.Now.AddHours(6),
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Path = "/"
                        });
                }

                HttpContext.Session.SetString("LastActivity", DateTime.Now.ToString());

                // ✅ Save login capture
                var fileNameOnly = Path.GetFileName(new Uri(model.PhotoUrl).AbsolutePath);
                var loginCapture = new Tbl_LogindetailsCapture
                {
                    UserId = currentUser.EmpNo,
                    Username = currentUser.EmpName,
                    ImageFileName = fileNameOnly,
                    BlobUrl = model.PhotoUrl,
                    CaptureDate = TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                    )
                };
                _locationContext.Tbl_LogindetailsCapture.Add(loginCapture);
                await _locationContext.SaveChangesAsync();

                _logger.LogInformation("Login successful. Role: {Role}, User: '{UserName}', EmpNo: '{EmpNo}'",
                    currentUser.UserTypeName, model.UserName, currentUser.EmpNo);

              

                var role = (currentUser.UserTypeName ?? "").Trim();

                return role.ToLower() switch
                {
                    "superadmin" => RedirectToAction("Index", "SuperAdmin"),
                    "administrator" => RedirectToAction("Index", "Admin"),
                    "manager" => RedirectToAction("Dashboard", "Manager"),
                    "merchandiser user" => RedirectToAction("Index", "ReviewPlane"),
                    "reporting user" => RedirectToAction("Index", "Reports"),
                    "account manager" => RedirectToAction("Dashboard", "AccountManager"),
                    "fieldmanager" => RedirectToAction("Dashboard", "FieldManager"),
                    _ => RedirectToAction("AccessDenied", "Login")
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error.");
                ViewData["ErrorMessage"] = "We are experiencing technical difficulties. Please try again later.";
                return View(model);
            }
        }









        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var empNo = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(empNo))
                {
                    // Get the latest login/capture record without LogoutDate
                    var lastLogin = await _locationContext.Tbl_LogindetailsCapture
                        .Where(x => x.UserId == empNo && x.LogoutDate == null)
                        .OrderByDescending(x => x.CaptureDate)
                        .FirstOrDefaultAsync();

                    if (lastLogin != null)
                    {
                        var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(
                            DateTime.UtcNow,
                            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                        );

                        lastLogin.LogoutDate = indiaTime;

                        // Update LastActivity to the latest action time
                        lastLogin.LastActivity = indiaTime;

                        await _locationContext.SaveChangesAsync();
                    }
                }

                // Sign out the authentication scheme
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Clear all cookies
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }

                // Clear session
                HttpContext.Session.Clear();

                _logger.LogInformation("User '{UserName}' logged out successfully.", User.Identity?.Name ?? "Unknown");

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user '{UserName}'", User.Identity?.Name ?? "Unknown");
                return RedirectToAction("Index", "Login");
            }
        }



        


        [HttpPost]
        [Route("/Login/UpdateActivity")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken] // Only use if you must allow anonymous JS calls
        public IActionResult UpdateActivity()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Unauthorized" });
                }

                // Update session last activity (IST)
                var lastActivity = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
                );
                HttpContext.Session.SetString("LastActivity", lastActivity.ToString("o"));

                return Json(new { success = true, lastActivity });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateActivity failed for user '{UserName}'", User.Identity?.Name ?? "Unknown");
                return Json(new { success = false, message = "Server error" });
            }
        }



        [HttpPost]
        [Route("/ClientError")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public IActionResult LogClientError([FromBody] ClientError error)
        {
            _logger.LogWarning("Client Error: {Message}\n{Stack}", error.message, error.stack);
            return Ok();
        }

      



     

        public IActionResult AccessDenied()
        {
            return View();
        }

        public class ClientError
        {
            public string message { get; set; }
            public string stack { get; set; }
            public string time { get; set; }
        }
    }
}