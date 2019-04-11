using System;
using System.Linq;
using System.Threading.Tasks;
using MapR.Stores.Map;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Extensions {
    public static class TableEntityExtensions {
        public static void GenerateRandomId(this TableEntity entity) {
            entity.RowKey = RandomString(6);
        }

        private static readonly Random random = new Random();
        private static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static async Task LoadImageBytes(this MapModel map, CloudBlobContainer mapContainer) {
            var mapBlob = mapContainer.GetBlobReference(map.ImageUri);

            await mapBlob.DownloadToByteArrayAsync(map.ImageBytes, 0);
        }
    }
}
