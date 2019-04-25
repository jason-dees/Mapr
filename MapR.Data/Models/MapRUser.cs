using System;
using Microsoft.AspNetCore.Identity;

namespace MapR.Data.Models {
	public class MapRUser : IdentityUser {
		public string PartitionKey { get; set; }
		public string RowKey { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string ETag { get; set; }

		public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string NameIdentifier { get; set; }
        
	}
}
