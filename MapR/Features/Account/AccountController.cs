using System.Security.Claims;
using System.Threading.Tasks;
using MapR.Features.Index;
using MapR.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.Account {
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller {

        [TempData]
        public string ErrorMessage { get; set; }

        readonly SignInManager<ApplicationUser> _signInManager;
        readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null) {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null) {
            if (remoteError != null) {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Index));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) {
                return RedirectToAction(nameof(Index));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.IsLockedOut) {
                return null;
            }
            if (result.Succeeded) {
                //var user = await _userManager.GetUserAsync(info.Principal);
                //var signInResult = await _userManager.AddLoginAsync(user, info);
                //await _signInManager.SignInAsync(user, isPersistent: false);
            }
            else { 
                // If the user does not have an account, then ask the user to create an account.
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nameIdentifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = new ApplicationUser {
                    UserName = email,
                    Email = email,
                    ProviderKey = info.ProviderKey,
                    LoginProvider = info.LoginProvider,
                    NameIdentifier = nameIdentifier
                };
                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded) {
                    var signInResult = await _userManager.AddLoginAsync(user, info);
                    if (signInResult.Succeeded) {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                    }
                }
            }
            return RedirectToLocal("/");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin() {
            return await ExternalLoginCallback();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToLocal("/");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(AccessDenied))]
        public IActionResult AccessDenied() {
            return View();
        }

        private void AddErrors(IdentityResult result) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl) {
            if (Url.IsLocalUrl(returnUrl)) {
                return Redirect(returnUrl);
            }
            else {
                return RedirectToAction(nameof(IndexViewModel), "Home");
            }
        }
    }
}
