using Carnivorous_Plant_Nursery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("admin")]
    public class AdminController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [Route("")]
        [ValidateAntiForgeryToken]
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [Route("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
