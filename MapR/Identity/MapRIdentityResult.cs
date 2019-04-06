using System;
using Microsoft.AspNetCore.Identity;

namespace MapR.Identity {
	public class MapRIdentityResult : IdentityResult {
		public MapRIdentityResult(bool succeeded = true) {
			base.Succeeded = succeeded;
		}
	}
}
