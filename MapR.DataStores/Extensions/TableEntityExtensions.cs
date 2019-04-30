using System;
using System.Linq;
using System.Threading.Tasks;
using MapR.DataStores.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Extensions {
    public static class TableEntityExtensions {
        public static void GenerateRandomId(this ITableEntity entity) {
            entity.RowKey = RandomString(6);
        }

        private static readonly Random random = new Random();
        private static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static async Task LoadImageBytes(this IHaveImageData obj, CloudBlobContainer blobContainer) {
            var blob = blobContainer.GetBlobReference(obj.ImageUri);

            obj.ImageBytes = new byte[blob.StreamMinimumReadSizeInBytes];

            await blob.DownloadToByteArrayAsync(obj.ImageBytes, 0);
        }
    }
}
