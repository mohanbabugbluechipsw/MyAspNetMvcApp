using BLL.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Model_New.Models;
using Model_New.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MR_Application_New.Controllers
{
    public class OutletMRSearchController : Controller
    {
        private readonly IOutletMRSearchService _service;
        private readonly MrAppDbNewContext _context;

        public OutletMRSearchController(IOutletMRSearchService service, MrAppDbNewContext context)
        {
            _service = service;
            _context = context;
        }

      

        private async Task LoadUsersToViewBag()
        {
            var rsList = await _context.tbl_Distributor_Regions
                .Select(u => new
                {
                    RS_Code = u.RS_Code.ToString(),
                    RS_Name = u.RS_Name
                })
                .Distinct()
                .ToListAsync();

            ViewBag.RSCodes = rsList.Select(u => new SelectListItem
            {
                Value = u.RS_Code,
                Text = u.RS_Code
            }).ToList();

            // Create dictionary for JS lookup
            ViewBag.RSCodeNameMap = System.Text.Json.JsonSerializer.Serialize(
                rsList.ToDictionary(x => x.RS_Code, x => x.RS_Name)
            );
        }

        [HttpGet]
        public async Task<IActionResult> Index(string rsCode, string rsName, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10, string sortColumn = "RS_Code", string sortOrder = "asc")
        {
            await LoadUsersToViewBag();

            // Preserve user input
            ViewBag.SelectedRSCode = rsCode;
            ViewBag.SelectedRSName = rsName;
            ViewBag.SelectedFromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.PageSize = pageSize;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            if (string.IsNullOrEmpty(rsCode) || fromDate == null || toDate == null)
            {
                ViewBag.TotalPages = 0;
                ViewBag.CurrentPage = 1;
                return View(new List<RSOutletSearchModel>());
            }

            var data = await _service.GetOutletSummaryAsync(rsCode, rsName, fromDate.Value, toDate.Value);

            // Sorting logic
            data = sortColumn switch
            {
                "RS_Code" => sortOrder == "asc" ? data.OrderBy(x => x.RS_Code).ToList() : data.OrderByDescending(x => x.RS_Code).ToList(),
                "OutletCode" => sortOrder == "asc" ? data.OrderBy(x => x.OutletCode).ToList() : data.OrderByDescending(x => x.OutletCode).ToList(),
                "Status" => sortOrder == "asc" ? data.OrderBy(x => x.Status).ToList() : data.OrderByDescending(x => x.Status).ToList(),
                "StartTime" => sortOrder == "asc" ? data.OrderBy(x => x.StartTime).ToList() : data.OrderByDescending(x => x.StartTime).ToList(),
                "EndTime" => sortOrder == "asc" ? data.OrderBy(x => x.EndTime).ToList() : data.OrderByDescending(x => x.EndTime).ToList(),
                "Date" => sortOrder == "asc" ? data.OrderBy(x => x.Date).ToList() : data.OrderByDescending(x => x.Date).ToList(),
                _ => data
            };

            // Pagination
            var pagedData = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedData);
        }
    }
}
