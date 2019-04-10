using MapR.Extensions;
using MapR.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index() {
            var isSignedIn = User.CheckIsSignedIn();

            var userName = User.GetUserName();

            var indexViewModel = new IndexViewModel {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync(),
                IsSignedIn = isSignedIn,
                UserName = userName
            };
            return View("Index", indexViewModel);
        }

    }
}
