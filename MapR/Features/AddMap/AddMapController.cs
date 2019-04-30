using System;
using System.IO;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Mvc;
using MapR.Models;

namespace MapR.Features.AddMap {
    [Route("games/{gameId}/maps/[action]")]
    public class AddMapController : Controller{

        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        public AddMapController(IStoreGames gameStore,
            IStoreMaps mapStore) {

            _gameStore = gameStore;
            _mapStore = mapStore;
        }

		[HttpPost]
        public async Task<IActionResult> AddMap(string gameId, [FromForm]Models.AddMap newMap) {
            var game = await _gameStore.GetGame(User.GetUserName(), gameId);

            if(game == null) {
                return NotFound("No game found with that id");
            }
            var map = new MapModel {
                GameId = gameId,
                Name = newMap.Name
            };
            using (Stream stream = newMap.ImageData.OpenReadStream()) {
                map.ImageBytes = new byte[(int)stream.Length];
                await stream.ReadAsync(map.ImageBytes, 0, (int)stream.Length);
            }
            await _mapStore.AddMap(map);

            return Ok();
        }
    }
}
