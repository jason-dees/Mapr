using MapR.Data.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Models {
    public class MapModel : TableEntity, IHaveImageData, IAmAMapModel {

        [IgnoreProperty]
		public string Id { get => RowKey; set { RowKey = value; } }
        [IgnoreProperty]
        public byte[] ImageBytes { get; set; }
        [IgnoreProperty]
        public string GameId { get => PartitionKey; set { PartitionKey = value; } }
        [IgnoreProperty]
        public string ImageBlobName { get => $"{GameId}-{Id}"; }

        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }

    }
}