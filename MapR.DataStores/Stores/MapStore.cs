using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.DataStores.Models;
using MapR.DataStores.Extensions;

namespace MapR.DataStores.Stores {

    public class MapStore : ImageStore<MapModel>, IStoreMaps {
    
        readonly IMapper _mapper;
        public MapStore(CloudTable mapTable, 
            CloudBlobContainer mapContainer,
			IMapper mapper) : base(mapTable, mapContainer) {
            _mapper = mapper;
        }

        public async Task<bool> AddMap(Data.Models.MapModel mapModel) {
			var map = _mapper.Map<MapModel>(mapModel);
            map.GenerateRandomId();
            map.IsActive = true;

			map.ImageUri = await UploadImage(map.ImageBlobName, map.ImageBytes);

			await InsertOrMerge(map);

            return true;
        }

        public async Task DeleteMap(string mapId) {
            await Delete(mapId);
        }

        public async Task<Data.Models.MapModel> GetMap(string mapId) {
			return await GetByRowKey(mapId);
		}

        public async Task<IList<Data.Models.MapModel>> GetMaps(string gameId) {
            return (await GetByPartitionKey(gameId)).Select(m => m as Data.Models.MapModel).ToList();
        }

		public async Task<bool> ReplaceMapImage(Data.Models.MapModel mapModel) {
            var map = _mapper.Map<MapModel>(mapModel);
            map.ImageUri = await UploadImage(map.ImageBlobName, map.ImageBytes);
			return await UpdateMap(map);
		}

		public async Task<bool> UpdateMap(Data.Models.MapModel map) {
			var originalMap = await GetByRowKey(map.Id);

			originalMap.IsPrimary = map.IsPrimary;
			originalMap.IsActive = map.IsActive;
			originalMap.Name = map.Name;

			await InsertOrMerge(originalMap);

			return true;
		}
    }
}