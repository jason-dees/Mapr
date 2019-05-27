using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MapR.Functions.Extensions;

namespace MapR.Functions.Functions
{
    public static class MarkerFunctions
    {
        [FunctionName("GetMarkers")]
        public static async Task<IActionResult> RunGetMarkers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}/maps/{mapId}/markers")] HttpRequest req,
            string gameId,
            string mapId,
            ILogger log)
        {

            var markers = await FunctionServices.MarkerStore.GetMarkers(mapId);

            return new OkObjectResult(markers);
        }

        [FunctionName("GetMarker")]
        public static async Task<IActionResult> RunGetMarker(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}/maps/{mapId}/markers/{markerId}")] HttpRequest req,
            string gameId,
            string mapId,
            string markerId,
            ILogger log)
        {

            var marker = await FunctionServices.MarkerStore.GetMarker(markerId);

            return new OkObjectResult(marker);
        }

        [FunctionName("GetMarkerImage")]
        public static async Task<IActionResult> RunGetMarkerImage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "games/{gameId}/maps/{mapId}/markers/{markerId}/image")] HttpRequest req,
            string gameId,
            string mapId,
            string markerId,
            ILogger log)
        {
            var marker = await FunctionServices.MarkerStore.GetMarker(markerId);

            if (!int.TryParse(req.Query["width"], out int width) | !int.TryParse(req.Query["height"], out int height))
            {
                return new FileContentResult(marker.ImageBytes, "image/jpeg");
            }

            return new FileContentResult(marker.ImageBytes.ResizeImageBytes(width, height), "image/jpeg");
        }
    }
}
