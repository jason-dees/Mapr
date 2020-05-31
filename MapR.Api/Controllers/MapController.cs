using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<IActionResult> GetMaps(string gameId) {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> AddMap(string gameId, AddMap map) {

            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{mapId}")]
        public async Task<IActionResult> GetMap(string gameId, string mapId) {
            throw new NotImplementedException();
        }
    }
}

public class AddMap { 
    public string MapId { get; set; }
    public string ImageUri { get; set; }
    public string Name { get; set; }
    public string IsActive { get; set; }
    public string IsPrimary { get; set; }
}
