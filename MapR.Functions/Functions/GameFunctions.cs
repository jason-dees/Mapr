using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace MapR.Functions
{
    public static class GameFunctions
    {
        [FunctionName("GetGames")]
        [Authorize]
        public static async Task<IActionResult> RunGetGames(
            [HttpTrigger(AuthorizationLevel.User, "get", Route = "{admin}/games")] HttpRequest req,
            string admin,
            ILogger log)
        {

            var gameStore = FunctionServices.GameStore;
            var games = await gameStore.GetGames(admin);

            return new OkObjectResult(games);
        }

        [FunctionName("GetGame")]
        public static async Task<IActionResult> RunGetGame(
                [HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}")] HttpRequest req,
                string gameId,
                ILogger log)
        {
            var gameStore = FunctionServices.GameStore;
            var game = await gameStore.GetGame(gameId);

            return new OkObjectResult(game);
        }
    }
}
