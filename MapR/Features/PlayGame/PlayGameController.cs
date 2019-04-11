using System;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Game;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.PlayGame {

    [Route("game/")]
    public class PlayGameController : Controller {

        readonly IStoreGames _gameStore;
        public PlayGameController(IStoreGames gameStore) {
            _gameStore = gameStore;
        }

        [HttpGet]
        [Route("{gameId}")]
        public async Task<IActionResult> Game(string gameId) {
            var game = await _gameStore.GetGame(gameId);

            if(game.Owner == User.GetUserName()) { return await GameAdmin(game); }

            return View();
        }

        async Task<IActionResult> GameAdmin(Game.Models.Game game) {
            return View("GameAdmin");
        }
    }
}
