using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MapR.Functions
{
    public static class MapFunctions
    {
        [FunctionName("GetMaps")]
        public static async Task<IActionResult> RunGetMaps(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}/maps")] HttpRequest req,
            string gameId,
            ILogger log) {

            var maps = await FunctionServices.MapStore.GetMaps(gameId);

            return new OkObjectResult(maps);
        }

        [FunctionName("GetMaps")]
        public static async Task<IActionResult> RunGetMap(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}/maps/{mapId}")] HttpRequest req,
        string gameId,
        string mapId,
        ILogger log) {

            var map = await FunctionServices.MapStore.GetMap(mapId);

            return new OkObjectResult(map);
        }
    }
}
