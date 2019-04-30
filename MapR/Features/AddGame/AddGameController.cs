using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Mvc;
using MapR.Models;

namespace MapR.Features.AddGame {

    [Route("games/[action]")]
    public class AddGameController : Controller {

        readonly IStoreGames _gameStore;
        public AddGameController(IStoreGames gameStore) {
            _gameStore = gameStore;
        }

        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] Models.AddGame game) {
            var newGame = new GameModel {
                Owner = User.GetUserName(),
                Name = game.Name,
                IsPrivate = game.IsPrivate
            };

            if(await _gameStore.AddGame(newGame)) {
                return Ok(new {
                    newGame.Id
                });
            }
            return BadRequest("There was an issue with creating your game. Blame the dev");
        }
    }
}
    