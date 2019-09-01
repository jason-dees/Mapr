using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Security.Claims;
using MapR.Functions.Extensions;
using System.Linq;
using MapR.Functions.Models;
using Newtonsoft.Json;
using System.IO;

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
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] string body,
			HttpRequest req,
			ClaimsPrincipal user,
			[SignalR(HubName = "mapr")]IAsyncCollector<SignalRMessage> signalRMessages,
			[SignalR(HubName = "mapr")] IAsyncCollector<SignalRGroupAction> signalRGroupActions) {

			var addToGame = JsonConvert.DeserializeObject<AddToGame>(body);

			signalRGroupActions.AddAsync(
				new SignalRGroupAction {
					UserId = user.GetUserName(),
					GroupName = addToGame.GameId,
					Action = GroupAction.Add
				}).Wait();

			var map = FunctionServices.MapStore.GetMaps(addToGame.GameId).Result;
			var markers = FunctionServices.MarkerStore.GetMarkers(map.First(m => m.IsPrimary).Id).Result;

			return signalRMessages.AddAsync(
				new SignalRMessage {
					UserId = user.GetUserName(),
					Target = "SetAllMapMarkers",
					Arguments = new[] { markers }
				});
		}

		[FunctionName("SendAllMapMarkers")]
		public static Task SendAllMapMarkers(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] string gameId,
			ClaimsPrincipal user,
			[SignalR(HubName = "mapr")]IAsyncCollector<SignalRMessage> signalRMessages) {
			
			var map = FunctionServices.MapStore.GetMaps(gameId).Result.FirstOrDefault(m => m.IsPrimary);
			var markers = FunctionServices.MarkerStore.GetMarkers(map.Id).Result;

			return signalRMessages.AddAsync(
				new SignalRMessage {
					UserId = user.GetUserName(),
					Target = "SetAllMapMarkers",
					Arguments = new[] {"o","b"}
				});

		}
	}
}

