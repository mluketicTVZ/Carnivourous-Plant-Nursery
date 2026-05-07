using Microsoft.AspNetCore.Mvc;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("admin")]
    public class AdminController : BaseController
    {
        private readonly IConfiguration _config;

        public AdminController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        [Route("")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string passkey)
        {
            var expected = _config["AdminPasskey"];
            if (!string.IsNullOrEmpty(expected) && passkey == expected)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid passkey.";
            return View("Login");
        }

        [HttpPost]
        [Route("logout")]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            return RedirectToAction("Index", "Home");
        }
    }
}
