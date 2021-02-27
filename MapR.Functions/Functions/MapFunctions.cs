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

        [FunctionName("GetMapImage")]
        public static async Task<IActionResult> RunGetMapImage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}/image")] HttpRequest req,
            string gameId,
            string mapId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal)
        {
            var map = await FunctionServices.MapStore.GetMap(gameId, mapId);

            if (!int.TryParse(req.Query["width"], out int width) | !int.TryParse(req.Query["height"], out int height))
            {
                return new FileContentResult(map.ImageBytes, "image/jpeg");
            }

            return new FileContentResult(map.ImageBytes.ResizeImageBytes(width, height), "image/jpeg");
        }

        [FunctionName("GetActiveMap")]
        public static async Task<IActionResult> RunGetActiveMap(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/activemap")] HttpRequest req,
            string gameId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal) {
            var map = (await FunctionServices.MapStore.GetActiveMap(gameId));

            return new OkObjectResult(map);
        }

        [FunctionName("GetActiveMapImage")]
        public static async Task<IActionResult> RunGetActiveMapImage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/activemap/image")] HttpRequest req,
            string gameId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal) {

            var map = (await FunctionServices.MapStore.GetActiveMap(gameId));

            if (!int.TryParse(req.Query["width"], out int width) | !int.TryParse(req.Query["height"], out int height)) {
                return new FileContentResult(map.ImageBytes, "image/jpeg");
            }

            return new FileContentResult(map.ImageBytes.ResizeImageBytes(width, height), "image/jpeg");
        }
    }
}
