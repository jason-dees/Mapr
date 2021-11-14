using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MapR.Functions.Extensions;
using System.Security.Claims;
using System.Linq;

namespace MapR.Functions
{
    public static class MapFunctions
    {
        [FunctionName("GetMaps")]
        public static async Task<IActionResult> RunGetMaps(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps")] HttpRequest req,
            string gameId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal) {
            var maps = await FunctionServices.MapStore.GetMaps(gameId);

            return new OkObjectResult(maps);
        }

        [FunctionName("GetMap")]
        public static async Task<IActionResult> RunGetMap(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}")] HttpRequest req,
        string gameId,
        string mapId,
        ILogger log) {

            var map = await FunctionServices.MapStore.GetMap(gameId, mapId);

            return new OkObjectResult(map);
        }

        [FunctionName("GetActiveMap")]
        public static async Task<IActionResult> RunGetActiveMap(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/activemap")] HttpRequest req,
            string gameId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal) {
            var map = (await FunctionServices.MapStore.GetActiveMap("", gameId));

            return new OkObjectResult(map);
        }
    }
}
