﻿using MapR.Extensions;
using MapR.Map;
using MapR.Stores.Game;
using MapR.Stores.Marker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.Hubs {

	[Authorize]
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

        //static List<MapMarker> _mapMarkers = new List<MapMarker>();

        public async Task SendMapMarker(MapMarker marker) {
            await Clients.Group(marker.GameId).SendAsync("SetMarker", marker);
			await _markerStore.UpdateMarker(new MarkerModel {
				Id = marker.Id,
				MapId = marker.MapId,
				GameId = marker.GameId,
				Name = marker.Name,
				Description = marker.Description,
				CustomCss = marker.CustomCss,
				X = marker.X,
				Y = marker.Y
			});
        }

        public async Task SendAllMapMarkers(string gameId, string mapId) {
			var mapMarkers = (await _markerStore.GetMarkers(gameId, mapId))
				.Select(marker => new MapMarker {
					Id = marker.Id,
					MapId = marker.MapId,
					GameId = marker.GameId,
					Name = marker.Name,
					Description = marker.Description,
					CustomCss = marker.CustomCss,
					X = marker.X,
					Y = marker.Y
				});

			await Clients.Group(gameId).SendAsync("SetAllMapMarkers", mapMarkers);
        }

		public async Task CreateMapMarker(MapMarker marker) {
			//Context.UserIdentifier
			if(await CheckGameAndMap(marker.GameId, marker.MapId)) { return; }

			await _markerStore.AddMarker(new MarkerModel {
				Id = marker.Id,
				MapId = marker.MapId,
				GameId = marker.GameId,
				Name = marker.Name,
				Description = marker.Description,
				CustomCss = marker.CustomCss,
				X = marker.X,
				Y = marker.Y
			});

			await Clients.Group(marker.GameId).SendAsync("SetMarker", marker);
		}

		public async Task AddToGame(string gameId) {
			await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
		}

		public async Task RemoveFromGame(string gameId) {
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
		}

		async Task<bool> CheckGameAndMap(string gameId, string mapId) {

			var game = await _gameStore.GetGame(gameId);
			if (game.Owner != Context.User.GetUserName()) { return false; }

			var map = await _mapStore.GetMap(mapId);
			if (map.GameId != game.Id) { return false; }

			return true;
		}
	}

    public class MapMarker {
        public string Id { get; set; }
		public string MapId { get; set; }
		public string GameId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CustomCss { get; set; }

        public override bool Equals(object obj) {
            if(typeof(MapMarker) != obj.GetType()) { return false; }

            return Id == ((MapMarker)obj).Id;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, X, Y);
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
