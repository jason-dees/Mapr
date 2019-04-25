using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.Data.Stores;
using MapR.DataStores.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using MapR.DataStores.Models;
using MapR.DataStores.Extensions;

namespace MapR.DataStores {


    public class MapStore : IStoreMaps {

        readonly CloudTable _mapTable;
        readonly CloudBlobContainer _mapContainer;
		readonly IMapper _mapper;

        public MapStore(CloudTable mapTable, 
            CloudBlobContainer mapContainer,
			IAmDataStoreMapper mapper) {

            _mapTable = mapTable;
            _mapContainer = mapContainer;
			_mapper = mapper;
        }

		async Task InsertOrMerge(Data.Models.MapModel mapModel) {
			var map = _mapper.Map<MapModel>(mapModel);

			TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(map);
			await _mapTable.ExecuteAsync(insertOrMergeOperation);

		}

		async Task<string> UploadMapImage(string gameId, string mapId, byte[] mapBytes) {
			CloudBlockBlob cloudBlockBlob = _mapContainer.GetBlockBlobReference($"{gameId}-{mapId}");
			await cloudBlockBlob.UploadFromByteArrayAsync(mapBytes, 0, mapBytes.Length);
			return cloudBlockBlob.Name;
		}

        public async Task<bool> AddMap(Data.Models.MapModel mapModel) {
			var map = _mapper.Map<MapModel>(mapModel);
            map.GenerateRandomId();
            map.IsActive = true;

			map.ImageUri = await UploadMapImage(map.GameId, map.Id, map.ImageBytes);

			await InsertOrMerge(map);

            return true;
        }

        public async Task<bool> DeleteMap(string mapId) {
            var map = await GetMapModel(mapId);

            var blob = _mapContainer.GetBlobReference(map.ImageUri);
            await blob.DeleteAsync();

            TableOperation delete = TableOperation.Delete(map);

            await _mapTable.ExecuteAsync(delete);

            return true;
        }

        public async Task<Data.Models.MapModel> GetMap(string mapId) {
			return await GetMapModel(mapId);
		}

        public async Task<IList<Data.Models.MapModel>> GetMaps(string gameId) {
            var ownerQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<MapModel>()
                .Where(ownerQuery);

            var mapResult = await _mapTable.ExecuteQuerySegmentedAsync(query, null);

            return (IList<Data.Models.MapModel>)mapResult.ToList();
        }

		public async Task<bool> ReplaceMapImage(Data.Models.MapModel map) {
			map.ImageUri = await UploadMapImage(map.GameId, map.Id, map.ImageBytes);
			return await UpdateMap(map);
		}

		public async Task<bool> UpdateMap(Data.Models.MapModel map) {
			var originalMap = await GetMap(map.Id);

			originalMap.IsPrimary = map.IsPrimary;
			originalMap.IsActive = map.IsActive;
			originalMap.Name = map.Name;

			await InsertOrMerge(originalMap);

			return true;
		}

		async Task<MapModel> GetMapModel(string mapId) {
			var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, mapId);

			var query = new TableQuery<MapModel>()
				.Where(idQuery);

			var map = (await _mapTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();

			await map.LoadImageBytes(_mapContainer);

			return map;
		}
    }
}