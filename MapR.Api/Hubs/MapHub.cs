
using MapR.Api.Extensions;
using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.Api.Hubs {

	public class MapHub : Hub {

		readonly IStoreGames _gameStore;
		readonly IStoreMaps _mapStore;
		public MapHub(
			IStoreGames gameStore,
			IStoreMaps mapStore) {
			_mapStore = mapStore;
			_gameStore = gameStore;
		}

        public async Task SendMarker(MarkerModel marker) {
            await Clients.Group(marker.GameId).SendAsync("SetMarker", marker);
        }

        async Task SendAllMarkers(string mapId) {
			var mapMarkers = await GetMapMarkers(mapId);

			await Clients.Caller.SendAsync("SetAllMapMarkers", mapMarkers);
        }

        public async Task MoveMarker(string mapId, string markerId, int x, int y) {
            var marker = (await GetMapMarkers(mapId)).FirstOrDefault(_ => _.Id == markerId);
            if(!await CheckGameAndMapOwnership(marker.GameId, marker.MapId)) { return; }

            marker.X = x;
            marker.Y = y;
            await Task.Factory.StartNew(async () => {
                //await _markerStore.UpdateMarker(marker);
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
                Y = marker.Y
            };
        }

		async Task<IEnumerable<MarkerModel>> GetMapMarkers(string mapId) =>
			(await _mapStore.GetMap(mapId)).Markers
				.Select(MapToModel);
	}

	public class MapMarkerPosition {
		public string Id { get; set; }
		public string MapId { get; set; }
		public string GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
}
