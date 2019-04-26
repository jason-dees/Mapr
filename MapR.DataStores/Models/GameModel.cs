using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Models {
    public class GameModel : TableEntity, Data.Models.GameModel {
        [IgnoreProperty]
        public string Id { get => RowKey; set { RowKey = value; } }

		public string Owner { get => PartitionKey; set { PartitionKey = value; } }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public DateTimeOffset LastPlayed { get => this.Timestamp; set { this.Timestamp = value; } }
    }
}