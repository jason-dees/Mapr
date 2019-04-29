﻿
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

        public async Task SendMarker(Marker marker) {
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
				.Select(marker => new Marker {
					Id = marker.Id,
					MapId = marker.MapId,
					GameId = marker.GameId,
					Name = marker.Name,
					Description = marker.Description,
					CustomCss = marker.CustomCss,
					X = marker.X,
					Y = marker.Y,
                    HasIcon = !string.IsNullOrEmpty(marker.ImageUri)
                });

			await Clients.Caller.SendAsync("SetAllMapMarkers", mapMarkers);
        }

		public async Task CreateMarker(Marker marker) {
			if(!await CheckGameAndMapOwnership(marker.GameId, marker.MapId)) { return; }
            var newMarker = new MarkerModel {
                MapId = marker.MapId,
                GameId = marker.GameId,
                Name = marker.Name,
                Description = marker.Description,
                CustomCss = marker.CustomCss,
                X = marker.X,
                Y = marker.Y,
                ImageBytes = new byte[]
            };
            marker.Id = (await _markerStore.AddMarker(newMarker)).Id;

            await Clients.Group(marker.GameId).SendAsync("SetMarker", marker);
		}

        public async Task MoveMarker(string markerId, int x, int y) {
            var marker = await _markerStore.GetMarker(markerId);
            if(!await CheckGameAndMapOwnership(marker.GameId, marker.MapId)) { return; }

            marker.X = x;
            marker.Y = y;
            await _markerStore.UpdateMarker(marker);
            await Clients.Group(marker.GameId).SendAsync("SetMarker", marker);
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
	}

    public class Marker {
        public string Id { get; set; }
		public string MapId { get; set; }
		public string GameId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CustomCss { get; set; }
        public string IconUrl { get => $"/games/{GameId}/maps/{MapId}/markers/{Id}"; }
        public bool HasIcon { get; set; }

        public override bool Equals(object obj) {
            if(typeof(Marker) != obj.GetType()) { return false; }

            return Id == ((Marker)obj).Id;
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
