using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

namespace MapR.CosmosStores.Stores {
    public interface IStoreImages {
        Task<string> UploadAndGetUri(string name, byte[] bytes);
        Task Delete(string name);
        Task<byte[]> GetImageBytes(string imageUri);
    }

    public class ImageStore: IStoreImages {

        readonly BlobContainerClient _container;

        public ImageStore(BlobContainerClient container) {
            _container = container;
        }

        public async Task Delete(string imageUri) {
            var blob = _container.GetBlobClient(imageUri);
            await blob.DeleteAsync();
        }

        public async Task<byte[]> GetImageBytes(string imageUri) {
            var client = _container.GetBlobClient(imageUri);
            using (var stream = new MemoryStream())
            {
                var blob = await client.DownloadToAsync(stream);

                return stream.ToArray();
            }
        }

        public async Task<string> UploadAndGetUri(string name, byte[] bytes) {
            var client = _container.GetBlobClient(name);
            using(var stream = new MemoryStream())
            {
                stream.Write(bytes);
                await client.UploadAsync(stream);
            }
            return name;
        }
    }
}
