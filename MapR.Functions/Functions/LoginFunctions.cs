using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MapR.Functions.Extensions;
using System;

namespace MapR.Functions.Functions {
    public static class LoginFunctions {

        [FunctionName("GoogleLogin")]
        [Authorize]
        public static IActionResult RunGoogleLogin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "login")] HttpRequest req,
            ClaimsPrincipal user,
            ILogger log) {
            return DoLogin(user, "google", req.Query["redirect"]);
        }

        [FunctionName("Login")]
        [Authorize]
        public static IActionResult RunLogin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "login/{provider}")] HttpRequest req,
            string provider,
            ClaimsPrincipal user,
            ILogger log) {
            return DoLogin(user, provider, req.Query["redirect"]);
        }

        [FunctionName("Logout")]
        public static IActionResult RunLogout(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "logout")] HttpRequest req,
            ClaimsPrincipal user,
            ILogger log) {
            return new RedirectResult("/.auth/logout");
        }

		[FunctionName("GetUserInformation")]
		[Authorize]
		public static IActionResult RunGetUserInformation(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequest req,
			ClaimsPrincipal user,
			ILogger log) {
			if (user.CheckIsSignedIn() && !string.IsNullOrEmpty(user.Identity.Name)) {
				return new OkObjectResult(new { user.Identity.Name });
			}
            req.HttpContext.Response.Cookies.Append("anon-id", Guid.NewGuid().ToString());
            return new UnauthorizedResult();
		}

        static IActionResult DoLogin(ClaimsPrincipal user, string provider, string redirectRoute) {

			redirectRoute = string.IsNullOrEmpty(redirectRoute) ? "/api/games" : redirectRoute;
			if (user.CheckIsSignedIn()) {
                return new RedirectResult(redirectRoute);
            }
            return new RedirectResult($"/.auth/login/{provider}?post_login_redirect_url={redirectRoute}");

        }
    }
}
