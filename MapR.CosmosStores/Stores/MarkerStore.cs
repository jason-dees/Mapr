using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public class MarkerStore : IStoreMarkers {
        public Task<MarkerModel> AddMarker(MarkerModel marker) {
            throw new NotImplementedException();
        }

        public Task DeleteMarker(string markerId) {
            throw new NotImplementedException();
        }

        public Task<MarkerModel> GetMarker(string markerId) {
            throw new NotImplementedException();
        }

        public Task<IList<MarkerModel>> GetMarkers(string mapId) {
            throw new NotImplementedException();
        }

        public Task UpdateMarker(MarkerModel marker) {
            throw new NotImplementedException();
        }
    }
}
