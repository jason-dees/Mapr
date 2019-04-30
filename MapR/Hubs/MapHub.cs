
using MapR.Data.Stores;
using MapR.Extensions;
using MapR.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.Hubs {

	public class MapHub : Hub {

		readonly IStoreMarkers _markerStore;
		readonly IStoreGames _gameStore;
		readonly IStoreMaps _mapStore;
		public MapHub(IStoreMarkers markerStore,
			IStoreGames gameStore,
			IStoreMaps mapStore) {

			_markerStore = markerStore;
			_mapStore = mapStore;
			_gameStore = gameStore;
		}

        public async Task SendMarker(MarkerModel marker) {
            await Clients.Group(marker.GameId).SendAsync("SetMarker", marker);
        }

		public async Task ChangeMap(string gameId, string mapId) {
			if(!await CheckGameAndMapOwnership(gameId, mapId)) { return; }

			var maps = await _mapStore.GetMaps(gameId);

			var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);
			primaryMap.IsPrimary = false;

			var newPrimaryMap = maps.FirstOrDefault(m => m.Id == mapId);
			newPrimaryMap.IsPrimary = true;

			await _mapStore.UpdateMap(primaryMap);
			await _mapStore.UpdateMap(newPrimaryMap);

			await Clients.Group(gameId).SendAsync("SetMap", mapId);
			await SendAllMarkers(mapId);
		}

        async Task SendAllMarkers(string mapId) {
			var mapMarkers = (await _markerStore.GetMarkers(mapId))
				.Select(MapToModel);

			await Clients.Caller.SendAsync("SetAllMapMarkers", mapMarkers);
        }

        public async Task MoveMarker(string markerId, int x, int y) {
            var marker = await _markerStore.GetMarker(markerId);
            if(!await CheckGameAndMapOwnership(marker.GameId, marker.MapId)) { return; }

            marker.X = x;
            marker.Y = y;
            await Task.Factory.StartNew(async () => {
                await _markerStore.UpdateMarker(marker);
            });
            await SendMarker(MapToModel(marker));
        }

		public async Task AddToGame(string gameId) {
			//everything is public now. Information wants to be free
			await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
			var maps = await _mapStore.GetMaps(gameId);
			var primaryMap = maps.FirstOrDefault(m => m.IsPrimary);

			await Clients.Caller.SendAsync("SetMap", primaryMap.Id);
			await SendAllMarkers(primaryMap.Id);
		}

		public async Task RemoveFromGame(string gameId) {
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
		}

		async Task<bool> CheckGameAndMapOwnership(string gameId, string mapId) {

			var game = await _gameStore.GetGame(gameId);
			if (game.Owner != Context.User.GetUserName()) { return false; }

			var map = await _mapStore.GetMap(mapId);
			if (map.GameId != game.Id) { return false; }

			return true;
		}

        MarkerModel MapToModel(Data.Models.MarkerModel marker) {
            return new MarkerModel {
                Id = marker.Id,
                MapId = marker.MapId,
                GameId = marker.GameId,
                Name = marker.Name,
                Description = marker.Description,
                CustomCss = marker.CustomCss,
                X = marker.X,
                Y = marker.Y,
                HasIcon = !string.IsNullOrEmpty(marker.ImageUri)
            };
        }
	}

	public class MapMarkerPosition {
		public string Id { get; set; }
		public string MapId { get; set; }
		public string GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
}
