using System;
using System.Threading.Tasks;
using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MapR.Features.Apis {
    [Route("api/game/")]
    public class PostApiController : Controller {

        readonly SignInManager<MapRUser> _signInManager;
        readonly IStoreGames _gameStore;
        readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;

        public PostApiController(IStoreGames gameStore,
            IStoreMaps mapStore,
            IStoreMarkers markerStore,
            SignInManager<MapRUser> signInManager) {
            _gameStore = gameStore;
            _mapStore = mapStore;
            _markerStore = markerStore;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateGame() {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{gameId}/maps")]
        public async Task<IActionResult> CreateGameMap(string gameId) {
            throw new NotImplementedException();
        }
    }
}
