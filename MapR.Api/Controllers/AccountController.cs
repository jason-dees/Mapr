using System.Security.Claims;
using System.Threading.Tasks;
using MapR.Api.Extensions;
using MapR.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Api.Controllers {
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller {

        [TempData]
        public string ErrorMessage { get; set; }

        readonly SignInManager<MapRUser> _signInManager;
        readonly UserManager<MapRUser> _userManager;

        public AccountController(SignInManager<MapRUser> signInManager,
            UserManager<MapRUser> userManager) {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null) {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null) {
            if (remoteError != null) {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return BadRequest(ErrorMessage);
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) {
                return NotFound("No external login info found");
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);
            if (result.IsLockedOut) {
                return null;
            }
            if (result.Succeeded) {
            }
            else { 
                // If the user does not have an account, then ask the user to create an account.
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var nameIdentifier = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = new MapRUser {
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
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin() {
            return await ExternalLoginCallback();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GoogleLogin([FromQuery] string redirect = "") {
            redirect = string.IsNullOrEmpty(redirect) ? "/games" : redirect;
            if (!string.IsNullOrEmpty(User.GetUserName())) {
                return new RedirectResult(redirect);

            }
            var provider = "google";
            var properties = new AuthenticationProperties() {
                // actual redirect endpoint for your app
                RedirectUri = redirect,
                Items =
                {
                    { "LoginProvider", provider },
                },
                AllowRefresh = true,
            };
            return Challenge(properties, "Google");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return Ok();
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
    }
}
