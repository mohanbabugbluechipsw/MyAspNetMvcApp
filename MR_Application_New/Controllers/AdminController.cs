


using BLL.Services;
using DAL;
using DAL.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model_New.Models;
using System.Security.Claims;

namespace MR_Application_New.Controllers
{
    [Authorize(Roles = "Admin")]

    [Authorize]

    [AllowAnonymous]
    public class AdminController : Controller
    {
        private readonly UnitOfWork<MrAppDbNewContext> unitOfWork;
        private readonly ILogger<AdminController> _logger;

        // Constructor with dependency injection
        public AdminController(
            UnitOfWork<MrAppDbNewContext> unitOfWork,
            ILogger<AdminController> logger)
        {
            this.unitOfWork = unitOfWork;
            _logger = logger;
        }

        //// Admin Dashboard
        //[Route("Admin/Index")]
        //public IActionResult Index()
        //{


        //    ViewBag.IsAdmin = User.IsInRole("Admin");
        //    _logger.LogInformation("AdminDashboard page accessed");

        //    return View();
        //}


        // Admin Dashboard
        [Route("Admin/Index")]
        public IActionResult Index()
        {
            try
            {
                // Check if user is authenticated
                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("Unauthorized access attempt to AdminDashboard.");
                    return RedirectToAction("Index", "Login");
                }

                // Check role from claim
                var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                // Consider "Administrator" as admin
                ViewBag.IsAdmin = string.Equals(roleClaim, "Administrator", StringComparison.OrdinalIgnoreCase);

                if (!ViewBag.IsAdmin)
                {
                    _logger.LogWarning("Access denied: User '{UserName}' with role '{Role}' tried to access AdminDashboard.",
                        User.Identity.Name, roleClaim);
                    return RedirectToAction("AccessDenied", "Login");
                }

                _logger.LogInformation("AdminDashboard page accessed by '{UserName}'", User.Identity.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AdminDashboard for user '{UserName}'", User.Identity.Name);
                return RedirectToAction("Index", "Login");
            }
        }


        // Distributions List
        public IActionResult DistributionsList()
        {
            try
            {
                var distributions = unitOfWork.Tbl_Distributor.GetAll().ToList();
                return PartialView("~/Views/Admin/PartialViews/_DistributionsList.cshtml", distributions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distribution list.");
                TempData["AlertMessage"] = "An error occurred while fetching the distribution list.";
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Get the form to add a distribution
        [HttpGet]
        public IActionResult AddDistribution()
        {
            try
            {
                _logger.LogInformation("Accessed the Add Distribution form.");
                return PartialView("~/Views/Admin/PartialViews/_AddDistribution.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while rendering AddDistribution form.");
                TempData["AlertMessage"] = "An error occurred while accessing the Add Distribution form.";
                return StatusCode(500, "Internal Server Error");
            }
        }

        

        [HttpPost]
        public async Task<IActionResult> AddDistribution(TblDistributor distribution)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Invalid data. Please correct the highlighted errors.";
                TempData["AlertType"] = "warning"; // Alert type for validation errors
                return Json(new { success = false, errors = ModelState.ToDictionary(k => k.Key, v => string.Join(", ", v.Value.Errors.Select(e => e.ErrorMessage))) });
            }

            try
            {
                await unitOfWork.Tbl_Distributor.AddAsync(distribution);
                await unitOfWork.SaveAsync();

                TempData["AlertMessage"] = "Distributor added successfully!";
                TempData["AlertType"] = "success"; // Alert type for success
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding distributor.");
                TempData["AlertMessage"] = "An error occurred while saving the distributor.";
                TempData["AlertType"] = "danger"; // Alert type for errors
                return Json(new { success = false });
            }
        }


        [HttpGet]
        public IActionResult ADDMRMaster()
        {
            try
            {
                _logger.LogInformation("Accessed the Add MRMaster form.");
                return PartialView("~/Views/Admin/PartialViews/_ADDMRMaster.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while rendering AddDistribution form.");
                TempData["AlertMessage"] = "An error occurred while accessing the Add Distribution form.";
                return StatusCode(500, "Internal Server Error");
            }
        }



       



        public IActionResult Outletcreate()
        {
            // Create an instance of OutLetMasterDetail and pass it to the view
            var model = new OutLetMasterDetail();
            return PartialView("~/Views/Admin/PartialViews/_Outletcreate.cshtml", model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Outletcreate([Bind("Id, Rscode, RsName, PartyMasterCode, PartyHllcode, PartyName, PrimaryChannel, SecondaryChannel, Category, ParStatus, UpdateStamp, OlCreatedDate, Address1, Address2, Address3, Address4, Latitude, Longitude, PrimarychannelCode, SecondarychannelCode")] OutLetMasterDetail outletMasterDetail)
        {
            if (ModelState.IsValid)
            {
                await unitOfWork.OutL_etMasterDetails.AddAsync(outletMasterDetail);
                await unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(outletMasterDetail);
        }


    }
}

