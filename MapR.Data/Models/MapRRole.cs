﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Data.Models {
	public class MapRRole : IdentityRole, ITableEntity {
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string ETag { get; set; }

		public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext) {
			MapRRole entity = TableEntity.ConvertBack<MapRRole>(properties, operationContext);

			this.Id = entity.Id;
			this.Name = entity.Name;
			this.PartitionKey = entity.PartitionKey;
			this.RowKey = entity.RowKey;
			this.Timestamp = entity.Timestamp;
			this.ETag = entity.ETag;
		}

		public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) {
			IDictionary<string, EntityProperty> flattenedProperties = TableEntity.Flatten(this, operationContext);
			return flattenedProperties;
		}
	}
}