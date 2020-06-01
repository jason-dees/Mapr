using Microsoft.Azure.Storage.Blob;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public interface IStoreImages {
        Task<string> Upload(string name, byte[] bytes);
        Task Delete(string name);
        Task<byte[]> GetImageBytes(string imageUri);
    }

    public class ImageStore: IStoreImages {

        readonly CloudBlobContainer _container;

        public ImageStore(CloudBlobContainer container) {
            _container = container;
        }

        public async Task Delete(string imageUri) {
            var blob = _container.GetBlobReference(imageUri);
            await blob.DeleteAsync();
        }

        public async Task<byte[]> GetImageBytes(string imageUri) {
            var blob = _container.GetBlobReference(imageUri);

            var bytes = new byte[blob.StreamMinimumReadSizeInBytes];

            await blob.DownloadToByteArrayAsync(bytes, 0);

            return bytes;

        }

        public async Task<string> Upload(string name, byte[] bytes) {
            CloudBlockBlob cloudBlockBlob = _container.GetBlockBlobReference(name);
            await cloudBlockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            return cloudBlockBlob.Name;
        }
    }
}
