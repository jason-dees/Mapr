using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MapR.Functions.Extensions;
using MapR.Functions.Models.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace MapR.Functions {
	
	public static class SignalRFunctions {
		[FunctionName("negotiate")]
		//[AnonAuthFunction]
		public static SignalRConnectionInfo Negotiate(
					[HttpTrigger(AuthorizationLevel.Anonymous)]
					HttpRequest req,
			ClaimsPrincipal user,
					IBinder binder) {
			var userId = "";
			if (string.IsNullOrEmpty(userId = user.GetUserName())) {
				userId = req.GetAnonId();
			}
			// connectionInfo contains an access key token with a name identifier claim set to the authenticated user
			//Using imperative binding to create a new connection, not sure it's working though for sending messages
			SignalRConnectionInfoAttribute attribute = new SignalRConnectionInfoAttribute {
				HubName = "mapr",
				UserId = userId
			};
			SignalRConnectionInfo connection = binder.BindAsync<SignalRConnectionInfo>(attribute).Result;
			return connection;
		}

		[FunctionName("AddToGame")]
		public static async Task<IActionResult> AddToGame(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] string body,
			HttpRequest req,
			ClaimsPrincipal user,
			[SignalR(HubName = "mapr")]IAsyncCollector<SignalRMessage> signalRMessages,
			[SignalR(HubName = "mapr")] IAsyncCollector<SignalRGroupAction> signalRGroupActions) {

			var addToGame = JsonConvert.DeserializeObject<AddToGame>(body);
			var userId = user.IsAnonymous() ? req.GetAnonId(): user.GetUserName();

			await signalRGroupActions.AddAsync(
				new SignalRGroupAction {
					UserId = userId,
					GroupName = addToGame.GameId,
					Action = GroupAction.Add
				});

			var game = await FunctionServices.GameStore.GetGame(addToGame.GameId);
			var isGameOwner = game.Owner == userId;

			var maps = await FunctionServices.MapStore.GetMaps(addToGame.GameId);
			var markers = await FunctionServices.MarkerStore.GetMarkers(maps.First(m => m.IsPrimary).Id);

			await signalRMessages.AddAsync(
				new SignalRMessage {
					UserId = userId,
					Target = "SetGameData",
					Arguments = new[] { new { markers, isGameOwner, maps } },
				});
			return new OkObjectResult(userId);
		}

		[FunctionName("MoveMarker")]
		public static Task MoveMarker([HttpTrigger(AuthorizationLevel.Anonymous, "post")] string body,
			HttpRequest req,
			ClaimsPrincipal user,
			[SignalR(HubName = "mapr")]IAsyncCollector<SignalRMessage> signalRMessages,
			[SignalR(HubName = "mapr")] IAsyncCollector<SignalRGroupAction> signalRGroupActions) {

			var moveMarker = JsonConvert.DeserializeObject<MoveMarker>(body);

			return null;
		}
	}
}

//public class AnonAuthFunctionAttribute : FunctionInvocationFilterAttribute, IFunctionInvocationFilter {
	
//	public async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken) {
//		var request = executingContext.Arguments.First().Value as HttpRequest;
//		if (!string.IsNullOrEmpty(request.GetAnonId())) {
//			request.Headers.Add("x-ms-client-principal-name", request.GetAnonId());
//		}
//	}

//	public async Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken) {

//	}

//	public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken) {
//		throw new System.NotImplementedException();
//	}
//}