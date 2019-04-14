using System;
using System.Linq;
using MapR.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Stores.Game {
    public class GameModel : TableEntity {

        public GameModel() { }

        public GameModel(string owner) {
            Owner = owner;
            PartitionKey = owner;
            this.GenerateRandomId();
        }
        [IgnoreProperty]
        public string Id => RowKey;

        public string Owner { get => PartitionKey; set { PartitionKey = value; } }
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
    }
}
