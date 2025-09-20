using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Model_New;
using Model_New.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MR_Application_New.Controllers
{
    public class CaptureDataController : Controller
    {
        private readonly IMRDataService _mrDataService;
        private readonly ILogger<CaptureDataController> _logger;

        public CaptureDataController(IMRDataService mrDataService, ILogger<CaptureDataController> logger)
        {
            _mrDataService = mrDataService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string srCode, string srCodeName, string mrId, string mrEmpNo, DateTime? fromDate, DateTime? toDate, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                PaginatedList<Tbl_capturedata_log> data;

                if (!string.IsNullOrEmpty(srCode) && !string.IsNullOrEmpty(srCodeName))
                {
                    data = await _mrDataService.GetMRDataBySrCodeAsync(srCode, srCodeName, fromDate, toDate, pageNumber, pageSize);
                }
                else if (!string.IsNullOrEmpty(mrId) && !string.IsNullOrEmpty(mrEmpNo))
                {
                    data = await _mrDataService.GetMRDataByMREmpAsync(mrId, mrEmpNo, fromDate, toDate, pageNumber, pageSize);
                }
                else
                {
                    data = new PaginatedList<Tbl_capturedata_log>(new List<Tbl_capturedata_log>(), 0, pageNumber, pageSize);
                }

                return View(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CaptureDataController.Index: {ex.Message}", ex);
                ViewData["ErrorMessage"] = "An error occurred while fetching the data. Please try again later.";
                return View(new PaginatedList<Tbl_capturedata_log>(new List<Tbl_capturedata_log>(), 0, pageNumber, pageSize));
            }
        }
    }
}
