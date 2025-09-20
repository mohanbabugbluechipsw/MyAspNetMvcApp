using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;

namespace MR_Application_New.Controllers
{
    public class ReportsController : Controller
    {

        private readonly IReportRepository _repository;

        private readonly MrAppDbNewContext _context;



        public ReportsController(IReportRepository repository, MrAppDbNewContext context)
        {
            _repository = repository;
            _context = context;

        }


     

        // Private method to load MR dropdown list
        private async Task LoadUsersToViewBag()
        {
            var users = await _context.TblUsers
                .OrderBy(u => u.EmpName)
                .Select(u => new SelectListItem
                {
                    Text = u.EmpNo + "  " + u.EmpName, // Display EmpNo + EmpName
                    Value = u.EmpName                  // Only EmpName used as value
                })
                .ToListAsync();

            ViewBag.Users = users;
        }

        [HttpGet]
        public async Task<IActionResult> VisitStatusPieChart(string mrName, DateTime? startDate, DateTime? endDate)
        {
            // Always load dropdown data
            await LoadUsersToViewBag();

            // Set previously selected values for repopulating form
            ViewBag.MrName = mrName;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // If any input is missing
            if (string.IsNullOrWhiteSpace(mrName) || !startDate.HasValue || !endDate.HasValue)
            {
                ViewBag.ErrorMessage = "Please enter all required fields.";
                return View();
            }

            // Fetch chart data
            var result = await _repository.GetVisitStatusSummaryAsync(mrName, startDate.Value, endDate.Value);

            // If no matching data found
            if (result == null || result.TotalCount == 0)
            {
                ViewBag.ErrorMessage = "No data found for the selected MR and date range.";
                return View();
            }

            // Data found, show chart
            return View(result);
        }



    }
}
