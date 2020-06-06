using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MapR.Api.Controllers {

    [Route("games/{gameId}/maps")]
    public class MapController : Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;

        private const string _owner = "string";

        public MapController(IStoreGames gameStore, IStoreMaps mapStore) {
            _gameStore = gameStore;
            _mapStore = mapStore;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMaps(string gameId) {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddMap(string gameId, [FromBody] AddMap map) {

            var newMap = new MapModel {
                Name = map.Name,
                IsActive = map.IsActive,
                IsPrimary = map.IsPrimary
            };
            var newId = await _mapStore.AddMap(_owner, gameId, newMap, map.ImageBytes);
            if (newId != "") {
                return Created(newId, new { });
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("{mapId}")]
        public async Task<IActionResult> GetMap(string gameId, string mapId) {
            throw new NotImplementedException();
        }
    }
}

public class AddMap { 
    public byte[] ImageBytes { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
}
