using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.DataStores.Models;
using MapR.DataStores.Extensions;
using MapR.Data.Models;

namespace MapR.DataStores.Stores {

    public class MapStore : ImageStore<MapModel>, IStoreMaps {
    
        readonly IMapper _mapper;
        public MapStore(CloudTable mapTable, 
            CloudBlobContainer mapContainer,
			IMapper mapper) : base(mapTable, mapContainer) {
            _mapper = mapper;
        }

        public async Task<string> AddMap(string owner, string gameId, IAmAMapModel mapModel, byte[] imageBytes) {
			var map = _mapper.Map<MapModel>(mapModel);
            map.GenerateRandomId();
            map.IsActive = true;

			map.ImageUri = await UploadImage(map.ImageBlobName, map.ImageBytes);

			await InsertOrMerge(map);

            return map.Id;
        }

        public async Task DeleteMap(string owner, string gameId, string mapId) {
            await Delete(mapId);
        }

        public async Task<Data.Models.IAmAMapModel> GetMap(string owner, string gameId, string mapId) {
			return await GetByRowKey(mapId);
        }

        public async Task<Data.Models.IAmAMapModel> GetActiveMap(string owner, string gameId) {
            var map = (await GetByPartitionKey(gameId)).Select(m => m as Data.Models.IAmAMapModel).FirstOrDefault( m => m.IsPrimary);

            await (map as MapModel).LoadImageBytes(_blobContainer);

            return map;
        }

        public async Task<IList<Data.Models.IAmAMapModel>> GetMaps(string owner, string gameId) {
            return (await GetByPartitionKey(gameId)).Select(m => m as Data.Models.IAmAMapModel).ToList();
        }

		public async Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, byte[] imageBytes) {
            var map = await GetMap(owner, gameId, mapId);
            map.ImageUri = await UploadImage(map.Id, imageBytes);
			return await UpdateMap(owner, gameId, mapId, map);
		}

		public async Task<bool> UpdateMap(string owner, string gameId, string mapId, IAmAMapModel map) {
			var originalMap = await GetByRowKey(map.Id);

			originalMap.IsPrimary = map.IsPrimary;
			originalMap.IsActive = map.IsActive;
			originalMap.Name = map.Name;

			await InsertOrMerge(originalMap);

			return true;
		}

        public async Task<byte[]> GetMapImage(string owner, string gameId, string mapId) {
            var map = await GetMap(owner, gameId, mapId);
            return await (map as MapModel).GetImageBytes(_blobContainer);
        }

        public Task<IList<IAmAMapModel>> GetMaps(string gameId) {
            throw new System.NotImplementedException();
        }

        public Task<IAmAMapModel> GetMap(string gameId, string mapId) {
            throw new System.NotImplementedException();
        }
    }
}