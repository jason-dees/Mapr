using System;
namespace MapR.Functions.Models.Messages {
	public class MoveMarker {
		public string GameId { get; set; }
		public string MarkerId { get; set; }
		public string X { get; set; }
		public string Y { get; set; }
	}
}
