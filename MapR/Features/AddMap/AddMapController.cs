using System;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Map;
using MapR.Stores.Game;
using MapR.Stores.Map;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.AddMap {
    [Route("game/{gameId}/[action]")]
    public class AddMapController : Controller{

        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        public AddMapController(IStoreGames gameStore,
            IStoreMaps mapStore) {

            _gameStore = gameStore;
            _mapStore = mapStore;
        }

        public async Task<IActionResult> AddMap(string gameId, [FromBody]Models.AddMap newMap) {
            var game = await _gameStore.GetGame(User.GetUserName(), gameId);

            if(game == null) {
                return NotFound("No game found with that id");
            }

            var map = new MapModel {
                GameId = gameId,
                Name = newMap.Name,
                ImageBytes = Convert.FromBase64String(newMap.ImageData)
            };

            await _mapStore.AddMap(map);

            return Ok();
        }
    }
}
