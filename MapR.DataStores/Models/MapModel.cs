using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Models {
    public class MapModel : TableEntity, Data.Models.MapModel {

        [IgnoreProperty]
		public string Id { get => RowKey; set { RowKey = value; } }
        [IgnoreProperty]
        public byte[] ImageBytes { get; set; }
        [IgnoreProperty]
        public string GameId { get => PartitionKey; set { PartitionKey = value; } }

        public string ImageUri { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }

    }
}