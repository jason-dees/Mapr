using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Stores {
    public abstract class CloudTableStore<TModel> where TModel : ITableEntity, new() {

        protected readonly CloudTable _table;

        protected static readonly List<TModel> _cache = new List<TModel>();

        protected CloudTableStore(CloudTable table) {
            _table = table;
        }
        protected virtual async Task InsertOrMerge(TModel model) {
            _cache.Add(model);

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(model);
            await _table.ExecuteAsync(insertOrMergeOperation);
        }

        protected virtual async Task<TModel> GetByRowKey(string rowKey) {
            var obj = _cache.FirstOrDefault(m => m.RowKey == rowKey);
            if (obj != null) return obj;

            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);

            var query = new TableQuery<TModel>()
                .Where(idQuery);

            obj = (await _table.ExecuteQuerySegmentedAsync(query, null)).Results.FirstOrDefault();
            if (obj == null) 
                return obj;
            _cache.Add(obj);

            return obj;
        }

        protected virtual async Task<IList<TModel>> GetByPartitionKey(string partitionKey) {
            var objs = _cache.Where(m => m.PartitionKey == partitionKey).ToList();
            if (objs.Any()) return objs;

            var ownerQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

            var query = new TableQuery<TModel>()
                .Where(ownerQuery);

            objs = (await _table.ExecuteQuerySegmentedAsync(query, null)).Results.ToList();
            _cache.AddRange(objs);

            return objs;
        }

        protected virtual async Task Delete(string rowKey) {
            _cache.Remove(_cache.FirstOrDefault(m => m.RowKey == rowKey));

            var model = await GetByRowKey(rowKey);
            TableOperation delete = TableOperation.Delete(model);

            await _table.ExecuteAsync(delete);
        }
    }
}
