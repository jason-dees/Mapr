using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace MapR.Identity.Models {
	public class ApplicationUser : IdentityUser, ITableEntity {
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string ETag { get; set; }

		public string AuthenticationSource { get; set; }

		public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext) {
			ApplicationUser entity = TableEntity.ConvertBack<ApplicationUser>(properties, operationContext);
			this.Email = entity.Email;
			this.UserName = entity.UserName;
			this.PartitionKey = entity.PartitionKey;
			this.RowKey = entity.RowKey;
			this.Timestamp = entity.Timestamp;
			this.ETag = entity.ETag;
			this.AuthenticationSource = entity.AuthenticationSource;
		}

		public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) {
			this.AuthenticationSource = "Google";
			IDictionary<string, EntityProperty> flattenedProperties = TableEntity.Flatten(this, operationContext);
			return flattenedProperties;
		}
	}
}
