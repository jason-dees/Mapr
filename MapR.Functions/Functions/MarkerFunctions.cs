using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using MapR.Data.Stores;

namespace MapR.Functions.Functions
{
    public class MarkerFunctions {

        private readonly IStoreMaps _mapsStore;
        private readonly IStoreMarkers _markersStore;
        public MarkerFunctions(IStoreMaps mapsStore, IStoreMarkers markersStore) {
            _mapsStore = mapsStore;
            _markersStore = markersStore;
        }

        [FunctionName("GetMarkers")]
        public async Task<IActionResult> RunGetMarkers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}/markers")] HttpRequest req,
            string gameId,
            string mapId,
            ILogger log) {

            var markers = await _markersStore.GetMarkers(mapId);

            return new OkObjectResult(markers);
        }

        [FunctionName("GetMarker")]
        public async Task<IActionResult> RunGetMarker(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/maps/{mapId}/markers/{markerId}")] HttpRequest req,
            string gameId,
            string mapId,
            string markerId,
            ILogger log) {

            var marker = await _markersStore.GetMarker(markerId);

            return new OkObjectResult(marker);
        }

        [FunctionName("GetActiveMapMarkers")]
        public async Task<IActionResult> RunGetActiveMapMarkers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "games/{gameId}/activemap/markers")] HttpRequest req,
            string gameId,
            ILogger log) {

            var map = (await _mapsStore.GetMaps(gameId)).FirstOrDefault(m => m.IsPrimary);
            var markers = await _markersStore.GetMarkers(map.Id);

            return new OkObjectResult(markers);
        }
    }
}
