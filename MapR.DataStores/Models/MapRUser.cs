using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace MapR.DataStores.Models {
	public class MapRUser : Data.Models.MapRUser, ITableEntity {

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext) {
			MapRUser entity = TableEntity.ConvertBack<MapRUser>(properties, operationContext);
			this.Email = entity.Email;
			this.UserName = entity.UserName;
			this.PartitionKey = entity.PartitionKey;
			this.RowKey = entity.RowKey;
			this.Timestamp = entity.Timestamp;
			this.ETag = entity.ETag;
			this.LoginProvider = entity.LoginProvider;
            this.ProviderKey = entity.ProviderKey;
            this.NameIdentifier = entity.NameIdentifier;
		}

		public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) {
			IDictionary<string, EntityProperty> flattenedProperties = TableEntity.Flatten(this, operationContext);
			return flattenedProperties;
		}
	}
}
