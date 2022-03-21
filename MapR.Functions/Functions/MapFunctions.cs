using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using MapR.Data.Stores;

namespace MapR.Functions {
    public class MapFunctions {

        private readonly IStoreMaps _mapsStore;
        public MapFunctions(IStoreMaps mapsStore) {
            _mapsStore = mapsStore;
        }

        [FunctionName("GetMaps")]
        public async Task<IActionResult> RunGetMaps(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps")] HttpRequest req,
            string gameId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal) {
            var maps = await _mapsStore.GetMaps(gameId);

            return new OkObjectResult(maps);
        }

        [FunctionName("GetMap")]
        public async Task<IActionResult> RunGetMap(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}")] HttpRequest req,
        string gameId,
        string mapId,
        ILogger log) {

            var map = await _mapsStore.GetMap(gameId, mapId);

            return new OkObjectResult(map);
        }

        [FunctionName("GetActiveMap")]
        public async Task<IActionResult> RunGetActiveMap(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/activemap")] HttpRequest req,
            string gameId,
            ILogger log,
            ClaimsPrincipal claimsPrincipal) {
            var map = (await _mapsStore.GetActiveMap("", gameId));

            return new OkObjectResult(map);
        }
    }
}
