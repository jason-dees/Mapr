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
        public IndexController(SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager) {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]   
        [Route("")]
        [AllowAnonymous]
        public async Task<IActionResult> Index() {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            var userName = info != null ? _userManager.GetUserName(info.Principal) : "";
            var indexViewModel = new IndexViewModel {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync(),
                IsSignedIn = info != null,
                UserName = userName
            };
            return View("Index", indexViewModel);
        }

    }
}
