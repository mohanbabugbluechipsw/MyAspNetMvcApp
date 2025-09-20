using BLL.Services;
using DAL.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model_New.ViewModels;
using Rotativa;
using Rotativa.AspNetCore.Options; // Ensure the correct namespace is used



namespace MR_Application_New.Controllers
{
    // Controllers/OSAReviewController.cs
    public class OSAReviewController : Controller
    {
        private readonly IOSAReviewService _service;

        public OSAReviewController(IOSAReviewService service)
        {
            _service = service;
        }











        public async Task<IActionResult> Index(OSAReviewFilterModel filter)
        {

            var employees = await _service.GetMRNames();
            Console.WriteLine($"Found {employees.Count} employees"); // Check count

            ViewBag.MrNames = new SelectList(employees, "EmpNo", "EmpName");



            ViewBag.RSCodes = !string.IsNullOrEmpty(filter.MrName)
       ? await _service.GetRSCodesByMRName(filter.MrName)
       : new List<string>();

            ViewBag.RSCodes = await _service.GetRSCodesByMRName(filter.MrName);


            ViewBag.OutletTypes = await _service.GetOutletTypes();




            var result = await _service.GetOSAReviewData(filter);
            return View(result);
        }








        [HttpGet]
        public async Task<IActionResult> GetRSCodes(string mrName)
        {
            var rsCodes = await _service.GetRSCodesByMRName(mrName);
            return Json(rsCodes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OSAReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateOSAReview(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(OSAReviewViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateOSAReview(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteOSAReview(id);
            return RedirectToAction("Index");
        }
    }
}
