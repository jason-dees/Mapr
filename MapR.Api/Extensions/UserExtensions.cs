using System;
using System.Linq;
using System.Security.Claims;

namespace MapR.Api.Extensions {
    public static class UserExtensions {
        public static bool CheckIsSignedIn(this ClaimsPrincipal user) {
            return user.Claims.Any();
        }

        public static string GetUserName(this ClaimsPrincipal user) {
            if (!user.CheckIsSignedIn()) {  
                return "";
            }
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public static string GetEmail(this ClaimsPrincipal user) {
            if (!user.CheckIsSignedIn()) {
                return "";
            }
            return user.FindFirstValue(ClaimTypes.Email);
        }
    }
}
