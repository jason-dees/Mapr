using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using MapR.Data.Stores;
using MapR.Data.Models;
using MapR.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using System;
using MapR.Api.Models;

namespace MapR.Api.Controllers {
    [Route("games/")]
    public class GamesController: Controller {

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
            var newGame = new GameModel {
                Owner = _owner,
                Name = game.Name,   
                IsPrivate = game.IsPrivate,
                LastPlayed = DateTime.Now
            };

            var createdGame = await _gameStore.AddGame(newGame);
            if (createdGame != null) {
                return Ok(new {
                    createdGame.Id
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
        public async Task<IActionResult> GetGame(string gameId) {
            var game = await _gameStore.GetGame(_owner, gameId);

            return new OkObjectResult(game);
        }

        [HttpDelete]
        [Route("{gameId}")]
        public async Task<IActionResult> DeleteGame(string gameId) {
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