using AutoMapper;
using MapR.CosmosStores.Models;
using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public class MapStore : IStoreMaps {

        readonly IStoreGames _gameStore;
        readonly IStoreImages _imageStore;
        readonly IMapper _mapper;

        public MapStore(IStoreGames gameStore, //Maybe a different wrapper for this?
            IStoreImages imageStore,
            IMapper mapper) {

            _gameStore = gameStore;
            _imageStore = imageStore;
            _mapper = mapper;
        }

        public async Task<bool> AddMap(string owner, string gameId, MapModel map, byte[] imageBytes) {
            var game = await _gameStore.GetGame(owner, gameId);
            //This is too abstracted and i don't have access to the maps attribute i need
            //Upload Map image
            map.Id = GameStore.GenerateRandomId();
            map.ImageUri = await _imageStore.Upload(map.Id, imageBytes);

            (game as Game).Maps.Add(_mapper.Map<Map>(map));
            // I hate this image byte stuff
            await _gameStore.UpdateGame(owner, gameId, game);
            
            return true;
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
