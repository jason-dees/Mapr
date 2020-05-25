using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using MapR.Data.Stores;
using MapR.Data.Models;
using MapR.Api.Extensions;

namespace MapR.Api.Controllers {
    [Route("game/")]
    public class GameController: Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;

        private const string Owner = "string";

        public GameController(IStoreGames gameStore) {
            _gameStore = gameStore;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetGames() =>
            new OkObjectResult(await _gameStore.GetGames(Owner));

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddGame([FromBody] AddGame game) {
            var newGame = new Models.GameModel {
                //Owner = User.GetUserName(),
                Owner = Owner,
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
        public async Task<IActionResult> UpdateMap(string gameId) {
            return NotFound();
        }

        [HttpGet]
        [Route("{gameId}")]
        public async Task<IActionResult> GetMap(string gameId) {
            var game = await _gameStore.GetGame(Owner, gameId);

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
