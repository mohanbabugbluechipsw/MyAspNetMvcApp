//using BLL.ViewModels;
//using DAL.IRepositories;
//using DAL.Repositories;
//using Dapper;
//using DocumentFormat.OpenXml.Bibliography;
//using DocumentFormat.OpenXml.EMMA;
//using DocumentFormat.OpenXml.InkML;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.SqlServer.Server;
//using Model_New.Models;
//using Model_New.ViewModels;
//using Newtonsoft.Json;
//using System;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.Linq;
//using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using Microsoft.Extensions.Configuration;


//namespace MR_Application_New.Controllers
//{

//    [Authorize]

//    [AllowAnonymous]
//    public class ReviewPlaneController : Controller
//    {

//        private readonly MrAppDbNewContext _locationContext;

//        private readonly IConfiguration _configuration;





//        private readonly ILogger<ReviewPlaneController> _logger;




//        public ReviewPlaneController(MrAppDbNewContext mylocationContext, ILogger<ReviewPlaneController> logger, IConfiguration configuration)
//        {
//            _locationContext = mylocationContext;

//            _logger = logger;
//            _configuration=configuration;



//        }


//        public IActionResult Wizard()
//        {
//            return PartialView("~/Views/ReviewPlane/PartialViews/_Wizard.cshtml");
//        }





//        [HttpGet]
//        public async Task<IActionResult> GetRSCodes(string term)
//        {
//            if (string.IsNullOrEmpty(term))
//            {
//                return BadRequest(new { message = "MR Name is required." });
//            }

//            // Step 1: Find EmpNo in TblUsers based on EmpName (MR Name)
//            var user = await _locationContext.TblUsers
//                .Where(u => u.EmpName == term)
//                .Select(u => new { u.EmpNo, u.EmpName })
//                .FirstOrDefaultAsync();

//            if (user == null)
//            {
//                return NotFound(new { message = $"No employee found with name: {term}" });
//            }

//            // Step 2: Parse EmpNo to integer safely
//            if (!int.TryParse(user.EmpNo, out int empNo))
//            {
//                return NotFound(new
//                {
//                    message = $"Invalid employee number format for {user.EmpName}"
//                });
//            }

//            // Step 3: Filter TblDistributors using subquery
//            var rscodes = await _locationContext.TblDistributors
//                .Where(d => _locationContext.tbl_MRSrMappings
//                    .Where(m => m.MrEmpNo == empNo)
//                    .Any(m => m.Rs_code == d.Distributor.ToString()))
//                .Select(d => new
//                {
//                    code = d.Distributor.ToString(),
//                    name = d.DistributorName ?? "N/A"
//                })
//                .ToListAsync();

//            if (!rscodes.Any())
//            {
//                return NotFound(new
//                {
//                    message = $"No RSCODEs found for MR: {user.EmpName}"
//                });
//            }

//            return Ok(rscodes);
//        }


































//        [HttpGet]
//        public IActionResult GetOutlets(string term, string rscode, string? srname = null, string? routeName = null, string? mrCode = null)
//        {
//            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(rscode))
//            {
//                return BadRequest("Search term and RS Code are required.");
//            }

//            if (string.IsNullOrEmpty(srname) || string.IsNullOrEmpty(routeName))
//            {
//                return BadRequest("Please select SR Name and Route Name first.");
//            }

//            try
//            {



//                var query = from sr in _locationContext.tbl_SRMaster_Datas
//                            join outlet in _locationContext.OutLetMasterDetails
//                            on new
//                            {
//                                Code = sr.Party_HUL_Code,
//                                RS = sr.RS_Code
//                            }
//                            equals new
//                            {
//                                Code = outlet.PartyHllcode,
//                                RS = outlet.Rscode.ToString()
//                            }
//                            where sr.RS_Code == rscode &&
//                                  sr.SMN_Name == srname &&
//                                  sr.Beat == routeName &&
//                                  (
//                                      outlet.PartyHllcode.Contains(term) ||
//                                      outlet.SecondarychannelCode.Contains(term) ||
//                                      EF.Functions.Like(outlet.PartyName, term + "%")
//                                  )
//                            select new
//                            {
//                                code = outlet.PartyHllcode,
//                                name = outlet.PartyName,
//                                outletSubType = outlet.SecondarychannelCode,
//                                address = outlet.Address1 + " " + outlet.Address2 + " " + outlet.Address3,
//                                salesperson = sr.Salesperson,
//                                srName = sr.SMN_Name,
//                                beat = sr.Beat,
//                                childParty = sr.Child_Party,
//                                servicingPLG = sr.Servicing_PLG
//                            };

//                var result = query.ToList<object>();


//                //var result = query.ToList<object>();

//                return Ok(result.Any() ? result : new List<object>());
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching outlets: {Message}", ex.Message);
//                return StatusCode(500, "An error occurred while fetching outlets");
//            }
//        }


























//        public IActionResult Index()
//        {
//            return View();

//        }



//        [HttpPost]
//        public IActionResult ReviewPlane(string Latitude, string Longitude, string DeviceInfo, string DeviceType)
//        {
//            try
//            {
//                string userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
//                string userName = User?.Identity?.Name ?? "Unknown";

//                _logger.LogInformation("Received ReviewPlane POST by UserId: {UserId}, UserName: {UserName} | Latitude: {Latitude}, Longitude: {Longitude}, DeviceInfo: {DeviceInfo}, DeviceType: {DeviceType}",
//                                        userId, userName, Latitude, Longitude, DeviceInfo, DeviceType);

//                var reviewData = new tbl_UserGeoCodeDetails
//                {
//                    UserId = userId,
//                    UserName = userName,
//                    Latitude = Latitude,
//                    Longitude = Longitude,
//                    DeviceInfo = DeviceInfo,
//                    DeviceType = DeviceType,
//                    CreatedAt = DateTime.Now  // use UTC
//                };

//                _locationContext.tbl_UserGeoCodeDetails.Add(reviewData);
//                _locationContext.SaveChanges();

//                _logger.LogInformation("Review data saved successfully for user: {UserName} (ID: {UserId})", userName, userId);

//                var rsList = _locationContext.TblDistributors
//                    .Select(o => new { o.Distributor, o.DistributorName })
//                    .Distinct()
//                    .ToList();

//                if (!rsList.Any())
//                {
//                    _logger.LogWarning("No RS codes found in TblDistributors.");
//                    ViewBag.RSList = new List<object>();
//                }
//                else
//                {
//                    _logger.LogInformation("{RSListCount} RS codes fetched from TblDistributors.", rsList.Count);
//                    ViewBag.RSList = rsList;
//                }

//                return PartialView("~/Views/ReviewPlane/PartialViews/_ReviewPlane.cshtml");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while executing ReviewPlane.");
//                return RedirectToAction("Error", "Home", new { message = "An unexpected error occurred. Please try again later." });
//            }
//        }









































//        public IActionResult ReviewSummary()
//        {
//            return View();
//        }










































//        [HttpGet]
//        public IActionResult GetSrNames(string rscode)
//        {
//            if (string.IsNullOrWhiteSpace(rscode))
//                return BadRequest(new { success = false, message = "RS Code is required." });

//            try
//            {
//                var srDetails = _locationContext.tbl_SRMaster_Datas
//                    .AsNoTracking()
//                    .Where(s => s.RS_Code == rscode)
//                    .Select(s => new
//                    {
//                        srCode = s.Salesperson,
//                        srName = s.SMN_Name ?? "N/A",
//                        rscode = s.RS_Code,
//                        fullDetails = $"{s.RS_Code} - {s.Salesperson} - {s.SMN_Name}"
//                    })
//                    .AsEnumerable()
//                    .GroupBy(s => s.srCode)
//                    .Select(g => g.First())
//                    .OrderBy(s => s.srName)
//                    .ToList();

//                if (!srDetails.Any())
//                {
//                    return NotFound(new
//                    {
//                        success = false,
//                        message = "No SR details found for the specified RS Code."
//                    });
//                }

//                return Ok(new
//                {
//                    success = true,
//                    data = srDetails
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching SR names for RS Code: {Rscode}", rscode);
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "An error occurred while processing your request",
//                    error = ex.Message
//                });
//            }
//        }




//        public class RouteViewModel
//        {
//            public string Beat { get; set; }
//        }


//        [HttpGet]
//        public async Task<IActionResult> GetRouteNameOutlet(string rscode, string srname = null, string mrCode = null)
//        {
//            // Input validation
//            if (string.IsNullOrWhiteSpace(rscode))
//            {
//                return BadRequest("RS Code is required.");
//            }

//            try
//            {
//                List<RouteViewModel> routes;

//                if (!string.IsNullOrWhiteSpace(srname))
//                {
//                    // Scenario 1: Get routes by SR name
//                    routes = await GetRoutesBySrName(rscode, srname);
//                }

//                else
//                {
//                    // Scenario 3: Get all routes for RS code
//                    routes = await _locationContext.tbl_SRMaster_Datas
//                        .Where(r => r.RS_Code == rscode)
//                        .Select(r => new RouteViewModel { Beat = r.Beat })
//                        .Distinct()
//                        .ToListAsync();
//                }

//                if (routes == null || !routes.Any())
//                {
//                    return NotFound("No routes found for the given criteria.");
//                }

//                return Ok(routes);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching route names for RS Code: {Rscode}, SR Name: {Srname}, MR Code: {MrCode}",
//                    rscode, srname, mrCode);
//                return StatusCode(500, "An error occurred while processing your request.");
//            }
//        }

//        private async Task<List<RouteViewModel>> GetRoutesBySrName(string rscode, string srname)
//        {
//            return await _locationContext.tbl_SRMaster_Datas
//                .Where(r => r.RS_Code == rscode && r.SMN_Name == srname)
//                .Select(r => new RouteViewModel { Beat = r.Beat })
//                .Distinct()
//                .ToListAsync();
//        }






//        public JsonResult GetEmployees()
//        {
//            var employees = _locationContext.TblUsers
//                                    .Select(u => new { u.EmpName, u.EmpNo, u.IsActive })
//                                    .ToList();

//            return Json(employees);
//        }






//        public IActionResult FilteredReviewAnswers()
//        {




//            var model = new FilteredReviewAnswersViewModel();



//            return PartialView("~/Views/ReviewPlane/PartialViews/_FilteredReviewAnswers.cshtml", model);
//        }


//        private void HandleError(Exception ex, string actionName, Guid? reviewId = null)
//        {
//            var controllerName = ControllerContext.ActionDescriptor.ControllerName;

//            _logger.LogError(ex, "Error in {Controller}/{Action}", controllerName, actionName);

//            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            connection.Open();

//            var errorSql = @"
//                INSERT INTO tbl_ErrorLog (
//                    ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
//                    ReviewId, ServerName, ApplicationName, CreatedAt
//                )
//                VALUES (
//                    @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
//                    @ReviewId, @ServerName, @ApplicationName, SYSUTCDATETIME()
//                );";

//            connection.Execute(errorSql, new
//            {
//                ControllerName = controllerName,
//                ActionName = actionName,
//                ErrorCode = ex.HResult,
//                ErrorMessage = ex.Message,
//                StackTrace = ex.ToString(),
//                ReviewId = reviewId,
//                ServerName = Environment.MachineName,
//                ApplicationName = "MR_Application_New"
//            });
//        }

//        // ---------------------------------
//        // Centralized Error Handling Helper (Async)
//        // ---------------------------------
//        private async Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
//        {
//            var controllerName = ControllerContext.ActionDescriptor.ControllerName;

//            _logger.LogError(ex, "Error in {Controller}/{Action}", controllerName, actionName);

//            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            await connection.OpenAsync();

//            var errorSql = @"
//                INSERT INTO tbl_ErrorLog (
//                    ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
//                    ReviewId, ServerName, ApplicationName, CreatedAt
//                )
//                VALUES (
//                    @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
//                    @ReviewId, @ServerName, @ApplicationName, SYSUTCDATETIME()
//                );";

//            await connection.ExecuteAsync(errorSql, new
//            {
//                ControllerName = controllerName,
//                ActionName = actionName,
//                ErrorCode = ex.HResult,
//                ErrorMessage = ex.Message,
//                StackTrace = ex.ToString(),
//                ReviewId = reviewId,
//                ServerName = Environment.MachineName,
//                ApplicationName = "MR_Application_New"
//            });
//        }



















//        [HttpPost]
//        public async Task<IActionResult> SaveReview([FromForm] ReviewInputModel model)
//        {
//            if (model == null)
//            {
//                _logger.LogError("SaveReview - Null ReviewInputModel received");
//                return BadRequest(new { Success = false, Message = "Invalid request data." });
//            }

//            try
//            {
//                string locationStatus = "Location Not Available";
//                double? distance = null;

//                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//                await connection.OpenAsync();

//                if (model.ReviewId == Guid.Empty)
//                    model.ReviewId = Guid.NewGuid();

//                var exists = await connection.ExecuteScalarAsync<int>(
//                    "SELECT COUNT(1) FROM tbl_Savereview_details WHERE ReviewId = @ReviewId",
//                    new { model.ReviewId });

//                if (exists > 0)
//                {
//                    return Ok(new SaveReviewResponse
//                    {
//                        Success = true,
//                        Status = "AlreadySaved",
//                        Distance = "N/A",
//                        ReviewId = model.ReviewId
//                    });
//                }

//                var outlet = await connection.QueryFirstOrDefaultAsync<OutletDto>(
//                    @"SELECT Latitude, Longitude 
//              FROM OutLetMaster_Details 
//              WHERE PartyHllcode = @OutletCode AND Rscode = @Rscode",
//                    new { model.OutletCode, model.Rscode });

//                if (model.Latitude.HasValue && model.Longitude.HasValue &&
//                    outlet?.Latitude.HasValue == true && outlet?.Longitude.HasValue == true)
//                {
//                    distance = GetDistanceInMeters(model.Latitude.Value, model.Longitude.Value,
//                                                   outlet.Latitude.Value, outlet.Longitude.Value);
//                    locationStatus = (distance >= 0 && distance <= 100) ? "Verified" : "Not Verified";
//                }

//                var createdAtSL = TimeZoneInfo.ConvertTimeFromUtc(
//                    DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

//                string? cleanPhotoUrl = null;
//                if (!string.IsNullOrWhiteSpace(model.PhotoUrl))
//                {
//                    var uri = new Uri(model.PhotoUrl);
//                    cleanPhotoUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
//                }

//                var sql = @"
//        INSERT INTO tbl_Savereview_details (
//            ReviewId, MrName, Rscode, SrName, SrCode, RouteName,
//            OutletCode, OutletName, OutletSubType, OutletAddress,
//            ChildParty, ServicingPLG, PhotoUrl, Latitude, Longitude,
//            DeviceInfo, DeviceType, LocationStatus, DistanceFromOutlet, ReviewMessage,CreatedAt
//        ) VALUES (
//            @ReviewId, @MrName, @Rscode, @SrName, @SrCode, @RouteName,
//            @OutletCode, @OutletName, @OutletSubType, @OutletAddress,
//            @ChildParty, @ServicingPLG, @PhotoUrl, @Latitude, @Longitude,
//            @DeviceInfo, @DeviceType, @LocationStatus, @DistanceFromOutlet,    @ReviewMessage
//, @CreatedAt
//        );";

//                await connection.ExecuteAsync(sql, new
//                {
//                    model.ReviewId,
//                    MrName = model.MrName ?? string.Empty,
//                    Rscode = model.Rscode ?? string.Empty,
//                    SrName = model.SrName ?? string.Empty,
//                    SrCode = model.SrCode ?? string.Empty,
//                    RouteName = model.RouteName ?? string.Empty,
//                    OutletCode = model.OutletCode ?? string.Empty,
//                    OutletName = model.OutletName ?? string.Empty,
//                    OutletSubType = model.OutletSubType ?? string.Empty,
//                    OutletAddress = model.OutletAddress ?? string.Empty,
//                    ChildParty = model.ChildParty ?? string.Empty,
//                    ServicingPLG = model.ServicingPLG ?? string.Empty,
//                    PhotoUrl = cleanPhotoUrl ?? model.PhotoUrl,
//                    model.Latitude,
//                    model.Longitude,
//                    DeviceInfo = model.DeviceInfo ?? string.Empty,
//                    DeviceType = model.DeviceType ?? string.Empty,
//                    LocationStatus = locationStatus,
//                    DistanceFromOutlet = distance,
//                    ReviewMessage = model.ReviewMessage ?? string.Empty,
//                    CreatedAt = createdAtSL
//                }, commandTimeout: 1800);

//                return Ok(new SaveReviewResponse
//                {
//                    Success = true,
//                    Status = locationStatus,
//                    Distance = distance?.ToString("0.##") ?? "N/A",
//                    ReviewId = model.ReviewId
//                });
//            }
//            //catch (Exception ex)
//            //{
//            //    _logger.LogError(ex, "Error occurred in SaveReview");
//            //    return StatusCode(500, new
//            //    {
//            //        Success = false,
//            //        Message = $"An unexpected error occurred: {ex.Message}"
//            //    });
//            //}

//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error occurred in SaveReview");

//                var controllerName = ControllerContext.ActionDescriptor.ControllerName;
//                var actionName = ControllerContext.ActionDescriptor.ActionName;

//                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//                await connection.OpenAsync();

//                var errorSql = @"
//        INSERT INTO tbl_ErrorLog (
//            ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
//            ReviewId, ServerName, ApplicationName
//        )
//        VALUES (
//            @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
//            @ReviewId, @ServerName, @ApplicationName
//        );";

//                await connection.ExecuteAsync(errorSql, new
//                {
//                    ControllerName = controllerName,
//                    ActionName = actionName,
//                    ErrorCode = ex.HResult,   // maps to exception error code
//                    ErrorMessage = ex.Message,
//                    StackTrace = ex.ToString(),
//                    ReviewId = model?.ReviewId == Guid.Empty ? null : model?.ReviewId,
//                    ServerName = Environment.MachineName,
//                    ApplicationName = "MR_Application_New"
//                });

//                return StatusCode(500, new
//                {
//                    Success = false,
//                    Message = $"Error in {controllerName}/{actionName} - {ex.Message}"

//                });
//            }

//        }




//        private double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
//        {
//            const double R = 6371000; // Earth radius in meters
//            var dLat = ToRadians(lat2 - lat1);
//            var dLon = ToRadians(lon2 - lon1);
//            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
//                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
//                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
//            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
//            return R * c;
//        }

//        private double ToRadians(double degrees) => degrees * (Math.PI / 180);





//        public class OutletDto
//        {
//            public double? Latitude { get; set; }
//            public double? Longitude { get; set; }
//        }


//        public class SaveReviewResponse
//        {
//            public bool Success { get; set; }
//            public string Status { get; set; }
//            public string Distance { get; set; }
//            public Guid ReviewId { get; set; }   // New property
//        }



//    }
//}


using BLL.ViewModels;
using DAL.IRepositories;
using DAL.Repositories;
using Dapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.SqlServer.Server;
using Model_New.Models;
using Model_New.ViewModels;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MR_Application_New.Controllers
{
    [Authorize]
    [AllowAnonymous]
    public class ReviewPlaneController : Controller
    {
        private readonly MrAppDbNewContext _locationContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReviewPlaneController> _logger;

        public ReviewPlaneController(MrAppDbNewContext mylocationContext, ILogger<ReviewPlaneController> logger, IConfiguration configuration)
        {
            _locationContext = mylocationContext;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Wizard()
        {
            try
            {
                _logger.LogInformation("Wizard action executed successfully");
                return PartialView("~/Views/ReviewPlane/PartialViews/_Wizard.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Wizard action");
                return StatusCode(500, "An error occurred while loading the wizard");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRSCodes(string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    _logger.LogWarning("GetRSCodes called with empty term parameter");
                    return BadRequest(new { message = "MR Name is required." });
                }

                // Step 1: Find EmpNo in TblUsers based on EmpName (MR Name)
                var user = await _locationContext.TblUsers
                    .Where(u => u.EmpName == term)
                    .Select(u => new { u.EmpNo, u.EmpName })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("No employee found with name: {Term}", term);
                    return NotFound(new { message = $"No employee found with name: {term}" });
                }

                // Step 2: Parse EmpNo to integer safely
                if (!int.TryParse(user.EmpNo, out int empNo))
                {
                    _logger.LogWarning("Invalid employee number format for {EmpName}: {EmpNo}", user.EmpName, user.EmpNo);
                    return NotFound(new
                    {
                        message = $"Invalid employee number format for {user.EmpName}"
                    });
                }

                // Step 3: Filter TblDistributors using subquery
                var rscodes = await _locationContext.TblDistributors
                    .Where(d => _locationContext.tbl_MRSrMappings
                        .Where(m => m.MrEmpNo == empNo)
                        .Any(m => m.Rs_code == d.Distributor.ToString()))
                    .Select(d => new
                    {
                        code = d.Distributor.ToString(),
                        name = d.DistributorName ?? "N/A"
                    })
                    .ToListAsync();

                if (!rscodes.Any())
                {
                    _logger.LogWarning("No RSCODEs found for MR: {EmpName}", user.EmpName);
                    return NotFound(new
                    {
                        message = $"No RSCODEs found for MR: {user.EmpName}"
                    });
                }

                _logger.LogInformation("Successfully retrieved {Count} RSCodes for MR: {EmpName}", rscodes.Count, user.EmpName);
                return Ok(rscodes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRSCodes for term: {Term}", term);
                 HandleErrorAsync(ex, nameof(GetRSCodes));
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOutlets(string term, string rscode, string? srname = null, string? routeName = null, string? mrCode = null)
        
       {
            try
            {
                if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(rscode))
                {
                    _logger.LogWarning("GetOutlets called with missing parameters - Term: {Term}, RSCode: {Rscode}", term, rscode);
                    return BadRequest("Search term and RS Code are required.");
                }

                if (string.IsNullOrEmpty(srname) || string.IsNullOrEmpty(routeName))
                {
                    _logger.LogWarning("GetOutlets called without SR Name or Route Name - SR: {Srname}, Route: {RouteName}", srname, routeName);
                    return BadRequest("Please select SR Name and Route Name first.");
                }

                var query = from sr in _locationContext.tbl_SRMaster_Datas
                            join outlet in _locationContext.OutLetMasterDetails
                            on new
                            {
                                Code = sr.Party_HUL_Code,
                                RS = sr.RS_Code
                            }
                            equals new
                            {
                                Code = outlet.PartyHllcode,
                                RS = outlet.Rscode.ToString()
                            }
                            where sr.RS_Code == rscode &&
                                  sr.SMN_Name == srname &&
                                  sr.Beat == routeName &&
                                  (
                                      outlet.PartyHllcode.Contains(term) ||
                                      outlet.SecondarychannelCode.Contains(term) ||
                                      EF.Functions.Like(outlet.PartyName, term + "%")
                                  )
                            select new
                            {
                                code = outlet.PartyHllcode,
                                name = outlet.PartyName,
                                outletSubType = outlet.SecondarychannelCode,
                                address = outlet.Address1 + " " + outlet.Address2 + " " + outlet.Address3,
                                salesperson = sr.Salesperson,
                                srName = sr.SMN_Name,
                                beat = sr.Beat,
                                childParty = sr.Child_Party,
                                servicingPLG = sr.Servicing_PLG
                            };

                var result = await query.ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} outlets for RSCode: {Rscode}, SR: {Srname}, Route: {RouteName}",
                    result.Count, rscode, srname, routeName);

                return Ok(result.Any() ? result : new List<object>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOutlets - Term: {Term}, RSCode: {Rscode}, SR: {Srname}, Route: {RouteName}",
                    term, rscode, srname, routeName);
                 HandleErrorAsync(ex, nameof(GetOutlets));
                return StatusCode(500, "An error occurred while fetching outlets");
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Index action executed successfully");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                 HandleErrorAsync(ex, nameof(Index));
                return RedirectToAction("Error", "Home", new { message = "An unexpected error occurred. Please try again later." });
            }
        }


        // GET: CheckReviewExists

        [HttpGet]
        public IActionResult CheckReviewExists(string preVisitGuid, string currentReviewId)
        {
            string userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";

            // Try to parse currentReviewId
            if (!string.IsNullOrEmpty(currentReviewId) && Guid.TryParse(currentReviewId, out Guid reviewGuid))
            {
                // Check if review exists
                bool reviewExists = _locationContext.tbl_Savereview_details.Any(r => r.ReviewId == reviewGuid);

                if (reviewExists)
                {
                    Guid? preVisitGuidGuid = null;

                    // Parse preVisitGuid only if it's valid
                    if (!string.IsNullOrEmpty(preVisitGuid) && Guid.TryParse(preVisitGuid, out Guid tempGuid))
                        preVisitGuidGuid = tempGuid;

                    if (preVisitGuidGuid.HasValue)
                    {
                        using (var connection = _locationContext.Database.GetDbConnection())
                        {
                            string sql = @"
                        SELECT TOP 1 VisitType, ChannelType
                        FROM TblStoreVisitAnswer
                        WHERE RowGuid = @RowGuid";

                            var answer = connection.QueryFirstOrDefault(sql, new { RowGuid = preVisitGuidGuid.Value });

                            if (answer != null)
                            {
                                // Both review & answer exist → redirect based on channelType
                                return Json(new
                                {
                                    exists = true,
                                    reviewId = reviewGuid,
                                    skipPage = true,
                                    channelType = answer.ChannelType,
                                    visitType = answer.VisitType
                                });
                            }
                        }
                    }

                    // Only review exists → go to OutletView page
                    return Json(new
                    {
                        exists = true,
                        reviewId = reviewGuid,
                        skipPage = true,
                        outletView = true
                    });
                }
                else
                {
                    // Review not found → normal page
                    return Json(new { exists = false, skipPage = false });
                }
            }
            else
            {
                // currentReviewId empty → check any ongoing answers
                using (var connection = _locationContext.Database.GetDbConnection())
                {
                    string sql = @"
                SELECT COUNT(1)
                FROM TblStoreVisitAnswer
                WHERE UserId = @UserId
                  AND Stage != 'Completed'";

                    int count = connection.QuerySingle<int>(sql, new { UserId = userId });

                    return Json(new { exists = count > 0, skipPage = false });
                }
            }
        }


        // POST: ReviewPlane
        [HttpPost]
        public async Task<IActionResult> ReviewPlane(
            string Latitude, string Longitude, string DeviceInfo, string DeviceType, string SkippedGeo, string LocationReason = null)
        {
            try
            {
                string userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                string userName = User?.Identity?.Name ?? "Unknown";

                bool isSkipped = SkippedGeo?.ToLower() == "true";

                // Save GeoCode only if not skipped
                if (!isSkipped)
                {
                    DeviceInfo = string.IsNullOrEmpty(DeviceInfo) ? "Unknown" : DeviceInfo;
                    DeviceType = string.IsNullOrEmpty(DeviceType) ? "Unknown" : DeviceType;
                    Latitude = string.IsNullOrEmpty(Latitude) ? "0" : Latitude;
                    Longitude = string.IsNullOrEmpty(Longitude) ? "0" : Longitude;

                    var reviewData = new tbl_UserGeoCodeDetails
                    {
                        UserId = userId,
                        UserName = userName,
                        Latitude = Latitude,
                        Longitude = Longitude,
                        DeviceInfo = DeviceInfo,
                        DeviceType = DeviceType,
                        //LocationReason = LocationReason ?? "Not provided",
                        CreatedAt = DateTime.Now
                    };

                    _locationContext.tbl_UserGeoCodeDetails.Add(reviewData);
                    await _locationContext.SaveChangesAsync();
                }

                // Always return ReviewPlane partial
                return PartialView("~/Views/ReviewPlane/PartialViews/_ReviewPlane.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReviewPlane action");
                HandleErrorAsync(ex, nameof(ReviewPlane));
                return RedirectToAction("Error", "Home", new { message = "An unexpected error occurred. Please try again later." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> MoveToTrackLog(Guid reviewId, Guid rowGuid)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await conn.OpenAsync();

                var currentUser = User.Identity?.Name ?? "Unknown";
                var deletionTime = DateTime.Now;
                int recordsProcessed = 0;
                List<string> processedRecords = new List<string>();

                // Process Review (only if ReviewId is valid)
                if (reviewId != Guid.Empty)
                {
                    var reviewRecord = await conn.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM tbl_Savereview_details WHERE ReviewId = @ReviewId",
                        new { ReviewId = reviewId });

                    if (reviewRecord != null)
                    {
                        // Log the deletion
                        await conn.ExecuteAsync(@"
                    INSERT INTO tbl_DeleteAuditLog (
                        TableName, 
                        DeletedRecordId, 
                        DeletedData, 
                        DeletedBy, 
                        DeletedDate, 
                        AdditionalInfo
                    ) VALUES (
                        'tbl_Savereview_details',
                        @Id,
                        @JsonData,
                        @UserName,
                        @DeletionTime,
                        @AdditionalInfo
                    )",
                            new
                            {
                                Id = reviewRecord.Id,
                                JsonData = JsonConvert.SerializeObject(reviewRecord),
                                UserName = currentUser,
                                DeletionTime = deletionTime,
                                AdditionalInfo = $"Moved to track log via API | ReviewId: {reviewId}"
                            });

                        // Delete the record
                        await conn.ExecuteAsync(
                            "DELETE FROM tbl_Savereview_details WHERE ReviewId = @ReviewId",
                            new { ReviewId = reviewId });

                        recordsProcessed++;
                        processedRecords.Add($"Review (ID: {reviewId})");
                        _logger.LogInformation("Moved review record to track log | ReviewId: {ReviewId}", reviewId);
                    }
                    else
                    {
                        _logger.LogWarning("Review record not found for deletion | ReviewId: {ReviewId}", reviewId);
                    }
                }

                // Process Answer (only if RowGuid is valid)
                if (rowGuid != Guid.Empty)
                {
                    var answerRecord = await conn.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT * FROM TblStoreVisitAnswer WHERE RowGuid = @RowGuid",
                        new { RowGuid = rowGuid });

                    if (answerRecord != null)
                    {
                        // Log the deletion
                        await conn.ExecuteAsync(@"
                    INSERT INTO tbl_DeleteAuditLog (
                        TableName, 
                        DeletedRecordId, 
                        DeletedData, 
                        DeletedBy, 
                        DeletedDate, 
                        AdditionalInfo
                    ) VALUES (
                        'TblStoreVisitAnswer',
                        @Id,
                        @JsonData,
                        @UserName,
                        @DeletionTime,
                        @AdditionalInfo
                    )",
                            new
                            {
                                Id = answerRecord.Id,
                                JsonData = JsonConvert.SerializeObject(answerRecord),
                                UserName = currentUser,
                                DeletionTime = deletionTime,
                                AdditionalInfo = $"Moved to track log via API | RowGuid: {rowGuid}"
                            });

                        // Delete the record
                        await conn.ExecuteAsync(
                            "DELETE FROM TblStoreVisitAnswer WHERE RowGuid = @RowGuid",
                            new { RowGuid = rowGuid });

                        recordsProcessed++;
                        processedRecords.Add($"Answer (RowGuid: {rowGuid})");
                        _logger.LogInformation("Moved answer record to track log | RowGuid: {RowGuid}", rowGuid);
                    }
                    else
                    {
                        _logger.LogWarning("Answer record not found for deletion | RowGuid: {RowGuid}", rowGuid);
                    }
                }

                // Prepare response
                string message;
                bool success;

                if (recordsProcessed > 0)
                {
                    success = true;
                    message = $"{recordsProcessed} record(s) moved to track log successfully: {string.Join(", ", processedRecords)}";
                }
                else
                {
                    // Determine why no records were processed
                    if (reviewId == Guid.Empty && rowGuid == Guid.Empty)
                    {
                        message = "No valid identifiers provided. Please provide either ReviewId or RowGuid.";
                        success = false;
                    }
                    else if (reviewId != Guid.Empty && rowGuid != Guid.Empty)
                    {
                        message = "No records found for the provided ReviewId and RowGuid.";
                        success = false;
                    }
                    else if (reviewId != Guid.Empty)
                    {
                        message = "No review record found for the provided ReviewId.";
                        success = false;
                    }
                    else
                    {
                        message = "No answer record found for the provided RowGuid.";
                        success = false;
                    }

                    _logger.LogWarning("MoveToTrackLog completed with no records processed | ReviewId: {ReviewId}, RowGuid: {RowGuid}",
                        reviewId, rowGuid);
                }

                return Json(new
                {
                    success,
                    message,
                    recordsProcessed,
                    processedRecords = processedRecords.ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReviewPlane.MoveToTrackLog | ReviewId: {ReviewId}, RowGuid: {RowGuid}", reviewId, rowGuid);
                await HandleErrorAsync(ex, nameof(ReviewPlane), reviewId);
                return Json(new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }


        public IActionResult ReviewSummary()
        {
            try
            {
                _logger.LogInformation("ReviewSummary action executed successfully");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReviewSummary action");
                HandleErrorAsync(ex, nameof(ReviewSummary));
                return RedirectToAction("Error", "Home", new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSrNames(string rscode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rscode))
                {
                    _logger.LogWarning("GetSrNames called with empty rscode parameter");
                    return BadRequest(new { success = false, message = "RS Code is required." });
                }

                var srDetails = await _locationContext.tbl_SRMaster_Datas
                    .AsNoTracking()
                    .Where(s => s.RS_Code == rscode)
                    .Select(s => new
                    {
                        srCode = s.Salesperson,
                        srName = s.SMN_Name ?? "N/A",
                        rscode = s.RS_Code,
                        fullDetails = $"{s.RS_Code} - {s.Salesperson} - {s.SMN_Name}"
                    })
                    .ToListAsync();

                var distinctSrDetails = srDetails
                    .GroupBy(s => s.srCode)
                    .Select(g => g.First())
                    .OrderBy(s => s.srName)
                    .ToList();

                if (!distinctSrDetails.Any())
                {
                    _logger.LogWarning("No SR details found for RS Code: {Rscode}", rscode);
                    return NotFound(new
                    {
                        success = false,
                        message = "No SR details found for the specified RS Code."
                    });
                }

                _logger.LogInformation("Successfully retrieved {Count} SR names for RS Code: {Rscode}", distinctSrDetails.Count, rscode);
                return Ok(new
                {
                    success = true,
                    data = distinctSrDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSrNames for RS Code: {Rscode}", rscode);
                 HandleErrorAsync(ex, nameof(GetSrNames));
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while processing your request"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRouteNameOutlet(string rscode, string srname = null, string mrCode = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rscode))
                {
                    _logger.LogWarning("GetRouteNameOutlet called with empty rscode parameter");
                    return BadRequest("RS Code is required.");
                }

                List<RouteViewModel> routes;

                if (!string.IsNullOrWhiteSpace(srname))
                {
                    routes = await GetRoutesBySrName(rscode, srname);
                }
                else
                {
                    routes = await _locationContext.tbl_SRMaster_Datas
                        .Where(r => r.RS_Code == rscode)
                        .Select(r => new RouteViewModel { Beat = r.Beat })
                        .Distinct()
                        .ToListAsync();
                }

                if (routes == null || !routes.Any())
                {
                    _logger.LogWarning("No routes found for RS Code: {Rscode}, SR Name: {Srname}", rscode, srname);
                    return NotFound("No routes found for the given criteria.");
                }

                _logger.LogInformation("Successfully retrieved {Count} routes for RS Code: {Rscode}, SR Name: {Srname}",
                    routes.Count, rscode, srname);
                return Ok(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRouteNameOutlet for RS Code: {Rscode}, SR Name: {Srname}", rscode, srname);
                 HandleErrorAsync(ex, nameof(GetRouteNameOutlet));
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private async Task<List<RouteViewModel>> GetRoutesBySrName(string rscode, string srname)
        {
            return await _locationContext.tbl_SRMaster_Datas
                .Where(r => r.RS_Code == rscode && r.SMN_Name == srname)
                .Select(r => new RouteViewModel { Beat = r.Beat })
                .Distinct()
                .ToListAsync();
        }

        public async Task<JsonResult> GetEmployees()
        {
            try
            {
                var employees = await _locationContext.TblUsers
                    .Select(u => new { u.EmpName, u.EmpNo, u.IsActive })
                    .ToListAsync();

                _logger.LogInformation("Successfully retrieved {Count} employees", employees.Count);
                return Json(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetEmployees");
                 HandleErrorAsync(ex, nameof(GetEmployees));
                return Json(new { error = "An error occurred while fetching employees" });
            }
        }

        public IActionResult FilteredReviewAnswers()
        {
            try
            {
                var model = new FilteredReviewAnswersViewModel();
                _logger.LogInformation("FilteredReviewAnswers action executed successfully");
                return PartialView("~/Views/ReviewPlane/PartialViews/_FilteredReviewAnswers.cshtml", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FilteredReviewAnswers");
                HandleErrorAsync(ex, nameof(FilteredReviewAnswers));
                return StatusCode(500, "An error occurred while loading the view");
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveReview([FromForm] ReviewInputModel model)
        //{
        //    if (model == null)
        //    {
        //        _logger.LogError("SaveReview - Null ReviewInputModel received");
        //        return BadRequest(new { Success = false, Message = "Invalid request data." });
        //    }

        //    try
        //    {
        //        string locationStatus = "Location Not Available";
        //        double? distance = null;

        //        using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        await connection.OpenAsync();

        //        if (model.ReviewId == Guid.Empty)
        //            model.ReviewId = Guid.NewGuid();

        //        var exists = await connection.ExecuteScalarAsync<int>(
        //            "SELECT COUNT(1) FROM tbl_Savereview_details WHERE ReviewId = @ReviewId",
        //            new { model.ReviewId });

        //        if (exists > 0)
        //        {
        //            _logger.LogInformation("Review with ID {ReviewId} already exists", model.ReviewId);
        //            return Ok(new SaveReviewResponse
        //            {
        //                Success = true,
        //                Status = "AlreadySaved",
        //                Distance = "N/A",
        //                ReviewId = model.ReviewId
        //            });
        //        }

        //        var outlet = await connection.QueryFirstOrDefaultAsync<OutletDto>(
        //            @"SELECT Latitude, Longitude 
        //      FROM OutLetMaster_Details 
        //      WHERE PartyHllcode = @OutletCode AND Rscode = @Rscode",
        //            new { model.OutletCode, model.Rscode });

        //        if (model.Latitude.HasValue && model.Longitude.HasValue &&
        //            outlet?.Latitude.HasValue == true && outlet?.Longitude.HasValue == true)
        //        {
        //            distance = GetDistanceInMeters(model.Latitude.Value, model.Longitude.Value,
        //                                           outlet.Latitude.Value, outlet.Longitude.Value);
        //            locationStatus = (distance >= 0 && distance <= 100) ? "Verified" : "Not Verified";
        //        }

        //        var createdAtSL = TimeZoneInfo.ConvertTimeFromUtc(
        //            DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

        //        string? cleanPhotoUrl = null;
        //        if (!string.IsNullOrWhiteSpace(model.PhotoUrl))
        //        {
        //            var uri = new Uri(model.PhotoUrl);
        //            cleanPhotoUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
        //        }

        //        var sql = @"
        //INSERT INTO tbl_Savereview_details (
        //    ReviewId, MrName, Rscode, SrName, SrCode, RouteName,
        //    OutletCode, OutletName, OutletSubType, OutletAddress,
        //    ChildParty, ServicingPLG, PhotoUrl, Latitude, Longitude,
        //    DeviceInfo, DeviceType, LocationStatus, DistanceFromOutlet, ReviewMessage,CreatedAt
        //) VALUES (
        //    @ReviewId, @MrName, @Rscode, @SrName, @SrCode, @RouteName,
        //    @OutletCode, @OutletName, @OutletSubType, @OutletAddress,
        //    @ChildParty, @ServicingPLG, @PhotoUrl, @Latitude, @Longitude,
        //    @DeviceInfo, @DeviceType, @LocationStatus, @DistanceFromOutlet, @ReviewMessage, @CreatedAt
        //);";

        //        await connection.ExecuteAsync(sql, new
        //        {
        //            model.ReviewId,
        //            MrName = model.MrName ?? string.Empty,
        //            Rscode = model.Rscode ?? string.Empty,
        //            SrName = model.SrName ?? string.Empty,
        //            SrCode = model.SrCode ?? string.Empty,
        //            RouteName = model.RouteName ?? string.Empty,
        //            OutletCode = model.OutletCode ?? string.Empty,
        //            OutletName = model.OutletName ?? string.Empty,
        //            OutletSubType = model.OutletSubType ?? string.Empty,
        //            OutletAddress = model.OutletAddress ?? string.Empty,
        //            ChildParty = model.ChildParty ?? string.Empty,
        //            ServicingPLG = model.ServicingPLG ?? string.Empty,
        //            PhotoUrl = cleanPhotoUrl ?? model.PhotoUrl,
        //            model.Latitude,
        //            model.Longitude,
        //            DeviceInfo = model.DeviceInfo ?? string.Empty,
        //            DeviceType = model.DeviceType ?? string.Empty,
        //            LocationStatus = locationStatus,
        //            DistanceFromOutlet = distance,
        //            ReviewMessage = model.ReviewMessage ?? string.Empty,
        //            CreatedAt = createdAtSL
        //        }, commandTimeout: 1800);

        //        _logger.LogInformation("Review saved successfully with ID: {ReviewId}, Status: {Status}", model.ReviewId, locationStatus);

        //        return Ok(new SaveReviewResponse
        //        {
        //            Success = true,
        //            Status = locationStatus,
        //            Distance = distance?.ToString("0.##") ?? "N/A",
        //            ReviewId = model.ReviewId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in SaveReview for ReviewId: {ReviewId}", model?.ReviewId);
        //        await HandleErrorAsync(ex, nameof(SaveReview), model?.ReviewId);
        //        return StatusCode(500, new
        //        {
        //            Success = false,
        //            Message = "An error occurred while saving the review"
        //        });
        //    }
        //}

        private double GetDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * (Math.PI / 180);



        [HttpPost]
        public async Task<IActionResult> SaveReview([FromForm] ReviewInputModel model)
        {
            if (model == null)
            {
                _logger.LogError("SaveReview - Null ReviewInputModel received");
                return BadRequest(new { Success = false, Message = "Invalid request data." });
            }

            try
            {
                string locationStatus = "Location Not Available";
                double? distance = null;

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.OpenAsync();

                if (model.ReviewId == Guid.Empty)
                    model.ReviewId = Guid.NewGuid();

                var exists = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM tbl_Savereview_details WHERE ReviewId = @ReviewId",
                    new { model.ReviewId });

                if (exists > 0)
                {
                    _logger.LogInformation("Review with ID {ReviewId} already exists", model.ReviewId);
                    return Ok(new SaveReviewResponse
                    {
                        Success = true,
                        Status = "AlreadySaved",
                        Distance = "N/A",
                        ReviewId = model.ReviewId
                    });
                }

                var outlet = await connection.QueryFirstOrDefaultAsync<OutletDto>(
                    @"SELECT Latitude, Longitude 
              FROM OutLetMaster_Details 
              WHERE PartyHllcode = @OutletCode AND Rscode = @Rscode",
                    new { model.OutletCode, model.Rscode });

                if (model.Latitude.HasValue && model.Longitude.HasValue &&
                    outlet?.Latitude.HasValue == true && outlet?.Longitude.HasValue == true)
                {
                    distance = GetDistanceInMeters(model.Latitude.Value, model.Longitude.Value,
                                                   outlet.Latitude.Value, outlet.Longitude.Value);
                    locationStatus = (distance >= 0 && distance <= 100) ? "Verified" : "Not Verified";
                }

                var createdAtSL = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));

                string? cleanPhotoUrl = null;
                if (!string.IsNullOrWhiteSpace(model.PhotoUrl))
                {
                    var uri = new Uri(model.PhotoUrl);
                    cleanPhotoUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
                }

                // 1️⃣ Insert into tbl_Savereview_details
                var sql = @"
                  INSERT INTO tbl_Savereview_details (
    ReviewId, MrName, Rscode, SrName, SrCode, RouteName,
    OutletCode, OutletName, OutletSubType, OutletAddress,
    ChildParty, ServicingPLG, PhotoUrl, Latitude, Longitude,
    DeviceInfo, DeviceType, LocationStatus, DistanceFromOutlet, ReviewMessage, CreatedAt
) VALUES (
    @ReviewId, @MrName, @Rscode, @SrName, @SrCode, @RouteName,
    @OutletCode, @OutletName, @OutletSubType, @OutletAddress,
    @ChildParty, @ServicingPLG, @PhotoUrl, @Latitude, @Longitude,
    @DeviceInfo, @DeviceType, @LocationStatus, @DistanceFromOutlet, @ReviewMessage, @CreatedAt
);";

                await connection.ExecuteAsync(sql, new
                {
                    model.ReviewId,
                    MrName = model.MrName ?? string.Empty,
                    Rscode = model.Rscode ?? string.Empty,
                    SrName = model.SrName ?? string.Empty,
                    SrCode = model.SrCode ?? string.Empty,
                    RouteName = model.RouteName ?? string.Empty,
                    OutletCode = model.OutletCode ?? string.Empty,
                    OutletName = model.OutletName ?? string.Empty,
                    OutletSubType = model.OutletSubType ?? string.Empty,
                    OutletAddress = model.OutletAddress ?? string.Empty,
                    ChildParty = model.ChildParty ?? string.Empty,
                    ServicingPLG = model.ServicingPLG ?? string.Empty,
                    PhotoUrl = cleanPhotoUrl ?? model.PhotoUrl,
                    model.Latitude,
                    model.Longitude,
                    DeviceInfo = model.DeviceInfo ?? string.Empty,
                    DeviceType = model.DeviceType ?? string.Empty,
                    LocationStatus = locationStatus,
                    DistanceFromOutlet = distance,
                    ReviewMessage = model.ReviewMessage ?? string.Empty,
                    CreatedAt = createdAtSL
                }, commandTimeout: 1800);

                _logger.LogInformation("Review saved successfully with ID: {ReviewId}", model.ReviewId);

                return Ok(new SaveReviewResponse
                {
                    Success = true,
                    Status = locationStatus,
                    Distance = distance?.ToString("0.##") ?? "N/A",
                    ReviewId = model.ReviewId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveReview for ReviewId: {ReviewId}", model?.ReviewId);
                HandleErrorAsync(ex, nameof(SaveReview), model?.ReviewId);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while saving the review"
                });
            }
        }




        private async System.Threading.Tasks.Task HandleErrorAsync(Exception ex, string actionName, Guid? reviewId = null)
        {
            var controllerName = ControllerContext.ActionDescriptor.ControllerName;

            _logger.LogError(ex, "Error in {Controller}/{Action}", controllerName, actionName);

            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            var errorSql = @"
        INSERT INTO tbl_ErrorLog (
            ControllerName, ActionName, ErrorCode, ErrorMessage, StackTrace,
            ReviewId, ServerName, ApplicationName, CreatedAt
        )
        VALUES (
            @ControllerName, @ActionName, @ErrorCode, @ErrorMessage, @StackTrace,
            @ReviewId, @ServerName, @ApplicationName, SYSDATETIMEOFFSET() AT TIME ZONE 'India Standard Time'
        );";

            await connection.ExecuteAsync(errorSql, new
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ErrorCode = ex.HResult,
                ErrorMessage = ex.Message,
                StackTrace = ex.ToString(),
                ReviewId = reviewId,
                ServerName = Environment.MachineName,
                ApplicationName = "MR_Application_New"
            });

            // No return needed for async Task
        }



    }

    public class RouteViewModel
    {
        public string Beat { get; set; }
    }

    public class OutletDto
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class SaveReviewResponse
    {
        public bool Success { get; set; }
        public string Status { get; set; }
        public string Distance { get; set; }
        public Guid ReviewId { get; set; }
    }
}
