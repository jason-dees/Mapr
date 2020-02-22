using System;
using System.Linq;
using System.Security.Claims;

namespace MapR.Functions.Extensions {
    public static class UserExtensions {
        public static bool CheckIsSignedIn(this ClaimsPrincipal user) {
            return user.Identity.IsAuthenticated;
        }

        public static string GetUserName(this ClaimsPrincipal user) {
            return user.Identity.Name;
        }

		public static bool IsAnonymous(this ClaimsPrincipal user) {
            return string.IsNullOrEmpty(user.Identity.Name);
		}
    }
}
