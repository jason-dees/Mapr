using AutoMapper;
using MapR.DataStores.Models;
using MapR.DataStores.Stores.Internal;
using MapR.Data.Models;
using MapR.Data.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapR.DataStores.Stores {
    public class MapStore : IStoreMaps {

        readonly IAmAGameContainerHelper _containerStore;
        readonly IStoreImages _imageStore;
        readonly IMapper _mapper;

        public MapStore(IAmAGameContainerHelper containerStore, //Maybe a different wrapper for this?
            IStoreImages imageStore,
            IMapper mapper) {

            _containerStore = containerStore;
            _imageStore = imageStore;
            _mapper = mapper;
        }

        public async Task<string> AddMap(string owner, string gameId, IAmAMapModel map, byte[] imageBytes) {
            var game = await GetGame(owner, gameId);
            //This is too abstracted and i don't have access to the maps attribute i need
            //Upload Map image
            map.Id = GameContainerHelper.GenerateRandomId();
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

        public async Task<IAmAMapModel> GetActiveMap(string owner, string gameId) {
            var game = await GetGame(owner, gameId);
            var map = game.Maps.FirstOrDefault(_ => _.IsActive);
            return _mapper.Map<IAmAMapModel>(map);
        }

        public async Task<IAmAMapModel> GetMap(string owner, string gameId, string mapId) {
            var game = await GetGame(owner, gameId);
            var map = game.Maps.FirstOrDefault(_ => _.Id == mapId);
            return _mapper.Map<IAmAMapModel>(map);
        }

        public async Task<IList<IAmAMapModel>> GetMaps(string owner, string gameId) {
            var game = await GetGame(owner, gameId);
            return game.Maps.Select(_mapper.Map<IAmAMapModel>).ToList();
        }

        public async Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, byte[] imageBytes) {
            var map = await GetMap(owner, gameId, mapId);
            if (!string.IsNullOrEmpty(map.ImageUri)) {
                await _imageStore.Delete(map.Id);
            }

            var uri = await _imageStore.UploadAndGetUri(mapId, imageBytes);
            map.ImageUri = uri;
            await UpdateMap(owner, gameId, mapId, map);

            return true;
        }

        public async Task<byte[]> GetMapImage(string owner, string gameId, string mapId) {
            return await _imageStore.GetImageBytes(mapId);
        }

        public async Task<bool> UpdateMap(string owner, string gameId, string mapId, IAmAMapModel map) {
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

        public async Task<IList<IAmAMapModel>> GetMaps(string gameId) {
            return (await _containerStore.GetGame(gameId)).Maps.Select(_mapper.Map<IAmAMapModel>).ToList();
        }

        public async Task<IAmAMapModel> GetMap(string gameId, string mapId) {
            return _mapper.Map<IAmAMapModel>((await _containerStore.GetGame(gameId)).Maps.First(_ => _.Id == mapId));
        }
    }
}
