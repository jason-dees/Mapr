using System;
using Microsoft.AspNetCore.Http;

namespace MapR.Functions.Extensions {
	public static class HttpRequestExtensions {
		public static string GetAnonId(this HttpRequest req) {
			return req.Cookies["anon-id"];
		}
		public static string SetAnonId(this HttpRequest req, string id) {
			var newId = Guid.NewGuid().ToString();
			req.HttpContext.Response.Cookies.Append("anon-id", newId);
			return newId;
		}
	}
}
