using MapR.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MapR.Features.Index {
    [Route("/")]
    public class IndexController : Controller {

        [TempData]
        public string ErrorMessage { get; set; }

        readonly SignInManager<ApplicationUser> _signInManager;
        readonly UserManager<ApplicationUser> _userManager;
        public IndexController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<IActionResult> Index() {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var indexViewModel = new IndexViewModel {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync(),
                IsSignedIn = info != null
            };
            return View("Index", indexViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route(nameof(ExternalLogin))]
        public IActionResult ExternalLogin(string provider, string returnUrl = null) {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(ExternalLoginCallback))]
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
            if (result.Succeeded) {
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut) {
                //return RedirectToAction(nameof(Lockout));
                return null;
            }
            else {
                var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
                // If the user does not have an account, then ask the user to create an account.
                //var user = new ApplicationUser { UserName = loginInfo., Email = model.Email };
                //var result = await _userManager.CreateAsync(user);
                //if (result.Succeeded) {
                //    result = await _userManager.AddLoginAsync(user, info);
                //    if (result.Succeeded) {
                //        await _signInManager.SignInAsync(user, isPersistent: false);
                //        return RedirectToLocal(returnUrl);
                //    }
                //}
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(ExternalLogin))]
        public async Task<IActionResult> GoogleSignIn(string state, string code, 
            string scope, int authUser, string session_state, string prompt) {

            return await ExternalLoginCallback();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(AccessDenied))]
        public IActionResult AccessDenied() {
            return View();
        }

        //      #region Helpers

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
