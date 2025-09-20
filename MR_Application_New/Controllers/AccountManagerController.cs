using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MR_Application_New.Controllers
{

    [Authorize(Roles = "Account Manager")]
    public class AccountManagerController : Controller
    {


        public IActionResult Dashboard()
        {
          
            return View();
        }



    }
}
