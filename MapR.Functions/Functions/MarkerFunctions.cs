using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MapR.Functions.Extensions;
using System.Linq;

namespace MapR.Functions.Functions
{
    public static class MarkerFunctions
    {
        [FunctionName("GetMarkers")]
        public static async Task<IActionResult> RunGetMarkers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}/markers")] HttpRequest req,
            string gameId,
            string mapId,
            ILogger log)
        {

            var markers = await FunctionServices.MarkerStore.GetMarkers(mapId);

            return new OkObjectResult(markers);
        }

        [FunctionName("GetMarker")]
        public static async Task<IActionResult> RunGetMarker(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}/markers/{markerId}")] HttpRequest req,
            string gameId,
            string mapId,
            string markerId,
            ILogger log)
        {

            var marker = await FunctionServices.MarkerStore.GetMarker(markerId);

            return new OkObjectResult(marker);
        }

        [FunctionName("GetActiveMapMarkers")]
        public static async Task<IActionResult> RunGetActiveMapMarkers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/activemap/markers")] HttpRequest req,
            string gameId,
            ILogger log) {

            var map = (await FunctionServices.MapStore.GetMaps(gameId)).FirstOrDefault(m => m.IsPrimary);
            var markers = await FunctionServices.MarkerStore.GetMarkers(map.Id);

            return new OkObjectResult(markers);
        }
    }
}
