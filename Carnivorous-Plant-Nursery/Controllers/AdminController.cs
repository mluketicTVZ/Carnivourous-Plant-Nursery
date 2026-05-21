using Microsoft.AspNetCore.Mvc;
using OtpNet;

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
        public IActionResult Login(string passkey, string totpCode)
        {
            var passkeyHash = _config["AdminPasskeyHash"];
            var totpSecret = _config["AdminTotpSecret"];

            bool passkeyValid = !string.IsNullOrEmpty(passkeyHash)
                && BCrypt.Net.BCrypt.Verify(passkey ?? string.Empty, passkeyHash);

            bool totpValid = false;
            if (!string.IsNullOrEmpty(totpSecret) && !string.IsNullOrEmpty(totpCode))
            {
                var key = Base32Encoding.ToBytes(totpSecret);
                var totp = new Totp(key);
                totpValid = totp.VerifyTotp(totpCode.Trim(), out _, VerificationWindow.RfcSpecifiedNetworkDelay);
            }

            if (passkeyValid && totpValid)
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials.";
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
