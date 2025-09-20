using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MR_Application_New.Controllers
{
    [Authorize(Roles = "FieldManager")]
    public class FieldManagerController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
         
        }
    }
}
