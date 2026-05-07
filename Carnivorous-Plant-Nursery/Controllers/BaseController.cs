using Microsoft.AspNetCore.Mvc;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

        protected IActionResult RequireAdmin() =>
            RedirectToAction("Index", "Admin");
    }
}
