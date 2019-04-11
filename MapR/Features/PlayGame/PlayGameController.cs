using System;
using System.Linq;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Features.PlayGame.Models;
using MapR.Map;
using MapR.Stores.Game;
using MapR.Stores.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ImageMagick;

namespace MapR.Features.PlayGame {

    [Route("game/")]
    public class PlayGameController : Controller {

        readonly SignInManager<ApplicationUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        public PlayGameController(IStoreGames gameStore, 
            IStoreMaps mapStore,
            SignInManager<ApplicationUser> signInManager) {
            _gameStore = gameStore;
            _mapStore = mapStore;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("{gameId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Game(string gameId) {
            var game = await _gameStore.GetGame(gameId);

            if(game.Owner == User.GetUserName()) { return await GameAdmin(game); }

            var model = new GamePlayer {
                AuthenticationSchemes = await _signInManager.GetExternalAuthenticationSchemesAsync(),
                GameId = game.Id,
                GameName = game.Name,
                IsSignedIn = User.CheckIsSignedIn(),
                UserName = User.GetUserName()
            };

            return View("Game", model);
        }

        async Task<IActionResult> GameAdmin(GameModel game) {
            var maps = (await _mapStore.GetMaps(game.Id)).Select(m => new GameMap { 
                Id = m.Id,
                Name = m.Name,
                IsPrimary = m.IsPrimary,
                ImageBytes = m.ImageBytes.Resize(100,100)
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
