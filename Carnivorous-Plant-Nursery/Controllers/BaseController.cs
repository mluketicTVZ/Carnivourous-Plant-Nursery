using Microsoft.AspNetCore.Mvc;

namespace Carnivorous_Plant_Nursery.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool IsAdmin => User.IsInRole("Admin");

        protected IActionResult RequireAdmin() =>
            Challenge();
    }
}
