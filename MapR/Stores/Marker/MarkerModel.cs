using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Stores.Marker {
	public class MarkerModel : TableEntity {

		[IgnoreProperty]
		public string Id { get => RowKey; set { RowKey = value; } }
		[IgnoreProperty]
		public string GameId { get => PartitionKey; set { PartitionKey = value; } }

		public string MapId { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CustomCss { get; set; }

	}
}
