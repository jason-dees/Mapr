using System;
using System.Linq;
using System.Threading.Tasks;
using MapR.Data.Models;
using MapR.Data.Stores;
using MapR.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.Apis {
    [Route("api/game/")]
    public class GetApiController : Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;

        public GetApiController(IStoreGames gameStore,
            IStoreMaps mapStore,
            IStoreMarkers markerStore,
            SignInManager<MapRUser> signInManager) {
            _gameStore = gameStore;
            _mapStore = mapStore;
            _markerStore = markerStore;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetGames() {
            var games = await _gameStore.GetGames(User.GetUserName());

            if (!games.Any()) return NotFound();

            return Ok(games.Select(_ => (Models.GameModel)_));
        }

        [HttpGet]
        [Route("{gameId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGame(string gameId) {
            var game = await _gameStore.GetGame(gameId);

            if (game.IsPrivate) return NotFound();

            return Ok((Models.GameModel)game);
        }

        [HttpGet]
        [Route("{gameId}/maps")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGameMaps(string gameId) {
            var game = await _gameStore.GetGame(gameId);

            if (game.IsPrivate) return NotFound();

            var maps = await _mapStore.GetMaps(gameId);
            if (!maps.Any()) return NotFound();

            return Ok(maps.Select(_ => (Models.MapModel)_));
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGameMap(string gameId, string mapId) {
            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) return NotFound();

            var map = await _mapStore.GetMap(mapId);

            return Ok((Models.MapModel)map);
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}/image")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapImage(string gameId, string mapId,
            [FromQuery]int width, [FromQuery]int height) {

            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) return NotFound();

            var map = await _mapStore.GetMap(mapId);
            if (width == 0 || height == 0) {
                return File(map.ImageBytes, "image/jpeg");
            }
            return File(map.ImageBytes.Resize(width, height), "image/jpeg");
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}/markers")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGameMapMarkers(string gameId, string mapId) {
            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) return NotFound();

            var markers = await _markerStore.GetMarkers(mapId);
            if (!markers.Any()) return NotFound();

            return Ok(markers.Select(_ => (Models.MarkerModel)_));
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}/markers/{markerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGameMapMarker(string gameId, string mapId, string markerId) {
            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) return NotFound();

            var marker = await _markerStore.GetMarker(markerId);

            return Ok((Models.MarkerModel)marker);
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}/markers/{markerId}/image")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGameMapMarkerImage(string gameId, string mapId, string markerId,
            [FromQuery]int width, [FromQuery]int height) {

            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) return NotFound();

            var marker = await _markerStore.GetMarker(markerId);
            if (width == 0 && height == 0) {
                return File(marker.ImageBytes, "image/jpeg");
            }
            return File(marker.ImageBytes.Resize(width, height), "image/jpeg");
        }
    }
}
