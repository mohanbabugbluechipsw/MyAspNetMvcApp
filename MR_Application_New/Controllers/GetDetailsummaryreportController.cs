

//using ClosedXML.Excel;
//using DAL.IRepositories;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Model_New.Models;
//using Model_New.ViewModels;
//using System.Security.Claims;

//namespace MR_Application_New.Controllers
//{
//    [Route("GetDetailsummaryreport")]
//    public class GetDetailsummaryreportController : Controller
//    {
//        private readonly IGetDailydetailsummaryReportRepository _repository;
//        private readonly MrAppDbNewContext _context;

//        public GetDetailsummaryreportController(
//            IGetDailydetailsummaryReportRepository repository,
//            MrAppDbNewContext context)
//        {
//            _repository = repository;
//            _context = context;
//        }

//        private async Task LoadUsersToViewBag()
//        {
//            var users = await _context.TblUsers
//                .Select(u => new SelectListItem
//                {
//                    Text = u.EmpNo + "  " + u.EmpName,
//                    Value = u.EmpNo // Changed to use EmpNo as value
//                })
//                .ToListAsync();

//            ViewBag.Users = users;
//        }

//        [HttpGet("Index")]
//        public async Task<IActionResult> Index(
//            [FromQuery] string mrCode,
//            [FromQuery] DateTime? monthStart,
//            [FromQuery] DateTime? monthEnd,
//            int page = 1,
//            int pageSize = 10,
//            string sortColumn = "RS_Code",
//            string sortOrder = "asc")
//        {
//            await LoadUsersToViewBag();

//            // Get current user info if mrCode not provided
//            if (string.IsNullOrEmpty(mrCode))
//            {

//                var userName = User.Identity.Name;
//                var currentUser = await _context.TblUsers
//                    .FirstOrDefaultAsync(u => u.UserName == userName);


//                //var currentUser = await _context.TblUsers
//                //    .FirstOrDefaultAsync(u => u.EmpName == User.Identity.Name);

//                if (currentUser != null)
//                {
//                    mrCode = currentUser.EmpNo;
//                    ViewBag.MrName = currentUser.EmpName;
//                }
//            }
//            else
//            {
//                // Get name for the selected mrCode
//                var user = await _context.TblUsers
//                    .FirstOrDefaultAsync(u => u.EmpNo == mrCode);
//                ViewBag.MrName = user?.EmpName;
//            }

//            ViewBag.MrCode = mrCode;
//            ViewBag.MonthStart = monthStart?.ToString("yyyy-MM-dd");
//            ViewBag.MonthEnd = monthEnd?.ToString("yyyy-MM-dd");
//            ViewBag.PageSize = pageSize;
//            ViewBag.SortColumn = sortColumn;
//            ViewBag.SortOrder = sortOrder;

//            if (string.IsNullOrEmpty(mrCode))
//            {
//                ViewBag.TotalPages = 0;
//                ViewBag.CurrentPage = 1;
//                return View(new List<Tbl_DetailsummaryReportDto>());
//            }

//            if (monthStart == null || monthEnd == null)
//            {
//                // Set default date range if not provided
//                monthStart = DateTime.Today.AddMonths(-1);
//                monthEnd = DateTime.Today;
//            }

//            var data = await _repository.GetOutletStatusAsync(mrCode, monthStart.Value, monthEnd.Value);

//            // Sorting logic
//            data = sortColumn switch
//            {
//                "RS_Code" => sortOrder == "asc" ? data.OrderBy(x => x.RS_Code).ToList() : data.OrderByDescending(x => x.RS_Code).ToList(),
//                "OutletCode" => sortOrder == "asc" ? data.OrderBy(x => x.OutletCode).ToList() : data.OrderByDescending(x => x.OutletCode).ToList(),
//                "Status" => sortOrder == "asc" ? data.OrderBy(x => x.Status).ToList() : data.OrderByDescending(x => x.Status).ToList(),
//                "StartTime" => sortOrder == "asc" ? data.OrderBy(x => x.StartTime).ToList() : data.OrderByDescending(x => x.StartTime).ToList(),
//                "EndTime" => sortOrder == "asc" ? data.OrderBy(x => x.EndTime).ToList() : data.OrderByDescending(x => x.EndTime).ToList(),
//                _ => data.OrderBy(x => x.RS_Code).ToList()
//            };

//            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
//            ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
//            ViewBag.CurrentPage = page;

//            return View(pagedData);
//        }

//        [HttpPost("Index")]
//        public async Task<IActionResult> IndexPost(
//            string mrCode,
//            DateTime? monthStart,
//            DateTime? monthEnd)
//        {
//            return RedirectToAction("Index", new
//            {
//                mrCode,
//                monthStart = monthStart?.ToString("yyyy-MM-dd"),
//                monthEnd = monthEnd?.ToString("yyyy-MM-dd"),
//                page = 1
//            });
//        }

//        public async Task<IActionResult> ExportToExcel(
//            string mrCode,
//            string monthStart,
//            string monthEnd,
//            string sortColumn,
//            string sortOrder)
//        {
//            if (string.IsNullOrWhiteSpace(mrCode) ||
//                string.IsNullOrWhiteSpace(monthStart) ||
//                string.IsNullOrWhiteSpace(monthEnd))
//            {
//                return BadRequest("MR Code, Start Date, and End Date are required.");
//            }

//            var data = await _repository.GetOutletStatusAsync(
//                mrCode,
//                DateTime.Parse(monthStart),
//                DateTime.Parse(monthEnd));

//            // Apply sorting
//            data = sortColumn switch
//            {
//                "RS_Code" => sortOrder == "asc" ? data.OrderBy(x => x.RS_Code).ToList() : data.OrderByDescending(x => x.RS_Code).ToList(),
//                "OutletCode" => sortOrder == "asc" ? data.OrderBy(x => x.OutletCode).ToList() : data.OrderByDescending(x => x.OutletCode).ToList(),
//                "Status" => sortOrder == "asc" ? data.OrderBy(x => x.Status).ToList() : data.OrderByDescending(x => x.Status).ToList(),
//                "StartTime" => sortOrder == "asc" ? data.OrderBy(x => x.StartTime).ToList() : data.OrderByDescending(x => x.StartTime).ToList(),
//                "EndTime" => sortOrder == "asc" ? data.OrderBy(x => x.EndTime).ToList() : data.OrderByDescending(x => x.EndTime).ToList(),
//                _ => data
//            };

//            using (var workbook = new XLWorkbook())
//            {
//                var worksheet = workbook.Worksheets.Add("Outlet Report");

//                // Add headers
//                var headers = new string[] { "RS Code", "Outlet Code", "SR Name", "SR Code", "Route Name",
//                                           "Child Party", "Servicing PLG", "Status", "Start Time", "End Time",
//                                           "Total Minutes", "Total Time Spent", "Row No" };

//                for (int i = 0; i < headers.Length; i++)
//                {
//                    worksheet.Cell(1, i + 1).Value = headers[i];
//                }

//                // Add data
//                int row = 2;
//                foreach (var item in data)
//                {
//                    worksheet.Cell(row, 1).Value = item.RS_Code;        // maps to v.Rscode
//                    worksheet.Cell(row, 2).Value = item.OutletCode;    // maps to v.OutletCode
//                    worksheet.Cell(row, 3).Value = item.OutletName;   // maps to v.OutletName

//                    worksheet.Cell(row, 4).Value = item.MrName;        // maps to v.MrName
//                    worksheet.Cell(row, 5).Value = item.SrCode;        // maps to v.SrCode
//                    worksheet.Cell(row, 6).Value = item.RouteName;     // maps to v.RouteName
//                    worksheet.Cell(row, 7).Value = item.ChildParty;    // maps to v.ChildParty
//                    worksheet.Cell(row, 8).Value = item.ServicingPLG;  // maps to v.ServicingPLG
//                    worksheet.Cell(row, 9).Value = item.Status;        // Completed / Visited
//                    worksheet.Cell(row, 10).Value = item.StartDate + " " + item.StartTime; // StartDate + StartTime
//                    worksheet.Cell(row, 11).Value = item.EndDate + " " + item.EndTime;    // EndDate + EndTime
//                    //worksheet.Cell(row, 11).Value = item.OutletName;   // maps to v.OutletName
//                    worksheet.Cell(row, 12).Value = item.TotalTimeSpent; // TotalTimeSpent
//                    row++;
//                }

//                using (var stream = new MemoryStream())
//                {
//                    workbook.SaveAs(stream);
//                    stream.Position = 0;
//                    return File(stream.ToArray(),
//                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                        $"OutletStatus_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
//                }
//            }
//        }
//    }
//}

using ClosedXML.Excel;
using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using Model_New.ViewModels;
using System.Security.Claims;

namespace MR_Application_New.Controllers
{
    [Route("GetDetailsummaryreport")]
    public class GetDetailsummaryreportController : Controller
    {
        private readonly IGetDailydetailsummaryReportRepository _repository;
        private readonly MrAppDbNewContext _context;

        public GetDetailsummaryreportController(
            IGetDailydetailsummaryReportRepository repository,
            MrAppDbNewContext context)
        {
            _repository = repository;
            _context = context;
        }

        private async Task LoadUsersToViewBag()
        {
            var users = await _context.TblUsers
                .Select(u => new SelectListItem
                {
                    Text = $"{u.EmpNo}  {u.EmpName}",
                    Value = u.EmpNo
                })
                .ToListAsync();

            ViewBag.Users = users;
        }

        //[HttpGet("Index")]
        //public async Task<IActionResult> Index(
        //    [FromQuery] string mrCode,
        //    [FromQuery] DateTime? monthStart,
        //    [FromQuery] DateTime? monthEnd,
        //    int page = 1,
        //    int pageSize = 10,
        //    string sortColumn = "RS_Code",
        //    string sortOrder = "asc")
        //{
        //    await LoadUsersToViewBag();

        //    // If no MR selected, get current logged-in user
        //    if (string.IsNullOrEmpty(mrCode) && User.Identity.IsAuthenticated)
        //    {
        //        var empNoClaim = User.FindFirstValue("EmpNo"); // Use claim if available
        //        if (!string.IsNullOrEmpty(empNoClaim))
        //        {
        //            mrCode = empNoClaim;
        //        }
        //        else
        //        {
        //            // fallback using username
        //            var currentUser = await _context.TblUsers
        //                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        //            if (currentUser != null)
        //                mrCode = currentUser.EmpNo;
        //        }

        //        if (!string.IsNullOrEmpty(mrCode))
        //        {
        //            var user = await _context.TblUsers.FirstOrDefaultAsync(u => u.EmpNo == mrCode);
        //            ViewBag.MrName = user?.EmpName;
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(mrCode))
        //    {
        //        var user = await _context.TblUsers.FirstOrDefaultAsync(u => u.EmpNo == mrCode);
        //        ViewBag.MrName = user?.EmpName;
        //    }

        //    ViewBag.MrCode = mrCode;
        //    ViewBag.MonthStart = monthStart?.ToString("yyyy-MM-dd");
        //    ViewBag.MonthEnd = monthEnd?.ToString("yyyy-MM-dd");
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.SortColumn = sortColumn;
        //    ViewBag.SortOrder = sortOrder;

        //    if (string.IsNullOrEmpty(mrCode))
        //    {
        //        ViewBag.TotalPages = 0;
        //        ViewBag.CurrentPage = 1;
        //        return View(new List<Tbl_DetailsummaryReportDto>());
        //    }

        //    monthStart ??= DateTime.Today.AddMonths(-1);
        //    monthEnd ??= DateTime.Today;

        //    var data = await _repository.GetOutletStatusAsync(mrCode, monthStart.Value, monthEnd.Value);

        //    // Sorting
        //    data = (sortColumn, sortOrder.ToLower()) switch
        //    {
        //        ("RS_Code", "asc") => data.OrderBy(x => x.RS_Code).ToList(),
        //        ("RS_Code", "desc") => data.OrderByDescending(x => x.RS_Code).ToList(),
        //        ("OutletCode", "asc") => data.OrderBy(x => x.OutletCode).ToList(),
        //        ("OutletCode", "desc") => data.OrderByDescending(x => x.OutletCode).ToList(),
        //        ("Status", "asc") => data.OrderBy(x => x.Status).ToList(),
        //        ("Status", "desc") => data.OrderByDescending(x => x.Status).ToList(),
        //        ("StartTime", "asc") => data.OrderBy(x => x.StartTime).ToList(),
        //        ("StartTime", "desc") => data.OrderByDescending(x => x.StartTime).ToList(),
        //        ("EndTime", "asc") => data.OrderBy(x => x.EndTime).ToList(),
        //        ("EndTime", "desc") => data.OrderByDescending(x => x.EndTime).ToList(),
        //        _ => data.OrderBy(x => x.RS_Code).ToList()
        //    };

        //    var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //    ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
        //    ViewBag.CurrentPage = page;

        //    return View(pagedData);
        //}


        [HttpGet("Index")]
        public async Task<IActionResult> Index(
    [FromQuery] string mrCode,
    [FromQuery] DateTime? monthStart,
    [FromQuery] DateTime? monthEnd,
    int page = 1,
    int pageSize = 10,
    string sortColumn = "RS_Code",
    string sortOrder = "asc")
        {
            await LoadUsersToViewBag();

            TblUser user = null;

            if (string.IsNullOrEmpty(mrCode) && User.Identity.IsAuthenticated)
            {
                // Try to get MR code from claim
                var empNoClaim = User.FindFirstValue("EmpNo");
                mrCode = !string.IsNullOrEmpty(empNoClaim) ? empNoClaim : null;

                if (string.IsNullOrEmpty(mrCode))
                {
                    // fallback using username
                    user = await _context.TblUsers
                        .FirstOrDefaultAsync(u => u.EmpName == User.Identity.Name);
                    mrCode = user?.EmpNo;
                }
            }

            // Get MR user if mrCode exists
            if (string.IsNullOrEmpty(mrCode))
            {
                ViewBag.MrName = User.Identity?.Name ?? "Unknown MR";
                ViewBag.MrCode = null;
                ViewBag.TotalPages = 0;
                ViewBag.CurrentPage = 1;
                return View(new List<Tbl_DetailsummaryReportDto>());
            }

            user ??= await _context.TblUsers.FirstOrDefaultAsync(u => u.EmpNo == mrCode);
            ViewBag.MrName = user?.EmpName ?? User.Identity?.Name ?? "Unknown MR";
            ViewBag.MrCode = mrCode;

            monthStart ??= DateTime.Today.AddMonths(-1);
            monthEnd ??= DateTime.Today;

            var data = await _repository.GetOutletStatusAsync(user?.EmpName, monthStart.Value, monthEnd.Value);

            // Sorting
            data = (sortColumn, sortOrder.ToLower()) switch
            {
                ("RS_Code", "asc") => data.OrderBy(x => x.RS_Code).ToList(),
                ("RS_Code", "desc") => data.OrderByDescending(x => x.RS_Code).ToList(),
                ("OutletCode", "asc") => data.OrderBy(x => x.OutletCode).ToList(),
                ("OutletCode", "desc") => data.OrderByDescending(x => x.OutletCode).ToList(),
                ("Status", "asc") => data.OrderBy(x => x.Status).ToList(),
                ("Status", "desc") => data.OrderByDescending(x => x.Status).ToList(),
                ("StartTime", "asc") => data.OrderBy(x => x.StartTime).ToList(),
                ("StartTime", "desc") => data.OrderByDescending(x => x.StartTime).ToList(),
                ("EndTime", "asc") => data.OrderBy(x => x.EndTime).ToList(),
                ("EndTime", "desc") => data.OrderByDescending(x => x.EndTime).ToList(),
                _ => data.OrderBy(x => x.RS_Code).ToList()
            };

            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.MonthStart = monthStart.Value.ToString("yyyy-MM-dd");
            ViewBag.MonthEnd = monthEnd.Value.ToString("yyyy-MM-dd");
            ViewBag.PageSize = pageSize;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            return View(pagedData);
        }


        [HttpPost("Index")]
        public IActionResult IndexPost(string mrCode, DateTime? monthStart, DateTime? monthEnd)
        {
            return RedirectToAction("Index", new
            {
                mrCode,
                monthStart = monthStart?.ToString("yyyy-MM-dd"),
                monthEnd = monthEnd?.ToString("yyyy-MM-dd"),
                page = 1
            });
        }

        public async Task<IActionResult> ExportToExcel(
            string mrCode,
            string monthStart,
            string monthEnd,
            string sortColumn,
            string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(mrCode) ||
                string.IsNullOrWhiteSpace(monthStart) ||
                string.IsNullOrWhiteSpace(monthEnd))
            {
                return BadRequest("MR Code, Start Date, and End Date are required.");
            }

            var data = await _repository.GetOutletStatusAsync(
                mrCode,
                DateTime.Parse(monthStart),
                DateTime.Parse(monthEnd));

            // Sorting
            data = (sortColumn, sortOrder.ToLower()) switch
            {
                ("RS_Code", "asc") => data.OrderBy(x => x.RS_Code).ToList(),
                ("RS_Code", "desc") => data.OrderByDescending(x => x.RS_Code).ToList(),
                ("OutletCode", "asc") => data.OrderBy(x => x.OutletCode).ToList(),
                ("OutletCode", "desc") => data.OrderByDescending(x => x.OutletCode).ToList(),
                ("Status", "asc") => data.OrderBy(x => x.Status).ToList(),
                ("Status", "desc") => data.OrderByDescending(x => x.Status).ToList(),
                ("StartTime", "asc") => data.OrderBy(x => x.StartTime).ToList(),
                ("StartTime", "desc") => data.OrderByDescending(x => x.StartTime).ToList(),
                ("EndTime", "asc") => data.OrderBy(x => x.EndTime).ToList(),
                ("EndTime", "desc") => data.OrderByDescending(x => x.EndTime).ToList(),
                _ => data
            };

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Outlet Report");

            var headers = new[]
            {
                "RS Code", "Outlet Code", "SR Name", "SR Code", "Route Name",
                "Child Party", "Servicing PLG", "Status", "Start Time", "End Time",
                "Total Minutes", "Total Time Spent", "Row No"
            };

            for (int i = 0; i < headers.Length; i++)
                worksheet.Cell(1, i + 1).Value = headers[i];

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = item.RS_Code;
                worksheet.Cell(row, 2).Value = item.OutletCode;
                worksheet.Cell(row, 3).Value = item.OutletName;
                worksheet.Cell(row, 4).Value = item.MrName;
                worksheet.Cell(row, 5).Value = item.SrCode;
                worksheet.Cell(row, 6).Value = item.RouteName;
                worksheet.Cell(row, 7).Value = item.ChildParty;
                worksheet.Cell(row, 8).Value = item.ServicingPLG;
                worksheet.Cell(row, 9).Value = item.Status;
                worksheet.Cell(row, 10).Value = $"{item.StartDate} {item.StartTime}";
                worksheet.Cell(row, 11).Value = $"{item.EndDate} {item.EndTime}";
                worksheet.Cell(row, 12).Value = item.TotalTimeSpent;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OutletStatus_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
