using AutoMapper;
using MapR.CosmosStores.Models;
using MapR.CosmosStores.Stores.Internal;
using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public class MapStore : IStoreMaps {

        readonly IStoreContainers _containerStore;
        readonly IStoreImages _imageStore;
        readonly IMapper _mapper;

        public MapStore(IStoreContainers containerStore, //Maybe a different wrapper for this?
            IStoreImages imageStore,
            IMapper mapper) {

            _containerStore = containerStore;
            _imageStore = imageStore;
            _mapper = mapper;
        }

        public async Task<string> AddMap(string owner, string gameId, MapModel map, byte[] imageBytes) {
            var game = await GetGame(owner, gameId);
            //This is too abstracted and i don't have access to the maps attribute i need
            //Upload Map image
            map.Id = ContainerStore.GenerateRandomId();
            map.GameId = gameId;
            if (imageBytes != null && imageBytes.Length > 0) {
                map.ImageUri = await _imageStore.UploadAndGetUri(map.Id, imageBytes);
            }
            else {
                map.ImageUri = "";
            }

            game.Maps.Add(_mapper.Map<Map>(map));
            await SaveGame(owner, gameId, game);
            
            return map.Id;
        }

        public async Task DeleteMap(string owner, string gameId, string mapId) {
            var game = await GetGame(owner, gameId);
            game.Maps = game.Maps.Where(_ => _.Id != mapId).ToList();
            await SaveGame(owner, gameId, game);
        }

        public async Task<MapModel> GetActiveMap(string owner, string gameId) {
            var game = await GetGame(owner, gameId);
            var map = game.Maps.FirstOrDefault(_ => _.IsActive);
            return _mapper.Map<MapModel>(map);
        }

        public async Task<MapModel> GetMap(string owner, string gameId, string mapId) {
            var game = await GetGame(owner, gameId);
            var map = game.Maps.FirstOrDefault(_ => _.Id == mapId);
            return _mapper.Map<MapModel>(map);
        }

        public async Task<IList<MapModel>> GetMaps(string owner, string gameId) {
            var game = await GetGame(owner, gameId);
            return game.Maps.Select(_mapper.Map<MapModel>).ToList();
        }

        public Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, byte[] imageBytes) {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateMap(string owner, string gameId, string mapId, MapModel map) {
            var game = await GetGame(owner, gameId);
            var oldMap = game.Maps.SingleOrDefault(_ => _.Id == mapId);
            
            if(oldMap == null) { return false; }

            game.Maps = game.Maps.Where(_ => _.Id != mapId).ToList();
            game.Maps.Add(_mapper.Map<Map>(map));
            await SaveGame(owner, gameId, game);
            
            return true;
        }

        async Task<Game> GetGame(string owner, string gameId) => await _containerStore.GetGame(owner, gameId);

        async Task SaveGame(string owner, string gameId, Game game) => await _containerStore.UpdateGame(owner, gameId, game);
    }
}
