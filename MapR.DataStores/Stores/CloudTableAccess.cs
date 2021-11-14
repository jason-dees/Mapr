using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace MapR.DataStores.Stores
{

    public interface IAccessCloudTableData<TModel> where TModel : ITableEntity, new()
    {
        Task Delete(string rowKey);
        Task<TModel> GetByRowKey(string rowKey);
        Task<IList<TModel>> GetByPartitionKey(string partitionKey);
        Task InsertOrMerge(TModel model);
        Task<bool> IsUniqueKey(string rowKey);
        Task Replace(TModel model);
        Task<TableResult> Retrieve(string partition, string rowKey);
    }

    public class CloudTableAccess<TModel> : IAccessCloudTableData<TModel> where TModel : ITableEntity, new()
    {

        protected readonly CloudTable _table;

        //This cache implementation is bad, probably
        protected static readonly List<TModel> _cache = new List<TModel>();

        public CloudTableAccess(CloudTable table)
        {
            _table = table;
        }
        public async Task Delete(string rowKey)
        {
            _cache.Remove(_cache.FirstOrDefault(m => m.RowKey == rowKey));

            var model = await GetByRowKey(rowKey);
            TableOperation delete = TableOperation.Delete(model);

            await _table.ExecuteAsync(delete);
        }
        public async Task<TModel> GetByRowKey(string rowKey)
        {
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

        public async Task<IList<TModel>> GetByPartitionKey(string partitionKey)
        {
            var objs = _cache.Where(m => m.PartitionKey == partitionKey).ToList();
            if (objs.Any()) return objs;

            var ownerQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

            var query = new TableQuery<TModel>()
                .Where(ownerQuery);

            objs = (await _table.ExecuteQuerySegmentedAsync(query, null)).Results.ToList();
            _cache.AddRange(objs);

            return objs;
        }

        public async Task InsertOrMerge(TModel model)
        {
            _cache.Add(model);

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(model);
            await _table.ExecuteAsync(insertOrMergeOperation);
        }

        public async Task<bool> IsUniqueKey(string key)
        {
            var idQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, key);

            var query = new TableQuery<TModel>()
                .Where(idQuery);

            return !(await _table.ExecuteQuerySegmentedAsync(query, null)).Results.Any();
        }

        public async Task Replace(TModel model)
        {
            TableOperation operation = TableOperation.Replace(model);

            await _table.ExecuteAsync(operation);
        }

        public async Task<TableResult> Retrieve(string partition, string key)
        {
            TableOperation operation = TableOperation.Retrieve<TModel>(partition, key);
            var result = await _table.ExecuteAsync(operation);

            return result;
        }
    }
}
