using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapR.Extensions;
using MapR.Stores.Map;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Map {

    public interface IStoreMaps {
        Task<bool> AddMap(MapModel map);
        Task<MapModel> GetMap(string mapId);
        Task<IList<MapModel>> GetMaps(string gameId);
        Task<bool> DeleteMap(string mapId);
    }

    public class MapStore : IStoreMaps {

        readonly CloudTable _mapTable;
        readonly CloudBlobContainer _mapContainer;

        public MapStore(CloudTable mapTable, 
            CloudBlobContainer mapContainer) {
            _mapTable = mapTable;
            _mapContainer = mapContainer;
        }

        public async Task<bool> AddMap(MapModel map) {
            map.GenerateRandomId();
            map.IsActive = true;

            CloudBlockBlob cloudBlockBlob = _mapContainer.GetBlockBlobReference($"{map.GameId}-{map.Id}");
            await cloudBlockBlob.UploadFromByteArrayAsync(map.ImageBytes, 0, map.ImageBytes.Length);

            map.ImageUri = cloudBlockBlob.Name;

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(map);
            await _mapTable.ExecuteAsync(insertOrMergeOperation);

            return true;
        }

        public async Task<bool> DeleteMap(string mapId) {
            var map = await GetMap(mapId);

            var blob = _mapContainer.GetBlobReference(map.ImageUri);
            await blob.DeleteAsync();

            TableOperation delete = TableOperation.Delete(map);

            await _mapTable.ExecuteAsync(delete);

            return true;
        }

        public async Task<MapModel> GetMap(string mapId) {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, mapId);

            var query = new TableQuery<MapModel>()
                .Where(idQuery);

            var map = (await _mapTable.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();

            return map;
        }

        public async Task<IList<MapModel>> GetMaps(string gameId) {
            var ownerQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, gameId);

            var query = new TableQuery<MapModel>()
                .Where(ownerQuery);

            var mapResult = await _mapTable.ExecuteQuerySegmentedAsync(query, null);
            foreach(var map in mapResult) {
                if (map.IsActive) {
                    await map.LoadImageBytes(_mapContainer);
                }
            }

            return mapResult.ToList();
        }
    }
}