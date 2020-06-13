using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using MapR.Data.Stores;
using MapR.Data.Models;
using Microsoft.Azure.Cosmos;
using MapR.Api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace MapR.Api.Controllers {
    [Route("games/")]
    public class GamesController: Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;

        private string _owner => User.GetUserName();

        public GamesController(IStoreGames gameStore) {
            _gameStore = gameStore;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetGames() =>
            new OkObjectResult(await _gameStore.GetGames(_owner));

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddGame([FromBody] AddGame game) {
            var newGame = new Models.GameModel {
                //Owner = User.GetUserName(),
                Owner = _owner,
                Name = game.Name,
                IsPrivate = game.IsPrivate
            };

            if (await _gameStore.AddGame(newGame)) {
                return Ok(new {
                    newGame.Id
                });
            }
            return BadRequest("There was an issue with creating your game. Blame the dev");
        }

        [HttpPut]
        [Route("{gameId}")]
        public async Task<IActionResult> UpdateMap(string gameId, [FromBody] EditGame game) {
            var currentGame = await _gameStore.GetGame(_owner, gameId);
            currentGame.IsPrivate = game.IsPrivate;
            currentGame.Name = game.Name;
            await _gameStore.UpdateGame(_owner, gameId, currentGame);
            return Ok("Game updated");
        }

        [HttpGet]
        [Route("{gameId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMap(string gameId) {
            var game = await _gameStore.GetGame(_owner, gameId);

            return new OkObjectResult(game);
        }

        [HttpDelete]
        [Route("{gameId}")]
        public async Task<IActionResult> DeleteMap(string gameId) {
            return NotFound();
        }
    }
}

public class AddGame {
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
}

public class EditGame {
    public string Name { get; set; }
    public bool IsPrivate { get; set; }
}