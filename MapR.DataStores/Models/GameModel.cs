using MapR.DataStores.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.DataStores.Models {
    public class GameModel : TableEntity, Data.Models.GameModel {

        public GameModel() { }

        public GameModel(string owner) {
            Owner = owner;
            PartitionKey = owner;
            this.GenerateRandomId();
        }
        [IgnoreProperty]
        public string Id { get => RowKey; set { RowKey = value; } }

		public string Owner { get => PartitionKey; set { PartitionKey = value; } }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
    }
}