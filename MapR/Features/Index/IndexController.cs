using MapR.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MapR.Features.Index {
    [Route("/")]
    public class IndexController : Controller {

        readonly SignInManager<ApplicationUser> _signInManager;
        public IndexController(SignInManager<ApplicationUser> signInManager){
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index() {
            var indexViewModel = new IndexViewModel {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
            };
            return View("Index", indexViewModel);
        }
    }
}
