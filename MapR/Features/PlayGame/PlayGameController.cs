using System;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Features.PlayGame.Models;
using MapR.Game;
using MapR.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.PlayGame {

    [Route("game/")]
    public class PlayGameController : Controller {

        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IStoreGames _gameStore;
        public PlayGameController(IStoreGames gameStore, SignInManager<ApplicationUser> signInManager) {
            _gameStore = gameStore;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("{gameId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Game(string gameId) {
            var game = await _gameStore.GetGame(gameId);

            if(game.Owner == User.GetUserName()) { return await GameAdmin(game); }

            var model = new GamePlayer {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync(),
                GameId = game.Id,
                GameName = game.Name,
                IsSignedIn = User.CheckIsSignedIn(),
                UserName = User.GetUserName()
            };

            return View("Game", model);
        }

        async Task<IActionResult> GameAdmin(Game.Models.Game game) {
            var model = new GameAdmin {
                UserName = User.GetUserName(),
                GameId = game.Id,
                GameName = game.Name
            };
            return View("GameAdmin", model);
        }
    }
}
