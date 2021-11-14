using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System.IO;

namespace MapR.DataStores.Stores
{
    public interface IAccessCloudBlobData
    {
        Task Delete(string blobName);
        Task<byte[]> GetBlobBytes(string blobName);
        Task<string> UploadBlob(string blobName, byte[] bytes);
    }

    public class CloudBlobAccess : IAccessCloudBlobData
    {
        private readonly BlobContainerClient _blobContainerClient;
        public CloudBlobAccess(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task Delete(string blobName)
        {
            await _blobContainerClient.GetBlobClient(blobName).DeleteAsync();
        }

        public async Task<byte[]> GetBlobBytes(string blobName)
        {
            var downloadResult = await _blobContainerClient.GetBlobClient(blobName).DownloadAsync();
            using (var memoryStream = new MemoryStream())
            {
                downloadResult.Value.Content.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<string> UploadBlob(string blobName, byte[] bytes)
        {
            var cloudBlobClient = _blobContainerClient.GetBlobClient(blobName);
            await cloudBlobClient.UploadAsync(System.BinaryData.FromBytes(bytes));
            return cloudBlobClient.Name;
        }
    }
}
