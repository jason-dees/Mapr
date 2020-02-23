using System;
using Microsoft.AspNetCore.Http;

namespace MapR.Functions.Extensions {
	public static class HttpRequestExtensions {
		private readonly static string _cookieName = "x-ms-client-principal-name";
		public static string GetAnonId(this HttpRequest req) {
			return req.Cookies[_cookieName];
		}
		public static string SetAnonId(this HttpRequest req, string id) {
			var newId = Guid.NewGuid().ToString();
			req.HttpContext.Response.Cookies.Append(_cookieName, newId);
			return newId;
		}
	}
}
