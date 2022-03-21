using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MapR.Data.Stores;
using MapR.Functions.Extensions;
using MapR.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MapR.Functions {
	public class GameFunctions {
        private readonly IStoreGames _gamesStore;
        private readonly IStoreMaps _mapsStore;
        private readonly IStoreMarkers _markersStore;
        public GameFunctions(IStoreGames gamesStore, IStoreMaps mapsStore, IStoreMarkers markersStore) {
            _gamesStore = gamesStore;
            _mapsStore = mapsStore;
            _markersStore = markersStore;
        }

        [FunctionName("GetUserGames")]
        public async Task<IActionResult> RunGetUserGames(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games")] HttpRequest req,
        ClaimsPrincipal user,
        ILogger log) {

            if (!user.CheckIsSignedIn()) {
                return new RedirectResult("/api/login");
            }

            var games = await _gamesStore.GetGames(user.GetUserName());
            var result = new OkObjectResult(games);

            return result;
        }

        [FunctionName("GetGames")]
        public async Task<IActionResult> RunGetGames(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{admin}/games")] HttpRequest req,
            string admin,
            ClaimsPrincipal user,
            ILogger log) {

            var games = await _gamesStore.GetGames(admin);

            return new OkObjectResult(games);
        }

        [FunctionName("GetGame")]
        public async Task<IActionResult> RunGetGame(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}")] HttpRequest req,
                string gameId,
                ClaimsPrincipal user,
                ILogger log) {

            var game = new Game(await _gamesStore.GetGame(gameId));
            var maps = (await _mapsStore.GetMaps(gameId)).Select(async m => { 
                var map = new Map(m);
                var mapMarkers = await _markersStore.GetMarkers(map.Id);
                map.ImageUri = $"{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/games/{map.GameId}/maps/{map.Id}/image";
                map.Markers = mapMarkers.Select(mm => { 
                    var marker = new Marker(mm);
                    marker.ImageUri = $"{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/games/{map.GameId}/maps/{map.Id}/markers/{marker.Id}/image";
                    return marker;
                    });
                return map;
            });

            game.Maps = await Task.WhenAll(maps.ToArray());

            return new OkObjectResult(game);
        }
    }
}
