using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using MapR.DataStores.Configuration;
using MapR.DataStores.Extensions;
using MapR.DataStores.Models;

namespace MapR.DataStores {

	public class MarkerStore : IStoreMarkers {

		readonly static List<MarkerModel> _markers = new List<MarkerModel>();

        readonly IMapper _mapper;
        public MarkerStore(IMapper mapper) {
            _mapper = mapper;
        }

        public async Task<Data.Models.MarkerModel> AddMarker(Data.Models.MarkerModel newMarker) {
            var marker = _mapper.Map<MarkerModel>(newMarker);
            marker.GenerateRandomId();
            marker.Id = "M" + marker.Id;
            _markers.Add(marker);
            return marker;
		}

		public async Task DeleteMarker(Data.Models.MarkerModel newMarker) {
            var marker = _mapper.Map<MarkerModel>(newMarker);
            _markers.Remove(marker);
		}

		public async Task<Data.Models.MarkerModel> GetMarker(string gameId, string mapId, string markerId) {
			return _markers.FirstOrDefault(m => m.GameId == gameId && m.MapId == mapId && m.Id == markerId);
		}

        public async Task<Data.Models.MarkerModel> GetMarker(string markerId) {
            return _markers.FirstOrDefault(m => m.Id == markerId);
        }

        public async Task<IList<Data.Models.MarkerModel>> GetMarkers(string gameId, string mapId) {
			return _markers.Where(m => m.GameId == gameId && m.MapId == mapId)
                .Select(m => m as Data.Models.MarkerModel).ToList();
		}

		public async Task UpdateMarker(Data.Models.MarkerModel marker) {
			var currentMarker = _markers.FirstOrDefault(m => m.Id == marker.Id);
			currentMarker.CustomCss = marker.CustomCss;
			currentMarker.Description = marker.Description;
			currentMarker.X = marker.X;
			currentMarker.Y = marker.Y;
		}
	}
}
