using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public class MarkerStore : IStoreMarkers {
        public Task<IAmAMarkerModel> AddMarker(IAmAMarkerModel marker) {
            throw new NotImplementedException();
        }

        public Task DeleteMarker(string markerId) {
            throw new NotImplementedException();
        }

        public Task<IAmAMarkerModel> GetMarker(string markerId) {
            throw new NotImplementedException();
        }

        public Task<IList<IAmAMarkerModel>> GetMarkers(string mapId) {
            throw new NotImplementedException();
        }

        public Task UpdateMarker(IAmAMarkerModel marker) {
            throw new NotImplementedException();
        }
    }
}
