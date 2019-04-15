using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Data.Models;
using MapR.Data.Stores;

namespace MapR.DataStores {


	public class MarkerStore : IStoreMarkers {

		readonly List<MarkerModel> _markers = new List<MarkerModel>();

		public async Task AddMarker(MarkerModel marker) {
			_markers.Add(marker);
		}

		public async Task DeleteMarker(MarkerModel marker) {
			_markers.Remove(marker);
		}

		public async Task<MarkerModel> GetMarker(string gameId, string mapId, string markerId) {
			return _markers.FirstOrDefault(m => m.GameId == gameId && m.MapId == mapId && m.Id == markerId);
		}

		public async Task<IList<MarkerModel>> GetMarkers(string gameId, string mapId) {
			return _markers.Where(m => m.GameId == gameId && m.MapId == mapId).ToList();
		}

		public async Task UpdateMarker(MarkerModel marker) {
			var currentMarker = _markers.FirstOrDefault(m => m.Id == marker.Id);
			currentMarker.CustomCss = marker.CustomCss;
			currentMarker.Description = marker.Description;
			currentMarker.X = marker.X;
			currentMarker.Y = marker.Y;
		}
	}
}
