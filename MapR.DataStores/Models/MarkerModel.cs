using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Models {
	public class MarkerModel : TableEntity, Data.Models.MarkerModel, IHaveImageData {

		[IgnoreProperty]
		public string Id { get => RowKey; set { RowKey = value; } }
		[IgnoreProperty]
		public string MapId { get => PartitionKey; set { PartitionKey = value; } }
        [IgnoreProperty]
        public string ImageBlobName { get => $"{MapId}-{Id}"; }
        [IgnoreProperty]
        public byte[] ImageBytes { get; set; }

        public string GameId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CustomCss { get; set; }
		public string ImageUri { get; set; }
	}
}
