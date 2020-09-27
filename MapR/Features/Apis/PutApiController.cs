using System;
using System.Threading.Tasks;
using MapR.Data.Models;
using MapR.Data.Stores;
using MapR.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.Apis {
    [Route("api/game/")]
    public class PutApiController : Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;

        public PutApiController(IStoreGames gameStore,
            IStoreMaps mapStore,
            IStoreMarkers markerStore,
            SignInManager<MapRUser> signInManager) {

            _gameStore = gameStore;
            _mapStore = mapStore;
            _markerStore = markerStore;
            _signInManager = signInManager;
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> CreateGame([FromBody] IAmAGameModel newGame) {
            newGame.Owner = User.GetUserName();
            if (await _gameStore.AddGame(newGame)) {

                return Created($"api/game/{newGame.Id}", newGame);
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("{gameId}")]
        public IActionResult UpdateGame(string gameId, [FromBody] IAmAGameModel game) {
            return BadRequest("Can't edit games at this time");
        }

        [HttpPut]
        [Route("{gameId}/maps")]
        public async Task<IActionResult> CreateGameMap(string gameId, [FromBody] IAmAMapModel newMap) {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("{gameId}/maps/{mapId}")]
        public async Task<IActionResult> UpdateGameMap(string gameId, string mapId, [FromBody] IAmAMapModel map) {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("{gameId}/maps/{mapId}/markers")]
        public async Task<IActionResult> CreateMapMarker(string gameId, string mapId, [FromBody] IAmAMarkerModel marker) {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("{gameId}/maps/{mapId}/markers/{markerId}")]
        public async Task<IActionResult> UpdateMapMarker(string gameId, string mapId, string markerId, [FromBody] IAmAMarkerModel marker) {
            throw new NotImplementedException();
        }

    }
}
