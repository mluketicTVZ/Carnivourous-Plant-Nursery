using Carnivorous_Plant_Nursery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Carnivorous_Plant_Nursery.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToLocal(returnUrl);

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }

            ModelState.AddModelError(string.Empty, ErrorMessage.InvalidLoginAttempt);
            return View(model);
        }

        [HttpGet]
        [Route("register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("register")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                PhoneNumber = model.PhoneNumber,
                DefaultShippingCity = model.DefaultShippingCity
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            await _userManager.AddToRoleAsync(user, AuthorizationRole.Customer);
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("manage")]
        [Authorize]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction(nameof(Login));

            return View(await BuildManageViewModel(user));
        }

        [HttpPost]
        [Route("manage")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Manage(AccountManageViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction(nameof(Login));

            model.Email = user.Email ?? string.Empty;
            model.HasLocalPassword = await _userManager.HasPasswordAsync(user);
            ValidatePasswordChange(model);

            if (!ModelState.IsValid)
                return View(model);

            user.DisplayName = model.DisplayName;
            user.PhoneNumber = model.PhoneNumber;
            user.DefaultShippingCity = model.DefaultShippingCity;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                AddIdentityErrors(updateResult.Errors, ErrorMessage.AccountUpdateFailed);
                return View(model);
            }

            if (ShouldChangePassword(model))
            {
                if (!model.HasLocalPassword)
                {
                    ModelState.AddModelError(string.Empty, ErrorMessage.LocalPasswordUnavailable);
                    return View(model);
                }

                var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);
                if (!passwordResult.Succeeded)
                {
                    AddIdentityErrors(passwordResult.Errors);
                    return View(model);
                }

                await _signInManager.RefreshSignInAsync(user);
                TempData["PasswordStatus"] = DisplayConstant.PasswordUpdated;
            }

            TempData["AccountStatus"] = DisplayConstant.AccountUpdated;
            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [Route("external-login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [Route("external-login-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return ExternalLoginFailure(returnUrl, ErrorMessage.ExternalLoginError);

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
                return ExternalLoginFailure(returnUrl, ErrorMessage.ExternalLoginEmailMissing);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = GetExternalDisplayName(info, email)
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return ExternalLoginFailure(returnUrl, createResult.Errors.Select(error => error.Description));

                await _userManager.AddToRoleAsync(user, AuthorizationRole.Customer);
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
                return ExternalLoginFailure(returnUrl, addLoginResult.Errors.Select(error => error.Description));

            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [Route("logout")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        private async Task<AccountManageViewModel> BuildManageViewModel(AppUser user)
        {
            return new AccountManageViewModel
            {
                Email = user.Email ?? string.Empty,
                DisplayName = user.DisplayName,
                PhoneNumber = user.PhoneNumber,
                DefaultShippingCity = user.DefaultShippingCity,
                HasLocalPassword = await _userManager.HasPasswordAsync(user)
            };
        }

        private void ValidatePasswordChange(AccountManageViewModel model)
        {
            if (ShouldChangePassword(model) && !model.HasLocalPassword)
                ModelState.AddModelError(string.Empty, ErrorMessage.LocalPasswordUnavailable);

            if (!string.IsNullOrWhiteSpace(model.NewPassword) && string.IsNullOrWhiteSpace(model.CurrentPassword))
                ModelState.AddModelError(nameof(model.CurrentPassword), ErrorMessage.CurrentPasswordRequired);

            if ((!string.IsNullOrWhiteSpace(model.CurrentPassword) || !string.IsNullOrWhiteSpace(model.ConfirmNewPassword)) &&
                string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError(nameof(model.NewPassword), ErrorMessage.NewPasswordRequired);
            }
        }

        private static bool ShouldChangePassword(AccountManageViewModel model)
        {
            return !string.IsNullOrWhiteSpace(model.CurrentPassword) ||
                   !string.IsNullOrWhiteSpace(model.NewPassword) ||
                   !string.IsNullOrWhiteSpace(model.ConfirmNewPassword);
        }

        private void AddIdentityErrors(IEnumerable<IdentityError> errors, string? fallback = null)
        {
            var hasErrors = false;
            foreach (var error in errors)
            {
                hasErrors = true;
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (!hasErrors && fallback != null)
                ModelState.AddModelError(string.Empty, fallback);
        }

        private IActionResult ExternalLoginFailure(string? returnUrl, string error)
        {
            return ExternalLoginFailure(returnUrl, new[] { error });
        }

        private IActionResult ExternalLoginFailure(string? returnUrl, IEnumerable<string> errors)
        {
            foreach (var error in errors)
                ModelState.AddModelError(string.Empty, error);

            return View("Login", new LoginViewModel { ReturnUrl = returnUrl });
        }

        private static string GetExternalDisplayName(ExternalLoginInfo info, string email)
        {
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrWhiteSpace(name))
                return name;

            return email.Split('@')[0];
        }
    }
}
