using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MapR.DataStores.Extensions;
using MapR.DataStores.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Stores {
    public abstract class ImageStore<TModel> : CloudTableStore<TModel> where TModel : ITableEntity, IHaveImageData, new(){
    
        protected readonly CloudBlobContainer _blobContainer;

        protected ImageStore(CloudTable table,
            CloudBlobContainer blobContainer) 
            : base(table) {
            _blobContainer = blobContainer;
        }

        protected virtual async Task<string> UploadImage(string blobname, byte[] iconBytes) {
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(blobname);
            await cloudBlockBlob.UploadFromByteArrayAsync(iconBytes, 0, iconBytes.Length);
            return cloudBlockBlob.Name;
        }

        protected override async Task<TModel> GetByRowKey(string rowKey) {
            var dataObject = await base.GetByRowKey(rowKey);

            await dataObject.LoadImageBytes(_blobContainer);

            return dataObject;
        }

        protected override async Task Delete(string rowKey) {
            var model = await GetByRowKey(rowKey);

            var blob = _blobContainer.GetBlobReference(model.ImageUri);
            await blob.DeleteAsync();

            TableOperation delete = TableOperation.Delete(model);

            await _table.ExecuteAsync(delete);
        }
    }
}
