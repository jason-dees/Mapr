
namespace MapR.Data.Models {
	public interface MarkerModel {

		string Id { get; set; }
		string GameId { get; set; }
		string MapId { get; set; }
		int X { get; set; }
		int Y { get; set; }
		string Name { get; set; }
		string Description { get; set; }
		string CustomCss { get; set; }
		string ImageUri { get; set; }
		byte[] ImageBytes { get; set; }

	}
}
