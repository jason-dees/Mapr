
using MapR.Api.Extensions;
using MapR.Api.Models;
using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.Api.Hubs {

	public class MapHub : Hub {

		readonly IStoreGames _gameStore;
		readonly IStoreMaps _mapStore;
        readonly IStoreMarkers _markerStore;

        const string UPDATE_MAP_MARKERS = "UpdateMapMarkers";
        const string UPDATE_MARKER = "UpdateMarker";
        const string UPDATE_MAP = "UpdateMap";

        public MapHub(
			IStoreGames gameStore,
			IStoreMaps mapStore,
            IStoreMarkers markerStore) {
			_mapStore = mapStore;
			_gameStore = gameStore;
            _markerStore = markerStore;
        }

        public async Task AddClientToGame(string gameId) {
            //everything is public now. Information wants to be free
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);

            await Clients.Caller.SendAsync("UpdateMap", primaryMap.Id);
            var mapMarkers = await GetMapMarkers(gameId, primaryMap.Id);

            await Clients.Caller.SendAsync("UpdateMapMarkers", mapMarkers);
        }

        public async Task RemoveFromGame(string gameId) {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task SendUpdateClientsMarkers(string gameId) {
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
            var mapMarkers = await GetMapMarkers(gameId, primaryMap.Id);
            await Clients.Groups(gameId).SendAsync(UPDATE_MAP_MARKERS, mapMarkers);
        }

        public async Task SendUpdateClientMarkers(string gameId) {
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
            var mapMarkers = await GetMapMarkers(gameId, primaryMap.Id);
            await Clients.Caller.SendAsync(UPDATE_MAP_MARKERS, mapMarkers);
        }

        public async Task SendUpdateClientsMap(string gameId) {
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
            await Clients.Groups(gameId).SendAsync(UPDATE_MAP, primaryMap);
        }

        public async Task SendUpdateClientMap(string gameId) {
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
            await Clients.Caller.SendAsync(UPDATE_MAP, primaryMap);
        }
        
        public async Task SendUpdateClientsMarker(string gameId, string markerId) {
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
            var mapMarkers = await GetMapMarkers(gameId, primaryMap.Id);
            var marker = mapMarkers.FirstOrDefault(_ => _.Id == markerId);
            await Clients.Groups(gameId).SendAsync(UPDATE_MARKER, marker);
        }

        public async Task SendUpdateClientMarker(string gameId, string markerId) {
            var maps = await _mapStore.GetMaps(gameId);
            var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
            var mapMarkers = await GetMapMarkers(gameId, primaryMap.Id);
            var marker = mapMarkers.FirstOrDefault(_ => _.Id == markerId);
            await Clients.Caller.SendAsync(UPDATE_MARKER, marker);
        }

        //	async Task<bool> CheckGameAndMapOwnership(string gameId, string mapId) {

        //		var game = await _gameStore.GetGame(gameId);
        //		if (game.Owner != Context.User.GetUserName()) { return false; }

        //		var map = await _mapStore.GetMap(gameId, mapId);
        //		if (map.GameId != game.Id) { return false; }

        //		return true;
        //	}

        IAmAMarkerModel MapToModel(Data.Models.IAmAMarkerModel marker) {
            return new MarkerModel {
                Id = marker.Id,
                MapId = marker.MapId,
                GameId = marker.GameId,
                Name = marker.Name,
                Description = marker.Description,
                CustomCss = marker.CustomCss,
                X = marker.X,
                Y = marker.Y
            };
        }

        async Task<IEnumerable<IAmAMarkerModel>> GetMapMarkers(string gameId, string mapId) =>
            (await _markerStore.GetMarkers(mapId)).Select(MapToModel);
        }

    public class MapMarkerPosition {
		public string Id { get; set; }
		public string MapId { get; set; }
		public string GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
 }
