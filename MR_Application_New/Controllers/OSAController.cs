using DAL.IRepositories;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Model_New.ViewModels;

namespace MR_Application_New.Controllers
{
    public class OSAController : Controller
    {
        private readonly IOSAServiceRepository _osaService;

        public OSAController(IOSAServiceRepository osaService)
        {
            _osaService = osaService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _osaService.GetFilterDataAsync();
            return View(model);
        }

  

        [HttpPost]
        public async Task<IActionResult> Filter(FilterViewModel filter)
        {
            var filterData = await _osaService.GetFilterDataAsync(); // Await the async method here
            filter.MrNames = filterData.MrNames; // No need for await here
            filter.OutletTypes = filterData.OutletTypes; // No need for await here
            filter.RsCodes = await _osaService.GetRsCodesByMrIdAsync(filter.SelectedMrId);

            filter.OsaResults = await _osaService.GetOSADataRawAsync(
                filter.SelectedMrId,
                filter.SelectedRsCode,
                filter.SelectedOutletType,
                filter.FromDate,
                filter.ToDate
            );

            return View("Index", filter);
        }


        public async Task<JsonResult> GetRsCodes(string mrId)
        {
            var rsCodes = await _osaService.GetRsCodesByMrIdAsync(mrId);
            return Json(rsCodes);
        }
    }

}
