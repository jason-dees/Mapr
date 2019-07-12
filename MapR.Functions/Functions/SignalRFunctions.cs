using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace MapR.Functions {
	public static class SignalRFunctions {
		[FunctionName("negotiate")]
		public static SignalRConnectionInfo Negotiate(
			[HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
			[SignalRConnectionInfo(HubName = "mapr", UserId = "{headers.x-ms-client-principal-id}")]
			SignalRConnectionInfo connectionInfo){
				// connectionInfo contains an access key token with a name identifier claim set to the authenticated user
				return connectionInfo;
		}

		[FunctionName("AddToGame")]
		public static Task AddToGame(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] string gameId,
			[SignalR(HubName = "mapr")] IAsyncCollector<SignalRMessage> signalRMessages,
			[SignalRConnectionInfo(HubName = "mapr", UserId = "{headers.x-ms-client-principal-id}")]
				SignalRConnectionInfo connectionInfo) {

			connectionInfo.
			return null;
		}
	}
}

