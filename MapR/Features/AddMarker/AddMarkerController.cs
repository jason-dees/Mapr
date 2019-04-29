using System;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MapR.Data.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MapR.Hubs;
using MapR.Extensions;
using System.IO;

namespace MapR.Features.AddMarker {
    [Route("games/{gameId}/maps/{mapId}/markers/[action]")]
    public class AddMarkerController : Controller {
        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;
        readonly IHubContext<MapHub> _hubContext;
        public AddMarkerController(IStoreGames gameStore,
            IStoreMaps mapStore,
            IStoreMarkers markerStore,
            IHubContext<MapHub> hubContext,
            SignInManager<MapRUser> signInManager) {
            _gameStore = gameStore;
            _mapStore = mapStore;
            _markerStore = markerStore;
            _hubContext = hubContext;
            _signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddMarker(string gameId, string mapId, [FromForm]Models.AddMarker newMarker) {

            var game = await _gameStore.GetGame(User.GetUserName(), gameId);
            if (game == null) {
                return NotFound("No game found with that id");
            }

            var map = await _mapStore.GetMap(mapId);
            if(map.GameId != game.Id) {
                return BadRequest("Map is not part of the game");
            }

            var marker = new MapR.Models.MarkerModel {
                MapId = mapId,
                GameId = gameId,
                Name = newMarker.Name,
                Description = newMarker.Description,
                CustomCss = newMarker.CustomCss,
                X = 0,
                Y = 0
            };
            using (Stream stream = newMarker.ImageData.OpenReadStream()) {
                marker.ImageBytes = new byte[(int)stream.Length];
                await stream.ReadAsync(marker.ImageBytes, 0, (int)stream.Length);
            }

            var createdMarker = await _markerStore.AddMarker(marker);

            await _hubContext.Clients.Group(gameId).SendAsync("SetMarker", new MapR.Models.MarkerModel{
                Id = createdMarker.Id,
                CustomCss = createdMarker.CustomCss,
                Description = createdMarker.Description,
                GameId = createdMarker.GameId,
                MapId = createdMarker.MapId,
                X = createdMarker.X,
                Y = createdMarker.Y,
                Name = createdMarker.Name,
                HasIcon = !string.IsNullOrEmpty(createdMarker.ImageUri)
            });

            return Ok();
        }
    }
}
