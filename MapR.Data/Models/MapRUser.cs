using System;
using Microsoft.AspNetCore.Identity;

namespace MapR.Data.Models {
	public class MapRUser : IdentityUser {
		public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string NameIdentifier { get; set; }
        
	}
}
