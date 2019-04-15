using System;
using Microsoft.AspNetCore.Identity;

namespace MapR.DataStores {
	public class MapRIdentityResult : IdentityResult {
		public MapRIdentityResult(bool succeeded = true) {
			base.Succeeded = succeeded;
		}
	}
}
