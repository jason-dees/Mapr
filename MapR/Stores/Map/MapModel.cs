﻿using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Stores.Map {
    public class MapModel : TableEntity {

        [IgnoreProperty]
        public string Id => RowKey;
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