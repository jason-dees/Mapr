using System.Security.Claims;

namespace MapR.Functions.Extensions {
	public static class UserExtensions {
        public static bool CheckIsSignedIn(this ClaimsPrincipal user) {
            return user.Identity.IsAuthenticated;
        }

        public static string GetUserName(this ClaimsPrincipal user) {
#if DEBUG
            //return "jhdees@gmail.com";
#endif
            return user.Identity.Name;
        }

		public static bool IsAnonymous(this ClaimsPrincipal user) {
            return string.IsNullOrEmpty(user.GetUserName());
		}
    }
}
