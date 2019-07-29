using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Security.Claims;
using MapR.Functions.Extensions;

namespace MapR.Functions {
	public static class SignalRFunctions {
		[FunctionName("negotiate")]
		public static SignalRConnectionInfo Negotiate(
			[HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
			[SignalRConnectionInfo(HubName = "mapr", UserId = "{headers.x-ms-client-principal-name}")]
			SignalRConnectionInfo connectionInfo){
				// connectionInfo contains an access key token with a name identifier claim set to the authenticated user
				return connectionInfo;
		}

		[FunctionName("AddToGame")]
		public static Task AddToGame(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] string gameId,
			ClaimsPrincipal user,
			[SignalR(HubName = "mapr")]IAsyncCollector<SignalRMessage> signalRMessages,
			[SignalR(HubName = "mapr")] IAsyncCollector<SignalRGroupAction> signalRGroupActions) {

			return signalRGroupActions.AddAsync(
				new SignalRGroupAction {
					UserId = user.GetUserName(),
					GroupName = gameId,
					Action = GroupAction.Add
				});
		}
	}
}

