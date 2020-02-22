using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Security.Claims;
using MapR.Functions.Extensions;
using System.Linq;
using Newtonsoft.Json;
using MapR.Functions.Models.Messages;
using System;
using Microsoft.Extensions.Primitives;

namespace MapR.Functions {
	public static class SignalRFunctions {
		[FunctionName("negotiate")]
		public static SignalRConnectionInfo Negotiate(
			[HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
			[SignalRConnectionInfo(HubName = "mapr")]
			SignalRConnectionInfo connectionInfo,
			ClaimsPrincipal user,
			IBinder binder){
			var userId = req.Headers["{headers.x-ms-client-principal-name}"];//{} for unauthenticated person
			if (userId == default(StringValues)){
				userId = Guid.NewGuid().ToString();
				var identity = new ClaimsIdentity();
				identity.AddClaim(new Claim(ClaimTypes.Name, userId));
				user.AddIdentity(identity);
			}
			
			// connectionInfo contains an access key token with a name identifier claim set to the authenticated user
			SignalRConnectionInfoAttribute attribute = new SignalRConnectionInfoAttribute {
				HubName = "mapr",
				UserId = userId
			};
			SignalRConnectionInfo connection = binder.BindAsync<SignalRConnectionInfo>(attribute).Result;
			return connection;
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

			var game = FunctionServices.GameStore.GetGame(addToGame.GameId).Result;
			var isGameOwner = game.Owner == user.GetUserName();

			var map = FunctionServices.MapStore.GetMaps(addToGame.GameId).Result;
			var markers = FunctionServices.MarkerStore.GetMarkers(map.First(m => m.IsPrimary).Id).Result;

			return signalRMessages.AddAsync(
				new SignalRMessage {
					UserId = user.GetUserName(),
					Target = "SetGameData",
					Arguments = new[] { new { markers, isGameOwner } }
				});
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