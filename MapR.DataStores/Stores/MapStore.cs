using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using MapR.DataStores.Models;
using MapR.DataStores.Extensions;
using MapR.Data.Models;

namespace MapR.DataStores.Stores {

    public class MapStore : IStoreMaps {
    
        readonly IMapper _mapper;
        readonly IAccessCloudTableData<MapModel> _cloudTableAccess;
        readonly IAccessCloudBlobData _cloudBlobAccess;
        public MapStore(IAccessCloudTableData<MapModel> cloudTableAccess, 
            IAccessCloudBlobData  cloudBlobData,
			IMapper mapper) {

            _cloudTableAccess = cloudTableAccess;
            _cloudBlobAccess = cloudBlobData;
            _mapper = mapper;
        }

        public async Task<string> AddMap(string owner, string gameId, IAmAMapModel mapModel, byte[] imageBytes) {
			var map = _mapper.Map<MapModel>(mapModel);
            map.GenerateRandomId();
            map.IsActive = true;

			map.ImageUri = await _cloudBlobAccess.UploadBlob(map.ImageBlobName, map.ImageBytes);

			await _cloudTableAccess.InsertOrMerge(map);

            return map.Id;
        }

        public async Task DeleteMap(string owner, string gameId, string mapId) {
            await _cloudTableAccess.Delete(mapId);
        }

        public async Task<Data.Models.IAmAMapModel> GetMap(string owner, string gameId, string mapId) {
			return await _cloudTableAccess.GetByRowKey(mapId);
        }

        public async Task<Data.Models.IAmAMapModel> GetActiveMap(string owner, string gameId) {
            var map = (await _cloudTableAccess.GetByPartitionKey(gameId)).Select(m => m as Data.Models.IAmAMapModel).FirstOrDefault( m => m.IsPrimary);
            return map;
        }

        public async Task<IList<Data.Models.IAmAMapModel>> GetMaps(string owner, string gameId) {
            return (await _cloudTableAccess.GetByPartitionKey(gameId)).Select(m => m as Data.Models.IAmAMapModel).ToList();
        }

		public async Task<bool> ReplaceMapImage(string owner, string gameId, string mapId, byte[] imageBytes) {
            var map = await GetMap(owner, gameId, mapId);
            map.ImageUri = await _cloudBlobAccess.UploadBlob((map as MapModel).ImageBlobName, imageBytes);
			return await UpdateMap(owner, gameId, mapId, map);
		}

		public async Task<bool> UpdateMap(string owner, string gameId, string mapId, IAmAMapModel map) {
			var originalMap = await _cloudTableAccess.GetByRowKey(map.Id);

			originalMap.IsPrimary = map.IsPrimary;
			originalMap.IsActive = map.IsActive;
			originalMap.Name = map.Name;

			await _cloudTableAccess.InsertOrMerge(originalMap);

			return true;
		}

        public async Task<byte[]> GetMapImage(string owner, string gameId, string mapId) {
            var map = await GetMap(owner, gameId, mapId);
            return await _cloudBlobAccess.GetBlobBytes((map as MapModel).ImageBlobName);
        }

        public Task<IList<IAmAMapModel>> GetMaps(string gameId) {
            throw new System.NotImplementedException();
        }

        public Task<IAmAMapModel> GetMap(string gameId, string mapId) {
            throw new System.NotImplementedException();
        }
    }
}