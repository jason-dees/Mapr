using System;
using Microsoft.AspNetCore.Identity;

namespace MapR.Data.Models {
	public class MapRRole : IdentityRole {
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string ETag { get; set; }

	}
}
