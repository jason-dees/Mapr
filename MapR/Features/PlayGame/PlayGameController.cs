using System;
using System.Linq;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Features.PlayGame.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ImageMagick;
using MapR.Data.Stores;
using MapR.Data.Models;

namespace MapR.Features.PlayGame {

    [Route("game/")]
    public class PlayGameController : Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;
        public PlayGameController(IStoreGames gameStore, 
            IStoreMaps mapStore,
            IStoreMarkers markerStore,
            SignInManager<MapRUser> signInManager) {
            _gameStore = gameStore;
            _mapStore = mapStore;
            _markerStore = markerStore;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(string gameId, string mapId, 
            [FromQuery]int width, [FromQuery]int height) {

            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) { return NotFound(); }

            var map = await _mapStore.GetMap(mapId);
            if(width == 0 || height == 0) {
                return File(map.ImageBytes, "image/jpeg");
            }
            return File(map.ImageBytes.Resize(width, height), "image/jpeg");
        }

        [HttpGet]
        [Route("{gameId}/maps/{mapId}/markers/{markerId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMarkerImage(string gameId, string mapId, string markerId,
            [FromQuery]int width, [FromQuery]int height) {

            var game = await _gameStore.GetGame(gameId);
            if (game.IsPrivate) { return NotFound(); }

            var marker = await _markerStore.GetMarker(markerId);
            if (width == 0 && height == 0) {
                return File(marker.ImageBytes, "image/jpeg");
            }
            return File(marker.ImageBytes.Resize(width, height), "image/jpeg");
        }

        [HttpGet]
        [Route("{gameId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Game(string gameId) {
            var game = await _gameStore.GetGame(gameId);

            if(game.Owner == User.GetUserName()) { return await GameAdmin(game); }
            var primaryMap = (await _mapStore.GetMaps(game.Id)).FirstOrDefault(m => m.IsPrimary);

            var model = new GamePlayer {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync(),
                GameId = game.Id,
                GameName = game.Name,
                IsSignedIn = User.CheckIsSignedIn(),
                UserName = User.GetUserName(),
                PrimaryMapId = primaryMap.Id,
                PrimaryMapName = primaryMap.Name
            };

            return View("GamePlayer", model);
        }

        async Task<IActionResult> GameAdmin(GameModel game) {
            var maps = (await _mapStore.GetMaps(game.Id)).Select(m => new GameMap { 
                Id = m.Id,
                Name = m.Name,
                IsPrimary = m.IsPrimary
            }).ToList();

            var model = new GameAdmin {
                UserName = User.GetUserName(),
                GameId = game.Id,
                GameName = game.Name,
                Maps = maps
            };
            return View("GameAdmin", model);
        }
    }

    internal static class Extensions {

        internal static byte[] Resize(this byte[] imageBytes, int width, int height) {
            using (var image = new MagickImage(imageBytes)) {
                width = width == 0 ? image.Width : width;
                height = height == 0 ? image.Height : height;

                var ratio = (double)image.Width / image.Height;
                var scalePercentage = new Percentage(100);

                scalePercentage = width / height > ratio
                    ? new Percentage((double)height / image.Height * 100)
                    : new Percentage((double)width / image.Width * 100);

                image.Resize(scalePercentage);
                return image.ToByteArray();
            }
        }
    }
}
