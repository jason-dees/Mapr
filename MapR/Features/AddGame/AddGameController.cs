using System;
using System.Threading.Tasks;
using MapR.Game;
using Microsoft.AspNetCore.Mvc;


namespace MapR.Features.AddGame {

    [Route("games/[action]")]
    public class AddGameController {

        readonly IStoreGames _gameStore;
        public AddGameController(IStoreGames gameStore) {
            _gameStore = gameStore;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGame(Models.AddGame game) {
            throw new NotImplementedException();
        }

    }
}
