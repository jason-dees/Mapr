using MapR.Extensions;
using MapR.Game;
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
        readonly IStoreGames _gameStore;
        public IndexController(SignInManager<ApplicationUser> signInManager, 
            IStoreGames gameStore) {
            _signInManager = signInManager;
            _gameStore = gameStore;
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
