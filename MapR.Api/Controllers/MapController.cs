using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using MapR.Api.Extensions;
using MapR.Api.Models;

namespace MapR.Api.Controllers {

    [Route("games/{gameId}/maps")]
    public class MapController : Controller {

        readonly IStoreMaps _mapStore;

        private string _owner => User.GetUserName();

        public MapController(IStoreMaps mapStore) {
            _mapStore = mapStore;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMaps(string gameId) {
            return new OkObjectResult(await _mapStore.GetMaps(_owner, gameId));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddMap(string gameId, [FromBody] AddMap map) {

            var newMap = new MapModel {
                Name = map.Name,
                IsActive = map.IsActive,
                IsPrimary = map.IsPrimary
            };
            var newId = await _mapStore.AddMap(_owner, gameId, newMap, new byte[0]);
            if (newId != "") {
                return Created(newId, new { });
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{mapId}")]
        public async Task<IActionResult> GetMap(string gameId, string mapId) {
            return new OkObjectResult(await _mapStore.GetMap(_owner, gameId, mapId));
        }

        [HttpPut]
        [Route("{mapId}")]
        public async Task<IActionResult> UpdateMap(string gameId, string mapId, [FromBody] UpdateMap updatedMap) {
            var map = new MapModel {
                Name = updatedMap.Name,
                IsActive = updatedMap.IsActive,
                IsPrimary = updatedMap.IsPrimary
            };
            await _mapStore.UpdateMap(_owner, gameId, mapId, map);

            return new OkResult();
        }

        [HttpPut]
        [Route("{mapId}/image")]
        public async Task<IActionResult> UpdateImage(string gameId, string mapId, [FromForm] IFormFile image) {
            using (Stream stream = image.OpenReadStream()) {
                var bytes = new byte[(int)stream.Length];
                await stream.ReadAsync(bytes, 0, (int)stream.Length);
                await _mapStore.ReplaceMapImage(_owner, gameId, mapId, bytes);
            }
            return Ok();
        }

        [HttpGet]
        [Route("{mapId}/image")]
        public async Task<IActionResult> GetImage(string gameId, string mapId, [FromQuery] int width, [FromQuery] int height) {
            var bytes = await _mapStore.GetMapImage(_owner, gameId, mapId); 
            if (width == 0 || height == 0) {
                return File(bytes, "image/jpeg");
            }
            return File(bytes.Resize(width, height), "image/jpeg");
        }

        [HttpDelete]
        [Route("{mapId}")]
        public async Task<IActionResult> DeleteMap(string gameId, string mapId) {
            await _mapStore.DeleteMap(_owner, gameId, mapId);
            return Ok();
        }
    }
}

public class AddMap { 
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateMap {
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
}
