using MapR.Data.Models;
using MapR.Data.Stores;
using MapR.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.Features.Index {
    [Route("/")]
    public class IndexController : Controller {

        [TempData]
        public string ErrorMessage { get; set; }

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        public IndexController(SignInManager<MapRUser> signInManager, 
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
                UserName = userName,
                YourGames = (await _gameStore.GetGames(userName)).Select(gm => new Models.YourGame {
                    Id = gm.Id,
                    Name = gm.Name,
                    LastPlayed = gm.Timestamp
                }).OrderByDescending(g => g.LastPlayed).ToList()
            };
            return View("Index", indexViewModel);
        }

    }
}
