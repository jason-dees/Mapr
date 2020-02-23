using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MapR.Functions.Extensions;
using MapR.Functions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MapR.Functions {
	public static class GameFunctions
    {
        [FunctionName("GetUserGames")]
        public static async Task<IActionResult> RunGetUserGames(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games")] HttpRequest req,
        ClaimsPrincipal user,
        ILogger log) {

            if (!user.CheckIsSignedIn()) {
                return new RedirectResult("/api/login");
            }

            var gameStore = FunctionServices.GameStore;
            var games = await gameStore.GetGames(user.GetUserName());
            var result = new OkObjectResult(games);

            return result;
        }

        [FunctionName("GetGames")]
        public static async Task<IActionResult> RunGetGames(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{admin}/games")] HttpRequest req,
            string admin,
            ClaimsPrincipal user,
            ILogger log)
        {

            var gameStore = FunctionServices.GameStore;
            var games = await gameStore.GetGames(admin);

            return new OkObjectResult(games);
        }

        [FunctionName("GetGame")]
        public static async Task<IActionResult> RunGetGame(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}")] HttpRequest req,
                string gameId,
                ClaimsPrincipal user,
                ILogger log)
        {
            var gameStore = FunctionServices.GameStore;
            var mapStore = FunctionServices.MapStore;
            var markerStore = FunctionServices.MarkerStore;

            var game = new Game(await gameStore.GetGame(gameId));
            var maps = (await mapStore.GetMaps(gameId)).Select(async m => { 
                var map = new Map(m);
                var mapMarkers = await markerStore.GetMarkers(map.Id);
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
