using MapR.Data.Models;
using MapR.Data.Stores;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public class MapStore : IStoreMaps {

        readonly IStoreGames _gameStore;

        public MapStore(IStoreGames gameStore,
            CloudBlobContainer mapContainer) {
            _gameStore = gameStore;
        }

        public async Task<bool> AddMap(string owner, string gameId, MapModel map) {
            var game = await _gameStore.GetGame(owner, map.GameId);
            throw new NotImplementedException();
        }

        public Task DeleteMap(string owner, string gameId, string mapId) {
            throw new NotImplementedException();
        }

        public Task DeleteMap(string owner, string gameId) {
            throw new NotImplementedException();
        }

        public Task<MapModel> GetActiveMap(string owner, string gameId) {
            throw new NotImplementedException();
        }

        public Task<MapModel> GetMap(string owner, string gameId, string mapId) {
            throw new NotImplementedException();
        }

        public Task<IList<MapModel>> GetMaps(string owner, string gameId) {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, MapModel map) {
            throw new NotImplementedException();
        }

        public Task<bool> ReplaceMapImage(MapModel map) {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMap(string owner, string gameId, string mapId, MapModel map) {
            throw new NotImplementedException();
        }
    }
}
