﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MapR.Features.Index;
using MapR.Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Identity {
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController( UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
       	public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null) {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //		[HttpPost]
        //		[AllowAnonymous]
        //		[ValidateAntiForgeryToken]
        //		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) {
        //			ViewData["ReturnUrl"] = returnUrl;
        //			if (ModelState.IsValid) {
        //				// This doesn't count login failures towards account lockout
        //				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
        //				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
        //				if (result.Succeeded) {
        //					return RedirectToLocal(returnUrl);
        //				}
        //				if (result.RequiresTwoFactor) {
        //					return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
        //				}
        //				if (result.IsLockedOut) {
        //					return RedirectToAction(nameof(Lockout));
        //				}
        //				else {
        //					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //					return View(model);
        //				}
        //			}

        //			// If we got this far, something failed, redisplay form
        //			return View(model);
        //		}

        //		[HttpGet]
        //		[AllowAnonymous]
        //		public IActionResult Lockout() {
        //			return View();
        //		}

        //		[HttpGet]
        //		[AllowAnonymous]
        //		public IActionResult Register(string returnUrl = null) {
        //			ViewData["ReturnUrl"] = returnUrl;
        //			return View();
        //		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(IndexViewModel), "Home");
        }

        [HttpPost]
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
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) {
                return RedirectToAction(nameof(Login));
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
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null) {
            if (ModelState.IsValid) {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null) {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded) {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded) {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(IndexViewModel), model);
        }

        [HttpGet]
        public IActionResult AccessDenied() {
            return View();
        }

        //		#region Helpers

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

        //		#endregion
    }
}
